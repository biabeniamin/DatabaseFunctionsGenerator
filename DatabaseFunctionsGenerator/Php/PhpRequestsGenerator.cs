using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Php
{
    public class PhpRequestsGenerator
    {
        private static string GenerateGetAllRequest(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //to get data
            builder.AppendLine($"if(\"get{table.Name}\" == $_GET[\"cmd\"])");
            builder.AppendLine("{");
            {
                builder.AppendLine($"\t$database = new DatabaseOperations();");
                builder.AppendLine($"\t\techo json_encode(Get{table.Name}($database));");
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GenerateGetDedicatedRequest(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            foreach (DedicatedGetRequest dedicatedRequest in table.DedicatedGetRequests)
            {
                builder.AppendLine($"else if(\"get{table.Name}By{dedicatedRequest.ToString("")}\" == $_GET[\"cmd\"])");
                builder.AppendLine("{");
                {
                    StringBuilder dedicatedBuilder;

                    dedicatedBuilder = new StringBuilder();

                    dedicatedBuilder.AppendLine("if(CheckGetParameters([");
                    foreach (Column column in dedicatedRequest.Columns)
                    {
                        dedicatedBuilder.AppendLine($"\t\'{column.LowerCaseName}\',");
                    }
                    if (dedicatedBuilder.ToString().Contains(','))
                    {
                        dedicatedBuilder.Remove(dedicatedBuilder.ToString().LastIndexOf(','), 1);
                    }

                    dedicatedBuilder.AppendLine("\t]))");
                    dedicatedBuilder.AppendLine("{");
                    {
                        dedicatedBuilder.AppendLine($"\t$database = new DatabaseOperations();");
                        dedicatedBuilder.AppendLine($"\techo json_encode(Get{table.Name}By{dedicatedRequest.ToString("")}($database, ");

                        foreach (Column column in dedicatedRequest.Columns)
                        {
                            dedicatedBuilder.AppendLine($"\t\t$_GET[\"{column.LowerCaseName}\"],");
                        }

                        if (dedicatedBuilder.ToString().Contains(","))
                        {
                            dedicatedBuilder.Remove(dedicatedBuilder.ToString().LastIndexOf(","), 1);
                        }

                        dedicatedBuilder.AppendLine($"\t));");
                    }
                    dedicatedBuilder.AppendLine("}");
                    builder.AppendLine(Helpers.AddIndentation(dedicatedBuilder.ToString(),
                        1));
                }
                builder.AppendLine("}");
            }

            return builder.ToString();
        }

        private static string GenerateGetAddRequest(Table table)
        {
            StringBuilder builder;
            StringBuilder addBlock;

            builder = new StringBuilder();
            addBlock = new StringBuilder();

                builder.AppendLine($"else if(\"add{table.SingularName}\" == $_GET[\"cmd\"])");
                builder.AppendLine("{");
                {

                    addBlock.AppendLine("if(CheckGetParameters([");
                    foreach (Column column in table.EditableColumns)
                    {
                        addBlock.AppendLine($"\t\'{column.LowerCaseName}\',");
                    }
                    if (addBlock.ToString().Contains(','))
                    {
                        addBlock.Remove(addBlock.ToString().LastIndexOf(','), 1);
                    }

                    addBlock.AppendLine("]))");
                    addBlock.AppendLine("{");
                    {

                        addBlock.AppendLine($"\t$database = new DatabaseOperations();");
                        addBlock.AppendLine($"\t${table.LowerCaseSingularName} = new {table.SingularName}(");

                        foreach (Column column in table.EditableColumns)
                        {
                            addBlock.AppendLine($"\t\t$_GET[\'{column.LowerCaseName}\'],");
                        }

                        if (addBlock.ToString().Contains(','))
                        {
                            addBlock.Remove(addBlock.ToString().LastIndexOf(','), 1);
                        }

                        addBlock.AppendLine($"\t);");
                        addBlock.AppendLine();

                        addBlock.AppendLine($"\techo json_encode(Add{table.SingularName}($database, ${table.LowerCaseSingularName}));");

                    }
                    addBlock.AppendLine($"}}");
                    builder.AppendLine(Helpers.AddIndentation(addBlock.ToString(), 1));

                }
                builder.AppendLine("}");

            return builder.ToString();
        }


        private static string GenerateGetRequest(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("if(CheckGetParameters([\"cmd\"]))");
            builder.AppendLine("{");
            {
                //to get data
                builder.AppendLine(Helpers.AddIndentation(GenerateGetAllRequest(table),
                    1));

                //to get data by dedicated fields
                builder.AppendLine(Helpers.AddIndentation(GenerateGetDedicatedRequest(table),
                    1));

                //to add data
                builder.AppendLine(Helpers.AddIndentation(GenerateGetAddRequest(table),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GeneratePostRequest(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder addBlock = new StringBuilder();
            string objectName;

            objectName = table.LowerCaseSingularName;

            builder.AppendLine("if(CheckGetParameters([\"cmd\"]))");
            builder.AppendLine("{");
            {
                //to add data
                builder.AppendLine($"\tif(\"add{table.SingularName}\" == $_GET[\"cmd\"])");
                builder.AppendLine("\t{");
                {

                    addBlock.AppendLine("\tif(CheckPostParameters([");
                    foreach (Column column in table.EditableColumns)
                    {
                        addBlock.AppendLine($"\t\t\'{column.LowerCaseName}\',");
                    }
                    if (addBlock.ToString().Contains(','))
                    {
                        addBlock.Remove(addBlock.ToString().LastIndexOf(','), 1);
                    }

                    addBlock.AppendLine("\t]))");
                    addBlock.AppendLine("\t{");
                    {

                        addBlock.AppendLine($"\t\t$database = new DatabaseOperations();");
                        addBlock.AppendLine($"\t\t${objectName} = new {table.SingularName}(");

                        foreach (Column column in table.EditableColumns)
                        {
                            addBlock.AppendLine($"\t\t\t$_POST[\'{column.LowerCaseName}\'],");
                        }

                        Helpers.RemoveLastApparition(addBlock, ",");

                        addBlock.AppendLine($"\t\t);");
                        addBlock.AppendLine();

                        addBlock.AppendLine($"\t\techo json_encode(Add{table.SingularName}($database, ${objectName}));");

                    }
                    addBlock.AppendLine($"\t}}");
                    builder.AppendLine(Helpers.AddIndentation(addBlock.ToString(), 1));

                }
                builder.AppendLine("\t}");
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GeneratePutRequest(Table table)
        {
            StringBuilder builder;
            StringBuilder requestBody;

            builder = new StringBuilder();
            requestBody = new StringBuilder();

            builder.AppendLine("if(CheckGetParameters([\"cmd\"]))");
            builder.AppendLine("{");
            {
                builder.AppendLine($"\tif(\"update{table.SingularName}\" == $_GET[\"cmd\"])");
                builder.AppendLine("\t{");
                {
                    requestBody.AppendLine($"$database = new DatabaseOperations();");

                    requestBody.AppendLine($"${table.LowerCaseSingularName} = new {table.SingularName}(");

                    foreach (Column column in table.EditableColumns)
                    {
                        requestBody.AppendLine($"\t$_POST[\'{column.LowerCaseName}\'],");
                    }

                    Helpers.RemoveLastApparition(requestBody, ",");

                    requestBody.AppendLine($");");
                    foreach (Column column in table.NonEditableColumns)
                    {
                        requestBody.AppendLine($"${table.LowerCaseSingularName}->Set{column.Name}($_POST['{column.LowerCaseName}']);");
                    }
                    requestBody.AppendLine();

                    requestBody.AppendLine($"${table.LowerCaseSingularName} = Update{table.SingularName}($database, ${table.LowerCaseSingularName});");
                    requestBody.AppendLine($"echo json_encode(${table.LowerCaseSingularName});");

                    builder.AppendLine(Helpers.AddIndentation(requestBody,
                        2));
                }
                builder.AppendLine("\t}");
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        public static string GenerateRequests(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine(GenerateGetRequest(table));
            builder.AppendLine(GeneratePostRequest(table));
            builder.AppendLine(GeneratePutRequest(table));


            return builder.ToString();
        }
    }
}
