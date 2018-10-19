using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    class TypescriptHelpersGenerator : IGenerator
    {
        private Database _database;
        public TypescriptHelpersGenerator(Database database)
        {
            _database = database;
        }
        public void Generate(string path)
        {
            StringBuilder builder;
            string serverUrl;
            string helpers;

            builder = new StringBuilder();
            serverUrl = Helpers.ReadFile("CodeHelpers\\ServerUrl.ts");

            //replace url
            serverUrl = serverUrl.Replace("!--url--!", _database.ServerUrl);

            Helpers.WriteFile($"{path}\\ServerUrl.ts", serverUrl);
        }
    }
}
