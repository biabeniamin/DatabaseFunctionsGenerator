using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpGenerator : IGenerator
    {
        private Database _database;

        private CSharpModelsGenerator _cSharpModelsGenerator;
        private CSharpControllerGenerator _cSharpControllerGenerator;
        private CSharpHelpersGenerator _cSharpHelpersGenerator;

        public CSharpGenerator(Database database)
        {
            _database = database;

            _cSharpModelsGenerator = new CSharpModelsGenerator(_database);
            _cSharpControllerGenerator = new CSharpControllerGenerator(_database);
            _cSharpHelpersGenerator = new CSharpHelpersGenerator(_database);
        }

        public void Generate(string path)
        {
            string cSharpPath;

            cSharpPath = $"{path}\\C#";

            _cSharpModelsGenerator.Generate(cSharpPath);
            _cSharpControllerGenerator.Generate(cSharpPath);
            _cSharpHelpersGenerator.Generate(cSharpPath);

        }
    }
}
