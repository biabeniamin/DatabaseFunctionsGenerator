using DatabaseFunctionsGenerator.Php;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class PhpDatabaseFunctionGenerator
    {
        private Database _database;

        public PhpDatabaseFunctionGenerator(Database database)
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

        private string GenerateDedicatedRequestFunctions(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            foreach (DedicatedGetRequest dedicatedRequest in table.DedicatedGetRequests)
            {
                StringBuilder functionBody;

                functionBody = new StringBuilder();

                builder.Append($"function Get{table.Name}By{dedicatedRequest.ToString("")}($database, ");

                foreach (Column column in dedicatedRequest.Columns)
                {
                    builder.Append($"${column.LowerCaseName}, ");
                }
                if (builder.ToString().Contains(", "))
                {
                    builder.Remove(builder.ToString().LastIndexOf(", "), 2);
                }

                builder.AppendLine($")");
                builder.AppendLine("{");

                functionBody.Append($"$data = $database->ReadData(\"SELECT * FROM {table.Name} WHERE ");

                foreach(Column column in dedicatedRequest.Columns)
                {
                    functionBody.Append($"{column.Name} = {Helpers.ConvertToSql("$",column.LowerCaseName, column.Type.Type)} and ");
                }
                if (functionBody.ToString().Contains(" and"))
                {
                    functionBody.Remove(functionBody.ToString().LastIndexOf(" and"), 5);
                }
                functionBody.AppendLine($"\");");

                functionBody.AppendLine($"${table.LowerCaseName} = ConvertListTo{table.Name}($data);");

                functionBody.AppendLine($"if(0== count(${table.LowerCaseName}))");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\treturn [GetEmpty{table.SingularName}()];");
                }
                functionBody.AppendLine("}");

                foreach (Table parentTable in table.Parents)
                {
                    functionBody.AppendLine($"Complete{parentTable.Name}($database, ${table.LowerCaseName});");
                }

                functionBody.AppendLine($"return ${table.LowerCaseName};");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
                builder.AppendLine("}");
            }

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
            functionBody.AppendLine($"$id = $database->ExecuteSqlWithoutWarning($query);");

            functionBody.AppendLine($"${parameter}->Set{table.PrimaryKeyColumn.Name}($id);");
            functionBody.AppendLine($"${parameter}->Set{table.CreationTimeColumn.Name}(date('Y-m-d H:i:s'));");

            foreach (Table parentTable in table.Parents)
            {
                functionBody.AppendLine($"${parameter}->Set{parentTable.SingularName}(Get{parentTable.Name}By{parentTable.GetDedicatedRequestById}($database, ${parameter}->Get{parentTable.PrimaryKeyColumn.Name}())[0]);");
            }

            functionBody.AppendLine($"return ${parameter};");
            functionBody.AppendLine();



            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateUpdateFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder functionBody;
            StringBuilder dataColumnsCommaSeparated;
            String parameter;

            builder = new StringBuilder();
            functionBody = new StringBuilder();
            dataColumnsCommaSeparated = new StringBuilder();

            parameter = table.LowerCaseSingularName;

            //generate columnsCommaSeparated and columnsCommaSeparated with data
            foreach (Column column in table.Columns)
            {
                if (column.Type.IsPrimaryKey)
                    continue;

                dataColumnsCommaSeparated.Append($"$query = $query . ");
                dataColumnsCommaSeparated.Append($"\"{column.Name}=");

                if (Types.Integer != column.Type.Type
                    && !column.IsCreationTimeColumn)
                {
                    dataColumnsCommaSeparated.Append("'\" . ");
                }

                if (column.IsCreationTimeColumn)
                {
                    dataColumnsCommaSeparated.Append($"NOW()\"");
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


            if (1 < dataColumnsCommaSeparated.Length)
            {
                dataColumnsCommaSeparated = dataColumnsCommaSeparated.Remove(dataColumnsCommaSeparated.Length - 6, 2);
            }

            dataColumnsCommaSeparated.Append($"$query = $query . \" WHERE {table.PrimaryKeyColumn.Name}=\" . ${table.LowerCaseSingularName}->Get{table.PrimaryKeyColumn.Name}();");


            builder.AppendLine($"function Update{table.SingularName}($database, ${parameter})");
            builder.AppendLine("{");

            functionBody.AppendLine($"$query = \"UPDATE {table.Name} SET\";");
            functionBody.AppendLine(dataColumnsCommaSeparated.ToString());
            functionBody.AppendLine();

            functionBody.AppendLine($"$result = $database->ExecuteSqlWithoutWarning($query);");

            functionBody.AppendLine("if(0 == $result)");
            functionBody.AppendLine("{");
            {
                functionBody.AppendLine($"\t${parameter}->Set{table.PrimaryKeyColumn.Name}(0);");
            }
            functionBody.AppendLine("}");


            functionBody.AppendLine($"return ${parameter};");
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

        private string GenerateGetEmptyEntryFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder functionBody;
            String objectName;

            builder = new StringBuilder();
            functionBody = new StringBuilder();
            objectName = table.LowerCaseSingularName;

            builder.AppendLine($"function GetEmpty{table.SingularName}()");
            builder.AppendLine("{");


            functionBody.AppendLine($"${objectName} = new {table.SingularName}(");

            foreach (Column column in table.EditableColumns)
            {
                functionBody.AppendLine($"\t{Helpers.GetEmptyColumnData(column.Type.Type)},//{column.Name}");
            }

            if (functionBody.ToString().Contains(','))
            {
                functionBody.Remove(functionBody.ToString().LastIndexOf(','), 1);
            }
            functionBody.AppendLine(");");
            functionBody.AppendLine();

            functionBody.AppendLine($"return ${objectName};");

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        

        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"<?php");
            builder.AppendLine("header('Access-Control-Allow-Origin: *'); ");
            builder.AppendLine("header('Access-Control-Allow-Headers: *'); ");
            builder.AppendLine("header('Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS');");
            builder.AppendLine("$_POST = json_decode(file_get_contents('php://input'), true);");
            builder.AppendLine($"require_once \'Models/{table.SingularName}.php\';");
            builder.AppendLine($"require_once \'DatabaseOperations.php\';");
            builder.AppendLine($"require_once \'Helpers.php\';");

            foreach(Table parentTable in table.Parents)
            {
                builder.AppendLine($"require_once \'{parentTable.Name}.php\';");
            }

            builder.AppendLine(GenerateListToObjectFunction(table));
            builder.AppendLine(GenerateGetFunction(table));
            builder.AppendLine(GenerateDedicatedRequestFunctions(table));
            builder.AppendLine(GenerateGetParentFunction(table));
            builder.AppendLine(GenerateAddFunction(table));
            builder.AppendLine(GenerateTestAddFunction(table));
            builder.AppendLine(GenerateGetEmptyEntryFunction(table));
            builder.AppendLine(PhpRequestsGenerator.GenerateRequests(table));
            builder.AppendLine(GenerateUpdateFunction(table));
            builder.AppendLine($"?>");

            Helpers.WriteFile($"{path}\\Php\\{table.Name}.php",
                builder.ToString());

            return builder.ToString();
        }

        public void Generate(string path)
        {
            foreach(Table table in _database.Tables)
            {
                GenerateFunctionsForTable(path, table);
            }
        }
    }
}
