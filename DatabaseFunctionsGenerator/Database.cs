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
        }
    }
}
