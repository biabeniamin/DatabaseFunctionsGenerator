using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Typescript
{
    public class TypescriptComponentStyleGenerator
    {
        
        private static string GenerateStyle(Table table)
        {
            StringBuilder builder = new StringBuilder();

            return builder.ToString();
        }


        public static string GenerateStyleForTable(Table table, string path)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(GenerateStyle(table));

            Helpers.WriteFile($"{path}\\{table.LowerCaseSingularName}.component.css",
                builder.ToString());

            return builder.ToString();
        }
    }
}
