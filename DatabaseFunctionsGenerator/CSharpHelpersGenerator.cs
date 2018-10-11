using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpHelpersGenerator : IGenerator
    {
        public void Generate(string path)
        {
            StringBuilder builder;
            string serverUrl;
            string helpers;

            builder = new StringBuilder();
            serverUrl = Helpers.ReadFile("CodeHelpers\\HttpClient.ts");

            Helpers.WriteFile($"{path}\\ServerUrl.ts", serverUrl);
        }
    }
}
