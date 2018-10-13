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
        private string _singularName;
        private ObservableCollection<Table> _parents;
        private ObservableCollection<Table> _childs;
        private List<DedicatedGetRequest> _dedicatedGetRequests;

        public List<DedicatedGetRequest> DedicatedGetRequests
        {
            get { return DedicatedGetRequests; }
            set { DedicatedGetRequests = value; }
        }


        public ObservableCollection<Table> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        public ObservableCollection<Table> Parents
        {
            get { return _parents; }
            set { _parents = value; }
        }


        

        public ObservableCollection<Column> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _singularName = Helpers.GetSingular(_name);
            }
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

        public Column CreationTimeColumn
        {
            get
            {
                IEnumerable<Column> columns;

                columns = _columns.Where((column) => {
                    return String.Equals(column.Name, "CreationTime");
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

        public IEnumerable<Column> ForeignKeyColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return true == column.Type.IsForeignKey;
                });
            }
        }

        public IEnumerable<Column> EditableColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return (false == column.Type.IsPrimaryKey) && (false == column.IsCreationTimeColumn);
                });
            }
        }
        public IEnumerable<Column> NonEditableColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return (true == column.Type.IsPrimaryKey) || (true == column.IsCreationTimeColumn);
                });
            }
        }

        public bool HasCreationTime
        {
            get
            {
                return 0 < _columns.Where((column) =>
                {
                    return String.Equals(column.Name, "CreationTime");
                }).Count();
            }
        }

        public string LowerCaseName
        {
            get
            {
                return Helpers.GetLowerCaseString(Name);
            }
        }

        public string SingularName
        {
            get
            {
                if (String.IsNullOrEmpty(_singularName))
                {
                    _singularName = Helpers.GetSingular(_name);
                }
                return _singularName;
            }
            set { _singularName = value; }
        }

        public string LowerCaseSingularName
        {
            get
            {
                return Helpers.GetLowerCaseString(Helpers.GetSingular(_name));
            }
            set { _singularName = value; }
        }

        public bool HasParent
        {
            get
            {
                if (null != _parents)
                    return true;
                return false;
            }
        }

        public Table(string name)
            :this(name, new ObservableCollection<Column>())
        {

        }

        public Table(string name, IEnumerable<Column> columns)
        {
            _name = name;
            _columns = new ObservableCollection<Column>(columns);
            _parents = new ObservableCollection<Table>();
            _childs = new ObservableCollection<Table>();
            _dedicatedGetRequests = new List<DedicatedGetRequest>();
        }

        

        public override string ToString()
        {
            return Name;
        }
    }
}
