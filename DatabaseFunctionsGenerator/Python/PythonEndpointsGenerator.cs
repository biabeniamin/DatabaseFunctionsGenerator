using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonEndpointsGenerator : IGenerator
    {
        private Database _database;

        public PythonEndpointsGenerator(Database database)
        {
            _database = database;
        }


        private void GenerateController(Table table, string path)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");

            IO.WriteFile($"{path}\\{table.SingularName}Endpoints.py", (builder.ToString()));
        }

        public void Generate(string path)
        {
            string endpointsPath;

            endpointsPath = $"{path}";

            IO.CreateDirectory(endpointsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, endpointsPath);
            }
        }
    }
}

