using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectorApp.Utils
{
    internal class CreateDb
    {
        public static void create()
        {
            // Các câu lệnh SQL để tạo các bảng
            string sqlScript = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INT PRIMARY KEY AUTO_INCREMENT,
                    Username VARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash VARCHAR(255) NOT NULL,
                    IsAdmin BOOLEAN DEFAULT FALSE,
                    FullName VARCHAR(100) NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );
                -- ... (Các câu lệnh SQL khác) ...
            ";

            try
            {
                using (MySqlConnection connection = Connection.GetConnection())
                {
                    if (connection != null)
                    {
                        connection.Open();
                        MySqlCommand command = new MySqlCommand(sqlScript, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình tạo cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
