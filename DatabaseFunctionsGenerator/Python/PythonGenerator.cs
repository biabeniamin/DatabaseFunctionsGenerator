using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    public class PythonGenerator : IGenerator
    {
        private Database _database;

        public PythonGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            string pythonPath;

            pythonPath = $"{path}\\Python";
            Directory.CreateDirectory(pythonPath);
        }
    }
}
