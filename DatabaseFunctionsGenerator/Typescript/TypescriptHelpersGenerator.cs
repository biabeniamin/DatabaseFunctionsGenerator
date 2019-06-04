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
            string helpersPath;

            builder = new StringBuilder();
            serverUrl = IO.ReadFile("CodeHelpers\\ServerUrl.ts");
            helpersPath = $"{path}\\Helpers";

            IO.CreateDirectory(helpersPath);


            //replace url
            serverUrl = serverUrl.Replace("!--url--!", _database.ServerUrl);

            IO.WriteFile($"{helpersPath}\\ServerUrl.ts", serverUrl);
        }
    }
}
