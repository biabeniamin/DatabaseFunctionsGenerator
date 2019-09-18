using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonFlaskInstanceGenerator : IGenerator
    {
        private Database _database;

        public PythonFlaskInstanceGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
        }
    }
}
