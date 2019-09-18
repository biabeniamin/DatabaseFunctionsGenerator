using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonControllerGenerator : IGenerator
    {
        private Database _database;

        public PythonControllerGenerator(Database database)
        {
            _database = database;
        }

        private void GenerateController(Table table, string path)
        {
            StringBuilder builder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            classBuilder = new StringBuilder();

            //class content
            {
                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                     1));
            }

            builder.AppendLine("#generated automatically");

            IO.WriteFile($"{path}\\{table.Name}.py", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string controllersPath;

            controllersPath = $"{path}\\Controllers";

            IO.CreateDirectory(controllersPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, controllersPath);
            }
        }
    }
}
