using MySql.Data.MySqlClient;
using System.Windows.Forms; // Cần thiết cho MessageBox.Show

namespace ElectorApp.Utils
{
    internal class Connection
    {
        // Phương thức này trả về một đối tượng MySqlConnection
        public static MySqlConnection GetConnection()
        {
            // Chuỗi kết nối đến cơ sở dữ liệu MySQL của XAMPP
            // Thay đổi 'electorapp' nếu tên cơ sở dữ liệu của bạn khác
            // Mặc định XAMPP sử dụng 'root' và không có mật khẩu
            string connectionString = "Server=localhost;Port=3306;Database=electorapp;Uid=root;Pwd=;";

            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                return connection;
            }
            catch (MySqlException ex)
            {
                // Hiển thị lỗi nếu không thể tạo đối tượng kết nối
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
