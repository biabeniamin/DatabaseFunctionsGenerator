using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptGetDataGenerator
    {
        private Database _database;

        public TypescriptGetDataGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateListToObjectFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = table.LowerCaseSingularName;

            builder.AppendLine($"function ConvertListTo{table.Name}($data)");
            builder.AppendLine("{");

            functionBody.AppendLine($"${table.LowerCaseName} = [];");
            functionBody.AppendLine();

            functionBody.AppendLine("foreach($data as $row)");
            functionBody.AppendLine("{");
            functionBody.AppendLine($"\t${objectName} = new {table.SingularName}(");
            foreach (Column column in table.EditableColumns)
            {
                functionBody.AppendLine($"\t$row[\"{column.Name}\"], ");
            }
            //to remove , \r\n
            if (functionBody.ToString().Contains(','))
                functionBody.Remove(functionBody.ToString().LastIndexOf(','), 1);

            functionBody.AppendLine("\t);");
            functionBody.AppendLine();

            foreach (Column column in table.NonEditableColumns)
            {
                functionBody.AppendLine($"\t${objectName}->Set{column.Name}($row[\"{column.Name}\"]);");
            }
            functionBody.AppendLine();


            functionBody.AppendLine($"\t${table.LowerCaseName}[count(${table.LowerCaseName})] = ${table.LowerCaseSingularName};");


            functionBody.AppendLine("}");
            functionBody.AppendLine();

            functionBody.AppendLine($"return ${table.LowerCaseName};");

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"function Get{table.Name}($database)");
            builder.AppendLine("{");

            functionBody.AppendLine($"$data = $database->ReadData(\"SELECT * FROM {table.Name}\");");
            functionBody.AppendLine($"${table.LowerCaseName} = ConvertListTo{table.Name}($data);");

            foreach (Table parentTable in table.Parents)
            {
                functionBody.AppendLine($"${table.LowerCaseName} = Complete{parentTable.Name}($database, ${table.LowerCaseName});");
            }


            functionBody.AppendLine($"return ${table.LowerCaseName};");

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetByIdFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = table.LowerCaseSingularName;

            builder.AppendLine($"function Get{table.Name}ById($database, ${table.LowerCaseSingularName}Id)");
            builder.AppendLine("{");

            functionBody.AppendLine($"$data = $database->ReadData(\"SELECT * FROM {table.Name} WHERE {table.PrimaryKeyColumn.Name} = ${Helpers.ConvertToSql(table.LowerCaseSingularName, table.PrimaryKeyColumn.Type.Type)}Id\");");
            functionBody.AppendLine($"${table.LowerCaseName} = ConvertListTo{table.Name}($data);");

            foreach (Table parentTable in table.Parents)
            {
                functionBody.AppendLine($"${table.LowerCaseName} = Complete{parentTable.Name}($database, ${table.LowerCaseName});");
            }


            functionBody.AppendLine($"return ${table.LowerCaseName};");

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetParentFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            StringBuilder doWhilenBody = new StringBuilder();
            string objectName;

            objectName = table.LowerCaseSingularName;

            foreach (Table parentTable in table.Parents)
            {
                functionBody = new StringBuilder();
                doWhilenBody = new StringBuilder();
                builder.AppendLine($"function Complete{parentTable.Name}($database, ${table.LowerCaseName})");
                builder.AppendLine("{");
                {

                    functionBody.AppendLine($"${parentTable.LowerCaseName} = Get{parentTable.Name}($database);");

                    functionBody.AppendLine($"foreach(${table.LowerCaseName} as ${table.LowerCaseSingularName})");
                    functionBody.AppendLine("{");
                    {
                        functionBody.AppendLine("\t$start = 0;");
                        functionBody.AppendLine($"\t$end = count(${parentTable.LowerCaseName}) - 1;");
                        functionBody.AppendLine("\tdo");
                        functionBody.AppendLine("\t{");
                        {
                            doWhilenBody.AppendLine("$mid = floor(($start + $end) / 2);");
                            doWhilenBody.AppendLine($"if(${table.LowerCaseSingularName}->Get{parentTable.SingularName}Id() > ${parentTable.LowerCaseName}[$mid]->Get{parentTable.PrimaryKeyColumn.Name}())");
                            doWhilenBody.AppendLine("{");
                            {
                                doWhilenBody.AppendLine("\t$start = $mid + 1;");
                            }
                            doWhilenBody.AppendLine("}");

                            doWhilenBody.AppendLine($"else if(${table.LowerCaseSingularName}->Get{parentTable.SingularName}Id() < ${parentTable.LowerCaseName}[$mid]->Get{parentTable.PrimaryKeyColumn.Name}())");
                            doWhilenBody.AppendLine("{");
                            {
                                doWhilenBody.AppendLine("\t$end = $mid - 1;");
                            }
                            doWhilenBody.AppendLine("}");

                            doWhilenBody.AppendLine($"else if(${table.LowerCaseSingularName}->Get{parentTable.SingularName}Id() == ${parentTable.LowerCaseName}[$mid]->Get{parentTable.PrimaryKeyColumn.Name}())");
                            doWhilenBody.AppendLine("{");
                            {
                                doWhilenBody.AppendLine("\t$start = $mid + 1;");
                                doWhilenBody.AppendLine("\t$end = $mid - 1;");
                                doWhilenBody.AppendLine($"\t${table.LowerCaseSingularName}->Set{parentTable.SingularName}(${parentTable.LowerCaseName}[$mid]);");
                            }
                            doWhilenBody.AppendLine("}");
                            functionBody.AppendLine();
                        }
                        functionBody.AppendLine(Helpers.AddIndentation(doWhilenBody.ToString(), 2));
                        functionBody.AppendLine("\t}while($start <= $end);");
                    }
                    functionBody.AppendLine("}");
                    functionBody.AppendLine();

                    functionBody.AppendLine($"return ${table.LowerCaseName};");

                    builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
                }
                builder.AppendLine("}");
            }

            return builder.ToString();
        }

        private string GenerateAddFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder functionBody;
            StringBuilder columnsCommaSeparated;
            StringBuilder dataColumnsCommaSeparated;
            String parameter;

            builder = new StringBuilder();
            functionBody = new StringBuilder();
            columnsCommaSeparated = new StringBuilder();
            dataColumnsCommaSeparated = new StringBuilder();

            parameter = table.LowerCaseSingularName;

            //generate columnsCommaSeparated and columnsCommaSeparated with data
            foreach (Column column in table.Columns)
            {
                if (column.Type.IsPrimaryKey)
                    continue;

                columnsCommaSeparated.Append($"{column.Name}, ");


                dataColumnsCommaSeparated.Append($"$query = $query . ");

                if (Types.Integer != column.Type.Type
                    && !column.IsCreationTimeColumn)
                {
                    dataColumnsCommaSeparated.Append("\"'\" . ");
                }

                if (column.IsCreationTimeColumn)
                {
                    dataColumnsCommaSeparated.Append($"\"NOW()\"");
                }
                else
                {
                    dataColumnsCommaSeparated.Append($"${parameter}->Get{column.Name}()");
                }

                if (Types.Integer != column.Type.Type
                    && !column.IsCreationTimeColumn)
                {
                    dataColumnsCommaSeparated.AppendLine(" . \"', \";");
                }
                else
                {
                    dataColumnsCommaSeparated.AppendLine(".\", \";");
                }
            }

            if (1 < columnsCommaSeparated.Length)
            {
                columnsCommaSeparated = columnsCommaSeparated.Remove(columnsCommaSeparated.Length - 2, 2);
            }

            if (1 < dataColumnsCommaSeparated.Length)
            {
                dataColumnsCommaSeparated = dataColumnsCommaSeparated.Remove(dataColumnsCommaSeparated.Length - 6, 2);
            }

            builder.AppendLine($"function Add{table.SingularName}($database, ${parameter})");
            builder.AppendLine("{");

            functionBody.AppendLine($"$query = \"INSERT INTO {table.Name}({columnsCommaSeparated.ToString()}) VALUES(\";");
            functionBody.AppendLine(dataColumnsCommaSeparated.ToString());
            functionBody.AppendLine($"$query = $query . \");\";");
            functionBody.AppendLine($"$result = $database->ExecuteSqlWithoutWarning($query);");
            functionBody.AppendLine("return $result;");
            functionBody.AppendLine();



            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateTestAddFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder functionBody;
            String objectName;

            builder = new StringBuilder();
            functionBody = new StringBuilder();
            objectName = table.LowerCaseSingularName;

            builder.AppendLine($"function TestAdd{table.SingularName}($database)");
            builder.AppendLine("{");


            functionBody.AppendLine($"${objectName} = new {table.SingularName}(");

            foreach (Column column in table.EditableColumns)
            {
                functionBody.AppendLine($"\t{Helpers.GetDefaultColumnData(column.Type.Type)},//{column.Name}");
            }

            if (functionBody.ToString().Contains(','))
            {
                functionBody.Remove(functionBody.ToString().LastIndexOf(','), 1);
            }
            functionBody.AppendLine(");");
            functionBody.AppendLine();

            functionBody.AppendLine($"Add{table.SingularName}($database, ${objectName});");

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetRequest(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder addBlock = new StringBuilder();
            string objectName;

            objectName = table.LowerCaseSingularName;

            builder.AppendLine("if(CheckGetParameters([\"cmd\"]))");
            builder.AppendLine("{");
            {
                //to get data
                builder.AppendLine($"\tif(\"get{table.Name}\" == $_GET[\"cmd\"])");
                builder.AppendLine("\t{");
                {
                    builder.AppendLine($"\t\t$database = new DatabaseOperations();");
                    builder.AppendLine($"\t\techo json_encode(Get{table.Name}($database));");
                }
                builder.AppendLine("\t}");

                //to get data by id

                builder.AppendLine($"\telse if(\"get{table.SingularName}ById\" == $_GET[\"cmd\"])");
                builder.AppendLine("\t{");
                {
                    builder.AppendLine($"\t\tif(CheckGetParameters([\"{table.SingularName}Id\"]))");
                    builder.AppendLine("\t\t{");
                    {
                        builder.AppendLine($"\t\t\t$database = new DatabaseOperations();");
                        builder.AppendLine($"\t\t\techo json_encode(Get{table.Name}ById($database, $_GET[\"{table.SingularName}Id\"]));");
                    }
                    builder.AppendLine("\t\t}");
                }
                builder.AppendLine("\t}");

                //to add data
                builder.AppendLine($"\telse if(\"add{table.SingularName}\" == $_GET[\"cmd\"])");
                builder.AppendLine("\t{");
                {

                    addBlock.AppendLine("\tif(CheckGetParameters([");
                    foreach (Column column in table.EditableColumns)
                    {
                        addBlock.AppendLine($"\t\t\'{column.Name}\',");
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
                            addBlock.AppendLine($"\t\t\t$_GET[\'{column.Name}\'],");
                        }

                        if (addBlock.ToString().Contains(','))
                        {
                            addBlock.Remove(addBlock.ToString().LastIndexOf(','), 1);
                        }

                        addBlock.AppendLine($"\t\t);");
                        addBlock.AppendLine();

                        addBlock.AppendLine($"\t\techo Add{table.SingularName}($database, ${objectName});");

                    }
                    addBlock.AppendLine($"\t}}");
                    builder.AppendLine(Helpers.AddIndentation(addBlock.ToString(), 1));

                }
                builder.AppendLine("\t}");
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"<?php");
            builder.AppendLine($"import {{ {table.SingularName} }} from '../app/Models/{table.SingularName}'");
            builder.AppendLine($"require_once \'DatabaseOperations.php\';");
            builder.AppendLine($"require_once \'Helpers.php\';");

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"import {{ {parentTable.SingularName} }} from '../app/Models/{parentTable.SingularName}'");
            }

            //builder.AppendLine(GenerateListToObjectFunction(table));
            //builder.AppendLine(GenerateGetFunction(table));
            //builder.AppendLine(GenerateGetByIdFunction(table));
            //builder.AppendLine(GenerateGetParentFunction(table));
            //builder.AppendLine(GenerateAddFunction(table));
            //builder.AppendLine(GenerateTestAddFunction(table));
            //builder.AppendLine(GenerateGetRequest(table));

            Helpers.WriteFile($"{path}\\Typescript\\{table.Name}.ts",
                builder.ToString());

            return builder.ToString();
        }

        public void Generate(string path)
        {
            foreach (Table table in _database.Tables)
            {
                GenerateFunctionsForTable(path, table);
            }
        }
    }
}
