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

        private string GenerateRequestMenu(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("Helper");

            return builder.ToString();
        }

        public void Generate(string path)
        {
            StringBuilder requestMenu;

            requestMenu = new StringBuilder();

            foreach (Table table in _database.Tables)
            {
                requestMenu.AppendLine(GenerateRequestMenu(table));
            }

            Helpers.WriteFile($"{path}\\Requests.txt", requestMenu.ToString());
        }
    }
}
