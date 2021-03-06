﻿using DatabaseFunctionsGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptComponentViewGenerator
    {
        private static string GenerateListView(Table table, DatabaseType type)
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
                tableBody.AppendLine($"<tr *ngFor=\"let {table.LowerCaseSingularName} of {table.LowerCaseSingularName}Service.{table.LowerCaseName} | async; let i = index\">");
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
                            tableBody.AppendLine($"\t<td *ngIf=\"{table.LowerCaseSingularName}.{parentTable.LowerCaseSingularName}\">");
                            {
                                if (type == DatabaseType.Php || type == DatabaseType.PythonFlaskRestful)
                                    tableBody.AppendLine($"\t\t{{{{{table.LowerCaseSingularName}.{parentTable.LowerCaseSingularName}.{Helpers.GetLowerCaseString(column.Name)}}}}}");
                                else if (type == DatabaseType.PhytonFlaskRestless)
                                    tableBody.AppendLine($"\t\t{{{{{table.LowerCaseSingularName}.{parentTable.LowerCaseName}.{Helpers.GetLowerCaseString(column.Name)}}}}}");
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

        private static string GenerateDropDownForParentSelection(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder optionBody = new StringBuilder();

            builder.AppendLine($"<select id={table.PrimaryKeyColumn.Name}DropDown (change)=\"{table.LowerCaseSingularName}Changed($event.target.value)\">");
            {
                optionBody.AppendLine($"<option [value]=\"{table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}\" *ngFor= \"let {table.LowerCaseSingularName} of {table.LowerCaseSingularName}Service.{table.LowerCaseName} | async\" >");
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

        private static string GenerateAddForm(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();

            builder.AppendLine($"<form (submit)=\"add{table.SingularName}($event)\">");
            {
                foreach (Column column in table.EditableColumns.Where(col => { return false == col.Type.IsForeignKey; }))
                {
                    tableBody.AppendLine("<div class=\"input-container\">");
                    tableBody.AppendLine($"<input type=\"text\" placeholder= \"{column.Name}\" id=\"{column.Name}\" />");
                    tableBody.AppendLine("<div class=\"bar\"></div>");
                    tableBody.AppendLine("</div>");
                }

                foreach (Table parentTable in table.Parents)
                {
                    tableBody.AppendLine(GenerateDropDownForParentSelection(parentTable));
                }

                tableBody.AppendLine("<div class=\"button-container\">");
                {
                    tableBody.AppendLine("\t<input type=\"submit\" style=\"padding:20px; background:white;color:tomato;border:2px solid tomato;\" value=\"Add\">");
                }
                tableBody.AppendLine("</div>");

                builder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
            }
            builder.AppendLine($"</form>");

            return builder.ToString();
        }

        private static string GenerateRefreshButton(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();

            builder.AppendLine($"<form (submit)=\"{table.LowerCaseSingularName}Service.Get{table.Name}()\">");
            {
                tableBody.AppendLine("<div class=\"button-container\">");
                {
                    tableBody.AppendLine("\t<input type=\"submit\" style=\"padding:20px; background:white;color:tomato;border:2px solid tomato;\" value=\"Update\">");
                }
                tableBody.AppendLine("</div>");

                builder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
            }
            builder.AppendLine($"</form>");

            return builder.ToString();
        }

        private static string GenerateRefreshLiveForm(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();

            builder.AppendLine($"<form (submit)=\"updateLive($event)\">");
            {
                tableBody.AppendLine("<div class=\"button-container\">");
                {
                    tableBody.AppendLine("\t<input type=\"submit\" style=\"padding:20px; background:white;color:tomato;border:2px solid tomato;\" value=\"Update Live\">");
                }
                tableBody.AppendLine("</div>");

                builder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
            }
            builder.AppendLine($"</form>");

            return builder.ToString();
        }

        private static string GenerateCustomRequestsForm(Table table)
        {
            StringBuilder builder = new StringBuilder();
            foreach (DedicatedGetRequest request in table.DedicatedGetRequests)
            {
                StringBuilder formBuilder = new StringBuilder();
                StringBuilder tableBody = new StringBuilder();

                formBuilder.AppendLine($"<form (submit)=\"get{table.Name}By{request.ToString("")}($event)\">");
                {
                    tableBody.AppendLine($"<h3>Get {table.Name} by {request.ToString(", ")}</h3>");
                    foreach (Column column in request.Columns)
                    {
                        tableBody.AppendLine("<div class=\"input-container\">");
                        tableBody.AppendLine($"<input type=\"text\" placeholder= \"{column.Name}\" id=\"{column.Name}\" />");
                        tableBody.AppendLine("<div class=\"bar\"></div>");
                        tableBody.AppendLine("</div>");
                    }

                    tableBody.AppendLine("<div class=\"button-container\">");
                    {
                        tableBody.AppendLine($"\t<input type=\"submit\" style=\"padding:20px; background:white;color:tomato;border:2px solid tomato;\" value=\"Get by {request.ToString(" ")}\">");
                    }
                    tableBody.AppendLine("</div>");

                    formBuilder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
                }
                formBuilder.AppendLine($"</form>");
                builder.AppendLine(formBuilder.ToString());
            }
            return builder.ToString();
        }


        public static string GenerateViewForTable(Table table, DatabaseType type, string path)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("<div class=\"container\">");
            builder.AppendLine("\t<div class=\"card rerun\">");
            builder.AppendLine(Helpers.AddIndentation(GenerateRefreshButton(table),
                                                      2));
            builder.AppendLine(Helpers.AddIndentation(GenerateRefreshLiveForm(table),
                                                      2));
            builder.AppendLine(Helpers.AddIndentation(GenerateAddForm(table),
                                                      2));

            builder.AppendLine(Helpers.AddIndentation(GenerateListView(table, type),
                                                      2));

            builder.AppendLine(Helpers.AddIndentation(GenerateCustomRequestsForm(table),
                                                      2));

            builder.AppendLine("\t</div>");
            builder.AppendLine("</div>");

            IO.WriteFile($"{path}\\{table.LowerCaseSingularName}.component.html",
                builder.ToString());

            return builder.ToString();
        }
    }
}
