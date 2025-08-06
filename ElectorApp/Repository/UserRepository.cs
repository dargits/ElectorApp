using ElectorApp.Utils;
using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace ElectorApp.Repository
{
    internal class UserRepository
    {
        public UserRepository() { }

        // Kiểu trả về đã được sửa thành User
        public User Login(string username, string password)
        {
            // Mã hóa mật khẩu người dùng nhập vào để so sánh với mật khẩu đã lưu trong DB
            string passwordHash = PasswordHasher.HashPassword(password);

            string query = "SELECT UserId, Username, PasswordHash, IsAdmin FROM Users WHERE Username = @username;";

            try
            {
                using (MySqlConnection connection = Connection.GetConnection())
                {
                    if (connection == null) return null;

                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Gán giá trị của tham số @username
                        command.Parameters.AddWithValue("@username", username);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Lấy mật khẩu đã mã hóa từ database
                                string dbPasswordHash = reader["PasswordHash"].ToString();

                                // So sánh mật khẩu đã mã hóa
                                if (dbPasswordHash == passwordHash)
                                {
                                    // Nếu khớp, tạo đối tượng User và trả về
                                    return new User
                                    {
                                        UserId = Convert.ToInt32(reader["UserId"]),
                                        Username = reader["Username"].ToString(),
                                        IsAdmin = Convert.ToBoolean(reader["IsAdmin"])
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Xử lý lỗi kết nối/truy vấn, ví dụ hiển thị một hộp thoại
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null; // Trả về null nếu có lỗi hoặc đăng nhập thất bại
        }
    }
}