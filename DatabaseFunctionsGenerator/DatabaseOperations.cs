using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class DatabaseOperations
    {
        private MySqlConnection _connection;
        private MySqlCommand _command;

        public DatabaseOperations()
        {
            string server = Constants.MysqlServer;
            string database = Constants.MysqlDatabase;
            string uid = "root";
            string password = "";
            string connectionString;
            connectionString = $"SERVER={Constants.MysqlServer};DATABASE={Constants.MysqlDatabase};UID={Constants.MysqlUsername};PASSWORD={Constants.MysqlPassword};";

            _connection = new MySqlConnection(connectionString);
            _connection.Open();

            //_command = new MySqlCommand("select * from users", _connection);

            //_reader = _command.ExecuteReader();
        }
    }
}
