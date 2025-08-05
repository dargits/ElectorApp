using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace ElectorApp.Utils
{
    public class Connection
    {
        // Db từ railway
        private const string connectionString = "server=gondola.proxy.rlwy.net;port=43240;user id=root;password=uXPqzHtzyrROSlVUQxYqEOfChpboWkRl;database=railway;";

        public static MySqlConnection GetConnection()
        {
            try
            {
                return new MySqlConnection(connectionString);
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}