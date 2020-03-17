using Newtonsoft.Json;
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
        private bool _requiresSecurityToken;

        public bool RequiresSecurityToken
        {
            get { return _requiresSecurityToken; }
            set { _requiresSecurityToken = value; }
        }


        public List<DedicatedGetRequest> DedicatedGetRequests
        {
            get { return _dedicatedGetRequests; }
            set { _dedicatedGetRequests = value; }
        }

        [JsonIgnore]
        public ObservableCollection<Table> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public bool HasPrimaryKey
        {
            get
            {
                return 0 < _columns.Where((column) => {
                    return true == column.Type.IsPrimaryKey;
                }).Count();
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore]
        public IEnumerable<Column> PrimaryKeyColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return true == column.Type.IsPrimaryKey;
                });
            }
        }

        [JsonIgnore]
        public IEnumerable<Column> ForeignKeyColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return true == column.Type.IsForeignKey;
                });
            }
        }

        [JsonIgnore]
        public IEnumerable<Column> EditableColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return (false == column.Type.IsPrimaryKey) && (false == column.IsCreationTimeColumn) && (Types.GUID != column.Type.Type);
                });
            }
        }

        [JsonIgnore]
        public IEnumerable<Column> DataColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return (false == column.Type.IsPrimaryKey);
                });
            }
        }

        [JsonIgnore]
        public IEnumerable<Column> NonEditableColumns
        {
            get
            {
                return _columns.Where((column) => {
                    return (true == column.Type.IsPrimaryKey) || (true == column.IsCreationTimeColumn);
                });
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public bool HasDedicatedGetRequestForPrimaryKey
        {
            get
            {
                return 0 < _dedicatedGetRequests.Where((dedicated) =>
                {
                    return 1 == dedicated.Columns.Count()
                        && 0 < dedicated.Columns.Where((column) =>
                            {
                                return column.Type.IsPrimaryKey;
                            }).Count();
                }).Count();
            }
        }

        [JsonIgnore]
        public string LowerCaseName
        {
            get
            {
                return Helpers.GetLowerCaseString(Name);
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public string LowerCaseSingularName
        {
            get
            {
                return Helpers.GetLowerCaseString(Helpers.GetSingular(_name));
            }
        }

        [JsonIgnore]
        public bool HasParent
        {
            get
            {
                if (null != _parents)
                    return true;
                return false;
            }
        }

        [JsonIgnore]
        public DedicatedGetRequest GetDedicatedRequestById
        {
            get
            {
                IEnumerable<DedicatedGetRequest> requests;

                requests = _dedicatedGetRequests.Where((req) => {
                    if(1 != req.Columns.Count)
                    {
                        return false;
                    }

                    return PrimaryKeyColumn == req.Columns[0];
                });

                if (0 == requests.Count())
                {
                    return null;
                }

                return requests.ElementAt(0);
            }
        }

        public Table()
            :this(null)
        {

        }

        public Table(string name, string singularName = null)
            :this(name,  new ObservableCollection<Column>(), singularName)
        {

        }

        public Table(string name, IEnumerable<Column> columns, string singularName = null)
        {
            _name = name;
            _singularName = singularName;
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
