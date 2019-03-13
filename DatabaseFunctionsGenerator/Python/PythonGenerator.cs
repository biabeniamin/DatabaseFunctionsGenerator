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

        private PythonModelsGenerator _pythonModelsGenerator;

        public PythonGenerator(Database database)
        {
            _database = database;

            _pythonModelsGenerator = new PythonModelsGenerator(_database);
        }

        public void Generate(string path)
        {
            string pythonPath;

            pythonPath = $"{path}\\Python";
            IO.CreateDirectory(pythonPath);

            _pythonModelsGenerator.Generate(pythonPath);
        }
    }
}
