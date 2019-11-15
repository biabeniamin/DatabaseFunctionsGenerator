using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    class AuthenticationSystem
    {
        private Database _database;

        public AuthenticationSystem(Database database)
        {
            _database = database;
        }

        private Table GenerateTokenTable()
        {
            Table table;
            List<Column> columns;

            columns = new List<Column>();

            columns.Add(new Column("Value", new ColumnType(Types.Varchar, 40)));
            columns.Add(new Column("Address", new ColumnType(Types.Varchar, 15)));
            columns.Add(new Column("LastUpdate", new ColumnType(Types.Varchar, 15)));

            table = new Table("Tokens", columns);

            //add dedicated request to check token
            table.DedicatedGetRequests.Add(new DedicatedGetRequest(columns[0]));

            return table;
        }

        private Table GenerateTokenUsersTable()
        {
            Table table;
            List<Column> columns;

            columns = new List<Column>();

            columns.Add(new Column("Username", new ColumnType(Types.Varchar, 40)));
            columns.Add(new Column("Password", new ColumnType(Types.Varchar, 40)));

            table = new Table("TokenUsers", columns);

            //add dedicated request to check token
            table.DedicatedGetRequests.Add(new DedicatedGetRequest(columns[0]));

            return table;
        }

        public Table[] GenerateAuthenticationTables()
        {
            Table tokenUsersTable = GenerateTokenUsersTable();
            Table tokenTable = GenerateTokenTable();

            _database.Relations.Add(new Relation(tokenUsersTable, tokenTable, RelationType.OneToMany));

            return new Table[] { tokenTable, tokenUsersTable };
        }
    }
}
