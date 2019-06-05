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

            columns.Add(new Column("Title", new ColumnType(Types.Varchar, 20)));
            columns.Add(new Column("Message", new ColumnType(Types.Text)));

            table = new Table("Notifications", columns);

            return table;
        }
    }
}
