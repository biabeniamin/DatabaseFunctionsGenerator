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
            :this(type, 0)
        {

        }

        public ColumnType(Types type, int length)
        {
            _type = type;
            _length = length;
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}
