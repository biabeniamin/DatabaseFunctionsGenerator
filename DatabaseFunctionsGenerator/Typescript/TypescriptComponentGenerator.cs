﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptComponentGenerator
    {
        private Database _database;

        public TypescriptComponentGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateAddEventHandler(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();

            builder.AppendLine($"add{table.SingularName}(event)");
            builder.AppendLine("{");
            {
                functionBody.AppendLine("event.preventDefault();");
                functionBody.AppendLine("const target = event.target;");
                functionBody.AppendLine($"let {table.LowerCaseSingularName} = {table.Name}.GetDefaultAccessLog();");
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
                functionBody.AppendLine($"this.{table.LowerCaseSingularName}Controller.Add{table.SingularName}({table.LowerCaseSingularName});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateDropDownChangeEventHandler(Table table)
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


        private string GenerateViewForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"import {{ {table.Name} }} from '../app/{table.Name}'");
            builder.AppendLine($"{table.LowerCaseSingularName}Controller : {table.Name};");

            builder.AppendLine(GenerateAddEventHandler(table));

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine(GenerateDropDownChangeEventHandler(parentTable));
            }

            Helpers.WriteFile($"{path}\\{table.SingularName}.component.ts",
                builder.ToString());

            return builder.ToString();
        }

        public void Generate(string path)
        {
            foreach (Table table in _database.Tables)
            {
                GenerateViewForTable(path, table);
            }
        }
    }
}
