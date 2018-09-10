using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Table
    {
        private string _name;
        private ObservableCollection<Column> _columns;

        public ObservableCollection<Column> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Table(string name)
            :this(name, new ObservableCollection<Column>())
        {

        }

        public Table(string name, ObservableCollection<Column> columns)
        {
            _name = name;
            _columns = columns;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
