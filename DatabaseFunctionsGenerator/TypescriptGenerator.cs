using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptGenerator : IGenerator
    {
        private Database _database;

        private TypescriptModelsGenerator _typescriptModelsGenerator;
        private TypescriptGetDataGenerator _typescriptGetDataGenerator;
        private TypescriptHelpersGenerator _typescriptHelpersGenerator;
        private TypescriptViewGenerator _typescriptViewGenerator;

        public TypescriptGenerator(Database database)
        {
            _database = database;

            _typescriptModelsGenerator = new TypescriptModelsGenerator(_database);
            _typescriptGetDataGenerator = new TypescriptGetDataGenerator(_database);
            _typescriptHelpersGenerator = new TypescriptHelpersGenerator();
            _typescriptViewGenerator = new TypescriptViewGenerator(_database);
        }

        public void Generate(string path)
        {
            string typescriptPath;

            typescriptPath = $"{path}\\Typescript";

            _typescriptModelsGenerator.Generate(typescriptPath);
            _typescriptGetDataGenerator.Generate(typescriptPath);
            _typescriptHelpersGenerator.Generate(typescriptPath);
            _typescriptViewGenerator.Generate(typescriptPath);
        }
    }
}
