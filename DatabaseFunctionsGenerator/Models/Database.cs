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
        private string _serverUrl = "http:/192.168.0.100/generator/test/";
        private string _javaPackageName= "com.example.biabe.testretrofitwatch";

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
    }
}
