using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    class AuthenticationSystem
    {
        public Table GenerateAuthenticationTable()
        {
            Table table;
            List<Column> columns;

            columns = new List<Column>();

            columns.Add(new Column("Value", new ColumnType(Types.Varchar, 40)));
            columns.Add(new Column("Address", new ColumnType(Types.Varchar, 15)));
            columns.Add(new Column("LastUpdate", new ColumnType(Types.Varchar, 15)));

            table = new Table("Notifications", columns);

            return table;
        }
    }
}
