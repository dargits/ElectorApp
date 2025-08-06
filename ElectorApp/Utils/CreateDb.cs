using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Cần thiết cho MessageBox.Show

namespace ElectorApp.Utils
{
    internal class CreateDb
    {
        public static void create()
        {
            // Các câu lệnh SQL để tạo các bảng cho ứng dụng bình chọn
            string sqlScript = @"
                -- Tạo bảng Users để lưu trữ thông tin người dùng
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INT PRIMARY KEY AUTO_INCREMENT,
                    Username VARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash VARCHAR(255) NOT NULL, -- Mật khẩu được lưu dưới dạng mã băm để bảo mật
                    IsAdmin BOOLEAN DEFAULT FALSE, -- TRUE cho quản trị viên, FALSE cho người dùng thông thường
                    FullName VARCHAR(100) NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );

                -- Tạo bảng Polls để lưu trữ các chủ đề bình chọn
                CREATE TABLE IF NOT EXISTS Polls (
                    PollId INT PRIMARY KEY AUTO_INCREMENT,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT NULL, -- Sử dụng TEXT cho các mô tả có thể dài (tương đương NVARCHAR(MAX))
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    IsActive BOOLEAN DEFAULT TRUE -- TRUE nếu cuộc bình chọn đang hoạt động, FALSE nếu không
                );

                -- Tạo bảng Options để lưu trữ các lựa chọn cho mỗi cuộc bình chọn
                CREATE TABLE IF NOT EXISTS Options (
                    OptionId INT PRIMARY KEY AUTO_INCREMENT,
                    PollId INT NOT NULL,
                    OptionText VARCHAR(255) NOT NULL,
                    FOREIGN KEY (PollId) REFERENCES Polls(PollId)
                );

                -- Tạo bảng Votes để lưu trữ các lượt bình chọn cá nhân
                CREATE TABLE IF NOT EXISTS Votes (
                    VoteId INT PRIMARY KEY AUTO_INCREMENT,
                    PollId INT NOT NULL,
                    OptionId INT NOT NULL,
                    UserId INT NOT NULL, -- Liên kết lượt bình chọn với một người dùng cụ thể
                    VotedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (PollId) REFERENCES Polls(PollId),
                    FOREIGN KEY (OptionId) REFERENCES Options(OptionId),
                    FOREIGN KEY (UserId) REFERENCES Users(UserId)
                );
            ";

            try
            {
                // Đảm bảo lớp 'Connection' và phương thức 'GetConnection()' được triển khai đúng cách
                // để cung cấp một thể hiện MySqlConnection hợp lệ.
                using (MySqlConnection connection = Connection.GetConnection())
                {
                    if (connection != null)
                    {
                        connection.Open();
                        MySqlCommand command = new MySqlCommand(sqlScript, connection);
                        command.ExecuteNonQuery();
                        //MessageBox.Show("Các bảng cơ sở dữ liệu đã được tạo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể thiết lập kết nối cơ sở dữ liệu. Vui lòng kiểm tra chuỗi kết nối của bạn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu việc tạo bảng thất bại
                MessageBox.Show("Lỗi khi tạo các bảng cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
