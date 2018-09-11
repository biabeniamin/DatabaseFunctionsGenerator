using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class NotificationSystem
    {
        public Table GenerateNotificationTable()
        {
            Table table;
            List<Column> columns;

            columns = new List<Column>();

            columns.Add(new Column("Title", new ColumnType(Types.Varchar, 15)));

            table = new Table("Notifications", columns);

            return table;
        }
    }
}
