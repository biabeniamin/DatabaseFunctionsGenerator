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
        private JavaModelsGenerator _javaModelsGenerator;

        public JavaGenerator(Database database)
        {
            _database = database;

            _javaModelsGenerator = new JavaModelsGenerator(_database);
        }

        public void Generate(string path)
        {
            string javaPath;

            javaPath = $"{path}\\Java";

            _javaModelsGenerator.Generate(javaPath);
        }
    }
}
