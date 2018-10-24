using DatabaseFunctionsGenerator.Typescript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptComponentGenerator
    {
        private Database _database;

        public TypescriptComponentGenerator(Database database)
        {
            _database = database;
        }

        private void GenerateComponentForTable(Table table, string path)
        {
            string tableComponentPath;

            tableComponentPath = $"{path}\\{table.LowerCaseSingularName}";

            Directory.CreateDirectory(tableComponentPath);

            TypescriptControllerComponentGenerator.GenerateControllerComponentForTable(table, tableComponentPath);
            TypescriptComponentSpecGenerator.GenerateComponentSpec(table, tableComponentPath);
        }

        public void Generate(string path)
        {
            foreach (Table table in _database.Tables)
            {
                GenerateComponentForTable(table, path);
            }
        }
    }
}
