using DatabaseFunctionsGenerator.Typescript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptControllerComponentGenerator
    {
        private static string GenerateAddEventHandler(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();

            builder.AppendLine($"add{table.SingularName}(event)");
            builder.AppendLine("{");
            {
                functionBody.AppendLine("event.preventDefault();");
                functionBody.AppendLine("const target = event.target;");
                functionBody.AppendLine($"let {table.LowerCaseSingularName} = {table.SingularName}Service.GetDefault{table.SingularName}();");
                foreach (Column column in table.EditableColumns)
                {
                    functionBody.Append($"{table.LowerCaseSingularName}.{Helpers.GetLowerCaseString(column.Name)} = target.querySelector('");
                    if (column.Type.IsForeignKey)
                    {
                        functionBody.Append($"#{column.Name}DropDown");
                    }
                    else
                    {
                        functionBody.Append($"#{column.Name}");
                    }

                    functionBody.AppendLine($"').value;");
                }

                foreach (Table parentTable in table.Parents)
                {
                    foreach (Column column in parentTable.Columns)
                    {
                        /* tableBody.AppendLine("\t<td>");
                         {
                             tableBody.AppendLine($"\t\t{{{{{table.LowerCaseSingularName}.{parentTable.LowerCaseSingularName}.{Helpers.GetLowerCaseString(column.Name)}}}}}");
                         }
                         tableBody.AppendLine("\t</td>");*/
                    }
                }

                functionBody.AppendLine($"console.log({table.LowerCaseSingularName});");
                functionBody.AppendLine($"this.{table.LowerCaseSingularName}Service.Add{table.SingularName}({table.LowerCaseSingularName});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GenerateDropDownChangeEventHandler(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();

            builder.AppendLine($"{table.LowerCaseSingularName}Changed(event)");
            builder.AppendLine("{");
            {
                functionBody.AppendLine($"console.log(event);");

                builder.AppendLine(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }


        public static string GenerateControllerComponentForTable(Table table, string path)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("import { Component, OnInit } from '@angular/core';");
            builder.AppendLine($"import {{ {table.Name} }} from '../app/{table.Name}'");
            builder.AppendLine($"import {{ {table.SingularName}Service }} from './{table.SingularName}Service'");
            builder.AppendLine();

            //generate component
            builder.AppendLine("@Component({");
            {
                builder.AppendLine($"selector: 'app-{table.LowerCaseSingularName}',");
                builder.AppendLine($"templateUrl: './{table.LowerCaseSingularName}.component.html',");
                builder.AppendLine($"styleUrls: ['./{table.LowerCaseSingularName}.component.css']");
            }
            builder.AppendLine("})");

            builder.AppendLine(GenerateAddEventHandler(table));

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine(GenerateDropDownChangeEventHandler(parentTable));
            }

            Helpers.WriteFile($"{path}\\{table.SingularName}.component.ts",
                builder.ToString());

            return builder.ToString();
        }

    }
}
