using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    class RequestTextMenuGenerator : IGenerator
    {
        private Database _database;

        public RequestTextMenuGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateRequestMenu(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine($"------------{table.Name}----------");
            builder.AppendLine($"{serverUrl}{table.Name}.php?cmd=get{table.Name} - GET request -return all {table.Name}");
            builder.AppendLine($"{serverUrl}{table.Name}.php?cmd=get{table.SingularName}ById=value - GET request -return one {table.SingularName} with specified id");

            return builder.ToString();
        }

        public void Generate(string path)
        {
            StringBuilder requestMenu;

            requestMenu = new StringBuilder();

            foreach (Table table in _database.Tables)
            {
                requestMenu.AppendLine(GenerateRequestMenu(table, _database.ServerUrl));
            }

            Helpers.WriteFile($"{path}\\Requests.txt", requestMenu.ToString());
        }
    }
}
