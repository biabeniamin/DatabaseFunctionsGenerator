using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class ColumnType
    {
        private Types _type;
        private int _length;
        private bool _isPrimaryKey;

        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }


        public Types Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public ColumnType(Types type)
            : this(type, 0)
        {

        }

        public ColumnType(Types type, bool isPrimaryKey)
            : this(type, 0, isPrimaryKey)
        {

        }

        public ColumnType(Types type, int length)
            : this(type, length, false)
        {
        }

        public ColumnType(Types type, int length, bool isPrimaryKey)
        {
            _type = type;
            _length = length;
            _isPrimaryKey = isPrimaryKey;
        }

        public string GetMysqlType()
        {
            switch(Type)
            {
                case Types.Date:
                    return "DATETIME";
                case Types.Integer:
                    return "INT";
                case Types.Text:
                    return "TEXT";
                case Types.Varchar:
                    return $"VARCHAR({_length})";
                    break;
            }

            return "NOT_EXISTING";
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}
