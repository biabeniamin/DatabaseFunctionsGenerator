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

        private string GenerateGetFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"function Get{table.Name}($database)");
            builder.AppendLine("{");

            functionBody.AppendLine($"${table.LowerCaseName} = [];");
            functionBody.AppendLine($"$data = $database->ReadData(\"SELECT * FROM {table.Name}\");");
            functionBody.AppendLine();

            functionBody.AppendLine("foreach($data as $row)");
            functionBody.AppendLine("{");
            functionBody.AppendLine($"\t${objectName} = new {table.SingularName}(");
            foreach (Column column in table.EditableColumns)
            {
                functionBody.AppendLine($"\t$row[\"{column.Name}\"], ");
            }
            //to remove , \r\n
            if(functionBody.ToString().Contains(','))
                functionBody.Remove(functionBody.ToString().LastIndexOf(','), 1);

            functionBody.AppendLine("\t);");
            functionBody.AppendLine();

            foreach (Column column in table.NonEditableColumns)
            {
                functionBody.AppendLine($"\t${objectName}->Set{column.Name}($row[\"{column.Name}\"]);");
            }
            functionBody.AppendLine();


            functionBody.AppendLine($"\t${table.LowerCaseName}[count(${table.LowerCaseName})] = ${Helpers.GetLowerCaseString(table.SingularName)};");


            functionBody.AppendLine("}");
            functionBody.AppendLine();

            functionBody.AppendLine($"return ${table.LowerCaseName};");

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

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

            parameter = Helpers.GetLowerCaseString(table.SingularName);

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
            String parameter;
            String objectName;
            
            builder = new StringBuilder();
            functionBody = new StringBuilder();
            objectName = Helpers.GetLowerCaseString(table.SingularName);

            parameter = Helpers.GetLowerCaseString(table.SingularName);

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

            builder.AppendLine("if(isset($_GET[\"cmd\"]))");
            builder.AppendLine("{");
            //to get data
            builder.AppendLine("\tif(\"getData\" == $_GET[\"cmd\"])");
            builder.AppendLine("\t{");
            builder.AppendLine($"\t\t$database = new DatabaseOperations();");
            builder.AppendLine($"\t\techo json_encode(Get{table.Name}($database));");
            builder.AppendLine("\t}");

            //to add data
            builder.AppendLine("\tif(\"addData\" == $_GET[\"cmd\"])");
            builder.AppendLine("\t{");

            addBlock.AppendLine("if(CheckGetParameters([");
            //foreach(Column )

            builder.AppendLine($"\t\t$database = new DatabaseOperations();");
            builder.AppendLine($"\t\techo json_encode(Get{table.Name}($database));");
            builder.AppendLine("\t}");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"<?php");
            builder.AppendLine($"require \'Models/{table.SingularName}.php\';");
            builder.AppendLine($"require \'DatabaseOperations.php\';");
            builder.AppendLine($"require \'Helpers.php\';");
            builder.AppendLine(GenerateGetFunction(table));
            builder.AppendLine(GenerateAddFunction(table));
            builder.AppendLine(GenerateTestAddFunction(table));
            builder.AppendLine(GenerateGetRequest(table));
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
