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

    }
}
