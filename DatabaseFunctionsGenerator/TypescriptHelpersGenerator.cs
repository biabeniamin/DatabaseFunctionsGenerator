using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    class TypescriptHelpersGenerator
    {
        public string Generate(string path)
        {
            StringBuilder builder;
            string serverUrl;
            string helpers;

            builder = new StringBuilder();
            serverUrl = Helpers.ReadFile("ServerUrl.ts");

            Helpers.WriteFile($"{path}\\ServerUrl.ts", serverUrl);

            return builder.ToString();
        }
    }
}
