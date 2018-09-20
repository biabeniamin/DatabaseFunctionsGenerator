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

        public TypescriptGenerator(Database database)
        {
            _database = database;

            _typescriptModelsGenerator = new TypescriptModelsGenerator(_database);
            _typescriptGetDataGenerator = new TypescriptGetDataGenerator(_database);
        }

        public void Generate(string path)
        {
            _typescriptModelsGenerator.Generate(path);
            _typescriptGetDataGenerator.Generate(path);
        }
    }
}
