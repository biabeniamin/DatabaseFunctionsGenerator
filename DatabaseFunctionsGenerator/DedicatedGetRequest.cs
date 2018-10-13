using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class DedicatedGetRequest
    {
        private List<Column> _columns;

        public List<Column> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public DedicatedGetRequest()
        {
            _columns = new List<Column>();
        }

        public DedicatedGetRequest(Column column)
            : this()
        {
            _columns.Add(column);
        }

        public DedicatedGetRequest(Column column1, Column column2)
            : this(column1)
        {
            _columns.Add(column2);
        }

        public override string ToString()
        {
            StringBuilder builder;

            builder = new StringBuilder();

            foreach(Column column in _columns)
            {
                builder.Append($"{column.Name} ");
            }

            return builder.ToString();
        }
    }
}
