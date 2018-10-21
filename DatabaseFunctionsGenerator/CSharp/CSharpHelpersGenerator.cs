using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpHelpersGenerator : IGenerator
    {
        private Database _database;

        public CSharpHelpersGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder;
            string httpRequestClient;

            builder = new StringBuilder();
            httpRequestClient = Helpers.ReadFile("CodeHelpers\\HttpRequestClient.cs");

            //replace url
            httpRequestClient = httpRequestClient.Replace("!--url--!", _database.ServerUrl);

            Helpers.WriteFile($"{path}\\Controllers\\HttpRequestClient.cs", httpRequestClient);
        }
    }
}
