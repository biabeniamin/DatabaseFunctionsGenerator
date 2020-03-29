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

        public string ToString(string delimitator, bool withLowerCase = false)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            foreach (Column column in _columns)
            {
                if(withLowerCase)
                    builder.Append($"{column.LowerCaseName}{delimitator}");
                else
                    builder.Append($"{column.Name}{delimitator}");
            }

            if (builder.ToString().Contains(delimitator))
            {
                builder.Remove(builder.ToString().LastIndexOf(delimitator), delimitator.Length);
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return ToString(" ");
        }

    }
}
