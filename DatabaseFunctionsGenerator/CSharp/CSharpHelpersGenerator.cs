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
            httpRequestClient = IO.ReadFile("CodeHelpers\\HttpRequestClient.cs");

            //replace url
            httpRequestClient = httpRequestClient.Replace("!--url--!", _database.ServerUrl);

            IO.WriteFile($"{path}\\Controllers\\HttpRequestClient.cs", httpRequestClient);
        }
    }
}
