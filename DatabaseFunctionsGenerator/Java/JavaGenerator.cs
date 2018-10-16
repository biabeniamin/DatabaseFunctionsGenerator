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
        private JavaControllerGenerator _javaControllerGenerator;
        private JavaHelpersGenerator _javaHelpersGenerator;

        public JavaGenerator(Database database)
        {
            _database = database;

            _javaModelsGenerator = new JavaModelsGenerator(_database);
            _javaControllerGenerator = new JavaControllerGenerator(_database);
            _javaHelpersGenerator = new JavaHelpersGenerator(_database);
        }

        public void Generate(string path)
        {
            string javaPath;

            javaPath = $"{path}\\Java";

            _javaModelsGenerator.Generate(javaPath);
            _javaControllerGenerator.Generate(javaPath);
            _javaHelpersGenerator.Generate(javaPath);
        }
    }
}
