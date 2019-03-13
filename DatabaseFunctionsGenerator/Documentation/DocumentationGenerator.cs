using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class DocumentationGenerator : IGenerator
    {
        private Database _database;

        private RequestTextMenuGenerator _requestTextMenuGenerator;

        public DocumentationGenerator(Database database)
        {
            _database = database;

            _requestTextMenuGenerator = new RequestTextMenuGenerator(_database);
        }

        public void Generate(string path)
        {
            string documentationPath;

            documentationPath = $"{path}\\Documentation";

            Helpers.CreateDirectory(documentationPath);

            _requestTextMenuGenerator.Generate(documentationPath);


        }
    }
}
