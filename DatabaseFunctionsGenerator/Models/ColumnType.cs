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
        private bool _autoIncrement;
        private bool _isForeignKey;
        private bool _isMandatory;

        public bool IsMandatory
        {
            get { return _isMandatory; }
            set { _isMandatory = value; }
        }


        public bool IsForeignKey
        {
            get { return _isForeignKey; }
            set { _isForeignKey = value; }
        }

        public bool AutoIncrement
        {
            get { return _autoIncrement; }
            set { _autoIncrement = value; }
        }

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

        public ColumnType()
            :this(Types.Text)
        {

        }
        
        public ColumnType(Types type)
            : this(type, 0)
        {

        }

        public ColumnType(Types type, bool isForeignKey, bool isMandatory)
            : this(type, 0, false, true, false, isMandatory)
        {
        }

        public ColumnType(Types type, bool isPrimaryKey, bool autoIncrement, bool isMandatory)
            : this(type, 0, isPrimaryKey, false, autoIncrement, isMandatory)
        {
            
        }

        public ColumnType(Types type, int length)
            : this(type, length, false, false, false, false)
        {
        }

        public ColumnType(Types type, int length, bool isMandatory)
            : this(type, length, false, false, false, isMandatory)
        {
        }

        public ColumnType(Types type, int length, bool isPrimaryKey, bool isForeignKey, bool autoIncrement, bool isMandatory)
        {
            _type = type;
            _length = length;
            _isPrimaryKey = isPrimaryKey;
            _autoIncrement = autoIncrement;
            _isForeignKey = isForeignKey;
            _isMandatory = isMandatory;
        }

        public string GetMysqlType()
        {
            switch (Type)
            {
                case Types.DateTime:
                    return "DATETIME";
                case Types.Integer:
                    return "INT";
                case Types.Text:
                    return "TEXT";
                case Types.Varchar:
                    return $"VARCHAR({_length})";
                case Types.Double:
                    return $"DOUBLE";
                case Types.GUID:
                    return $"VARCHAR(40)";
                    break;
            }

            return "NOT_EXISTING";
        }

        public string GetTypescriptType()
        {
            switch (Type)
            {
                case Types.DateTime:
                    return "string";
                case Types.Integer:
                    return "number";
                case Types.GUID:
                case Types.Varchar:
                case Types.Text:
                    return "string";
                case Types.Double:
                    return $"number";
                    break;
            }

            return "NOT_EXISTING";
        }

        public string GetCSharpType()
        {
            switch (Type)
            {
                case Types.DateTime:
                    return "DateTime";
                case Types.Integer:
                    return "int";
                case Types.Text:
                case Types.Varchar:
                    return "string";
                case Types.Double:
                    return "double";
                    break;
            }

            return "NOT_EXISTING";
        }

        public string GetJavaType()
        {
            switch (Type)
            {
                case Types.DateTime:
                    return "Date";
                case Types.Integer:
                    return "Integer ";
                case Types.Text:
                case Types.Varchar:
                    return "String";
                case Types.Double:
                    return "double";
                    break;
            }

            return "NOT_EXISTING";
        }

        public string GetPythonDataClassType()
        {
            switch (Type)
            {
                case Types.DateTime:
                    return "str";
                case Types.Integer:
                    return "int";
                case Types.Text:
                case Types.Varchar:
                case Types.Double:
                    return "str";
                    break;
            }

            return "NOT_EXISTING";
        }

        public string GetSqlAlchemyType()
        {
            switch (Type)
            {
                case Types.Varchar:
                    return $"String({Length})";
                case Types.GUID:
                    return $"String({40})";
                case Types.Integer:
                    return $"Integer";
                case Types.Text:
                    return $"Text";
                case Types.DateTime:
                    return $"DateTime";
                case Types.Double:
                    return $"Float";
            }
            return "NOT_EXISTING";
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}
