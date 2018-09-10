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

        public bool HasPrimaryKey
        {
            get
            {
                return 0 < _columns.Where((column) => {
                    return true == column.Type.IsPrimaryKey;
                }).Count();
            }
        }

        public Column PrimaryKeyColumn
        {
            get
            {
                IEnumerable<Column> columns;

                columns = _columns.Where((column) => {
                    return true == column.Type.IsPrimaryKey;
                });

                if (0 == columns.Count())
                {
                    return null;
                }

                return columns.ElementAt(0);
            }
        }

        public IEnumerable<Column> PrimaryKeyColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return true == column.Type.IsPrimaryKey;
                });
            }
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
