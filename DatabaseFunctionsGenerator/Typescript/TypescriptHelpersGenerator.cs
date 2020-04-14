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
            string authentication;
            string webSockets;
            string helpersPath;

            builder = new StringBuilder();
            serverUrl = IO.ReadFile("CodeHelpers\\ServerUrl.ts");
            authentication = IO.ReadFile("CodeHelpers\\Authentication.ts");
            webSockets = IO.ReadFile("CodeHelpers\\WebSockets.ts");

            helpersPath = $"{path}\\Helpers";

            IO.CreateDirectory(helpersPath);

            //replace url
            serverUrl = serverUrl.Replace("!--url--!", _database.ServerUrl);
            webSockets = webSockets.Replace("!--url--!", _database.WebSocketsServerAddress);

            IO.WriteFile($"{helpersPath}\\ServerUrl.ts", serverUrl);
            IO.WriteFile($"{helpersPath}\\Authentication.ts", authentication);
            IO.WriteFile($"{helpersPath}\\WebSockets.ts", webSockets);
        }
    }
}
