using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Generator
    {
        private Database _database;

        private SqlGenerator _sqlGenerator;
        private PhpHelpersGenerator _phpHelpersGenerator;
        private PhpModelsGenerator _phpModelsGenerator;

        public Generator(Database database)
        {
            _database = database;

            _sqlGenerator = new SqlGenerator(_database);
            _phpModelsGenerator = new PhpModelsGenerator(_database);
            _phpHelpersGenerator = new PhpHelpersGenerator();
        }

        public void Generate()
        {
            string path;

            path = $"GeneratorResult\\{Helpers.GenerateTimeStamp()}";

            if (!Directory.Exists("GeneratorResult"))
            {
                Directory.CreateDirectory("GeneratorResult");
            }

            Directory.CreateDirectory(path);

            _sqlGenerator.Generate(path);

            _phpModelsGenerator.Generate(path);
            _phpHelpersGenerator.Generate(path);
        }
    }
}
