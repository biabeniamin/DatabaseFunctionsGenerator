using DatabaseFunctionsGenerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Database
    {
        private ObservableCollection<Table> _tables;
        private ObservableCollection<Relation> _relations;
        private string _serverUrl = "http://192.168.0.100/messages";
        private string _javaPackageName= "com.example.biabe.DatabaseFunctionsGenerator";
        private string _databaseName = "gen";
        private DatabaseType _type = DatabaseType.Php;

        public DatabaseType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }


        public string JavaPackageName
        {
            get { return _javaPackageName; }
            set { _javaPackageName = value; }
        }

        public string ServerUrl
        {
            get { return _serverUrl; }
            set { _serverUrl = value; }
        }


        public ObservableCollection<Relation> Relations
        {
            get { return _relations; }
            set { _relations = value; }
        }

        public ObservableCollection<Table> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        [JsonIgnore]
        public bool HasNotificationSystem
        {
            get
            {
                return 0 < _tables.Where((table) => {
                    return String.Equals("Notifications", table.Name);
                }). Count();
            }
        }

        public Database()
            :this(new ObservableCollection<Table>())
        {

        }

        public Database(ObservableCollection<Table> tables)
        {
            _tables = tables;
            _relations = new ObservableCollection<Relation>();
        }

        public static Database ImportFromJson(string json)
        {
            Database database;

            database = new Database();

            database = JsonConvert.DeserializeObject<Database>(json);


            return database;
        }
    }
}
