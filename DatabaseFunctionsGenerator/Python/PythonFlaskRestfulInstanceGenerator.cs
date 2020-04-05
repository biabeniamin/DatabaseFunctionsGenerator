using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonFlaskRestfulInstanceGenerator : IGenerator
    {
        private Database _database;

        public PythonFlaskRestfulInstanceGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");

            IO.WriteFile($"{path}\\FlaskRestful.py", (builder.ToString()));
        }
    }
}
