using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    class JavaGenerator : IGenerator
    {
        private Database _database;

        public JavaGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            string javaPath;

            javaPath = $"{path}\\C#";

        }
    }
}
