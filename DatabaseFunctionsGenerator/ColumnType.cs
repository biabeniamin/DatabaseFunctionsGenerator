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

        public Types Type
        {
            get { return _type; }
            set { _type = value; }
        }

    }
}
