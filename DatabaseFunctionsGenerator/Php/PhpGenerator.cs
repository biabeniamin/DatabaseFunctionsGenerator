using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class PhpGenerator : IGenerator
    {
        private Database _database;

        private PhpHelpersGenerator _phpHelpersGenerator;
        private PhpModelsGenerator _phpModelsGenerator;
        private PhpDatabaseFunctionGenerator _phpDatabaseFunctionGenerator;

        public PhpGenerator(Database database)
        {
            _database = database;

            _phpModelsGenerator = new PhpModelsGenerator(_database);
            _phpHelpersGenerator = new PhpHelpersGenerator();
            _phpDatabaseFunctionGenerator = new PhpDatabaseFunctionGenerator(_database);
        }

        public void Generate(string path)
        {
            _phpDatabaseFunctionGenerator.Generate(path);
            _phpModelsGenerator.Generate(path);
            _phpHelpersGenerator.Generate(path);
        }
    }
}
