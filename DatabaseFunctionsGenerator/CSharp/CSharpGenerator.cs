using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpGenerator : IGenerator
    {
        private Database _database;

        private CSharpModelsGenerator _cSharpModelsGenerator;
        private CSharpClientControllerGenerator _cSharpClientControllerGenerator;
        private CSharpHelpersGenerator _cSharpHelpersGenerator;

        public CSharpGenerator(Database database)
        {
            _database = database;

            _cSharpModelsGenerator = new CSharpModelsGenerator(_database);
            _cSharpClientControllerGenerator = new CSharpClientControllerGenerator(_database);
            _cSharpHelpersGenerator = new CSharpHelpersGenerator(_database);
        }

        public void Generate(string path)
        {
            string cSharpPath;
            string cSharpClientPath;
            string cSharpServerPath;

            cSharpPath = $"{path}\\C#";
            cSharpClientPath = $"{cSharpPath}\\Client";
            cSharpServerPath = $"{cSharpPath}\\Server";

            Directory.CreateDirectory(cSharpClientPath);
            Directory.CreateDirectory(cSharpServerPath);

            _cSharpModelsGenerator.Generate(cSharpPath);
            _cSharpClientControllerGenerator.Generate(cSharpClientPath);
            _cSharpHelpersGenerator.Generate(cSharpClientPath);

        }
    }
}
