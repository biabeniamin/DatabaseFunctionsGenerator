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

            builder.AppendLine($"function Get{table.Name}($database)");
            builder.AppendLine("{");

            functionBody.AppendLine($"${table.LowerCaseName} = [];");
            functionBody.AppendLine($"$data = $database->ReadData(\"SELECT * FROM {table.Name}\");");
            functionBody.AppendLine();

            functionBody.AppendLine("foreach($data as $row)");
            functionBody.AppendLine("{");
            functionBody.AppendLine($"\t${Helpers.GetLowerCaseString(table.SingularName)} = new {table.SingularName}(");
            foreach (Column column in table.Columns)
            {
                functionBody.AppendLine($"\t$row[\"{column.Name}\"], ");
            }
            //to remove , \r\n
            functionBody.Remove(functionBody.Length - 4, 4);
            functionBody.AppendLine("\t);");
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
            foreach (Column column in table.EditableColumns)
            {
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
            functionBody.AppendLine($"$data = $database->ReadData(\"SELECT * FROM {table.Name}\");");
            functionBody.AppendLine();

            

            builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            //builder.Append(GenerateGetFunction(table);
            builder.Append(GenerateAddFunction(table));

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
