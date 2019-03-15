using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class DatabaseOperations
    {
        private MySqlConnection _connection;
        private MySqlCommand _command;
        private MySqlDataReader _reader;
        private MySqlDataAdapter _adapter;

        public DatabaseOperations()
        {
            string connectionString = $"SERVER={Constants.MysqlServer};DATABASE={Constants.MysqlDatabase};UID={Constants.MysqlUsername};PASSWORD={Constants.MysqlPassword};";

            _connection = new MySqlConnection(connectionString);
            _connection.Open();

            //_command = new MySqlCommand("select * from users", _connection);

            //_reader = _command.ExecuteReader();
        }

        public MySqlDataReader GetReader(string query)
        {
            if (ConnectionState.Open != _connection.State)
            {
                return null;
            }

            _command = new MySqlCommand(query, _connection);
            _reader = _command.ExecuteReader();
            return _reader;
        }
        public type ReadOneValue<type>(string query)
        {
            type value = default(type);
            GetReader(query);
            if (_reader.Read())
            {
                value = (type)_reader[0];
            }
            _reader.Dispose();
            return value;
        }
        public void ExecuteQuery(string query)
        {
            if (ConnectionState.Open != _connection.State)
            {
                return;
            }
            _command = new MySqlCommand(query, _connection);
            _command.ExecuteNonQuery();
        }
        public MySqlDataAdapter GetAdapter(string query)
        {
            _command = new MySqlCommand(query, _connection);
            _adapter = new MySqlDataAdapter(_command);
            return _adapter;
        }
    }
}
