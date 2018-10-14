using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptComponentViewGenerator
    {
        private Database _database;

        public TypescriptComponentViewGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateListView(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();

            builder.AppendLine($"<tbody>");
            {
                //generate header of table
                tableBody.AppendLine($"<tr>");
                {
                    foreach (Column column in table.Columns)
                    {
                        tableBody.AppendLine("\t<th>");
                        {
                            tableBody.AppendLine($"\t\t{column.Name}");
                        }
                        tableBody.AppendLine("\t</th>");
                    }

                    foreach (Table parentTable in table.Parents)
                    {
                        foreach (Column column in parentTable.Columns)
                        {
                            tableBody.AppendLine("\t<th>");
                            {
                                tableBody.AppendLine($"\t\t{parentTable.LowerCaseSingularName}.{column.Name}");
                            }
                            tableBody.AppendLine("\t</th>");
                        }
                    }
                }
                tableBody.AppendLine("</tr>");

                //generate data
                tableBody.AppendLine($"<tr *ngFor=\"let {table.LowerCaseSingularName} of {table.LowerCaseSingularName}Controller.{table.LowerCaseName}; let i = index\">");
                {
                    foreach (Column column in table.Columns)
                    {
                        tableBody.AppendLine("\t<td>");
                        {
                            tableBody.AppendLine($"\t\t{{{{{table.LowerCaseSingularName}.{Helpers.GetLowerCaseString(column.Name)}}}}}");
                        }
                        tableBody.AppendLine("\t</td>");
                    }

                    foreach (Table parentTable in table.Parents)
                    {
                        foreach (Column column in parentTable.Columns)
                        {
                            tableBody.AppendLine("\t<td>");
                            {
                                tableBody.AppendLine($"\t\t{{{{{table.LowerCaseSingularName}.{parentTable.LowerCaseSingularName}.{Helpers.GetLowerCaseString(column.Name)}}}}}");
                            }
                            tableBody.AppendLine("\t</td>");
                        }
                    }
                }
                tableBody.AppendLine("</tr>");

                builder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
            }
            builder.AppendLine($"</tbody>");

            return builder.ToString();
        }

        private string GenerateDropDownForParentSelection(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder optionBody = new StringBuilder();

            builder.AppendLine($"<select id={table.PrimaryKeyColumn.Name}DropDown (change)=\"{table.LowerCaseSingularName}Changed($event.target.value)\">");
            {
                optionBody.AppendLine($"<option [value]=\"{table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}\" *ngFor= \"let {table.LowerCaseSingularName} of {table.LowerCaseSingularName}Controller.{table.LowerCaseName}\" >");
                {
                    optionBody.AppendLine($"{{{{{table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}}}}}");
                    foreach (Column column in table.EditableColumns)
                    {
                        optionBody.AppendLine($"{{{{{table.LowerCaseSingularName}.{column.LowerCaseName}}}}}");
                        //optionBody.AppendLine($"<input type=\"text\" id=\"{column.Name}\"><br>");

                    }
                }
                optionBody.AppendLine("</option>");
                builder.Append(Helpers.AddIndentation(optionBody.ToString(), 1));
            }
            builder.AppendLine($"</select>");

            return builder.ToString();
        }

        private string GenerateAddForm(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();

            builder.AppendLine($"<form (submit)=\"add{table.SingularName}($event)\">");
            {
                foreach (Column column in table.EditableColumns)
                {
                    tableBody.AppendLine($"{column.Name}");
                    tableBody.AppendLine($"<input type=\"text\" id=\"{column.Name}\"><br>");

                }

                foreach (Table parentTable in table.Parents)
                {
                    tableBody.AppendLine(GenerateDropDownForParentSelection(parentTable));
                }

                tableBody.AppendLine("<input type=\"submit\" value=\"Add\">");

                builder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
            }
            builder.AppendLine($"</form>");

            return builder.ToString();
        }


        private string GenerateViewForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(GenerateAddForm(table));
            builder.AppendLine(GenerateListView(table));

            Helpers.WriteFile($"{path}\\{table.SingularName}.component.html",
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
