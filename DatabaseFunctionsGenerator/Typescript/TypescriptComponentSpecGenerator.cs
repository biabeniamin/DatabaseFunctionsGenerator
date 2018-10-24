using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Typescript
{
    public class TypescriptComponentSpecGenerator
    {
        public static void GenerateComponentSpec(Table table, string path)
        {
            string componentSpecTemplate;

            componentSpecTemplate = Helpers.ReadFile("CodeHelpers\\TypescriptComponentSpec.ts");

            //replace component name
            componentSpecTemplate = componentSpecTemplate.Replace("!--ComponentName--!", table.SingularName);

            //replace component file name
            componentSpecTemplate = componentSpecTemplate.Replace("!--ComponentFileName--!", table.LowerCaseSingularName);

            Helpers.WriteFile($"{path}\\{table.LowerCaseSingularName}.component.spec.ts",
               componentSpecTemplate);
        }
    }
}
