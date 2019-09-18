using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Column
    {
        private string _name;
        private ColumnType _type;
        private Table _parentTable;

        public Table ParentTable
        {
            get { return _parentTable; }
            set { _parentTable = value; }
        }

        public ColumnType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [JsonIgnore]
        public string LowerCaseName
        {
            get
            {
                return Helpers.GetLowerCaseString(_name);
            }
        }

        [JsonIgnore]
        public bool IsCreationTimeColumn
        {
            get
            {
                return String.Equals("CreationTime", Name) && Types.DateTime == Type.Type;
            }
        }

        public Column(string name, ColumnType type)
        {
            _name = name;
            _type = type;
        }

        public Column(string name, ColumnType type, Table parentTable)
            :this(name, type)
        {
            _parentTable = parentTable;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
