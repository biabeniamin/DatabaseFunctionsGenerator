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
            string httpRequestClient;

            builder = new StringBuilder();
            httpRequestClient = Helpers.ReadFile("CodeHelpers\\HttpRequestClient.cs");

            Helpers.WriteFile($"{path}\\Controllers\\HttpRequestClient.cs", httpRequestClient);
        }
    }
}
