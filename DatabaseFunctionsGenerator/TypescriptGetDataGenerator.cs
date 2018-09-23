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

            builder.AppendLine($"Get{table.Name}()");
            builder.AppendLine("{");
            {

                functionBody.AppendLine($"return this.http.get<{table.SingularName}[]>(ServerUrl.GetUrl()  + \"{table.Name}.php?cmd=get{table.Name}\").subscribe(data =>");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tthis.{table.LowerCaseName} = data;");
                }
                functionBody.AppendLine("});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetDefaultValueFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"static GetDefault{table.SingularName}()");
            builder.AppendLine("{");
            {

                functionBody.AppendLine($"return {{");

                foreach(Column column in table.Columns)
                {
                    functionBody.AppendLine($"{Helpers.GetLowerCaseString(column.Name)} : {Helpers.GetDefaultColumnData(column.Type.Type)},");
                }

                foreach (Table parentTable in table.Parents)
                {
                    functionBody.AppendLine($"{parentTable.LowerCaseSingularName} : {parentTable.Name}.GetDefault{parentTable.SingularName}(),");
                }

                functionBody.AppendLine("};");
                if (functionBody.ToString().Contains(','))
                {
                    functionBody = functionBody.Remove(functionBody.ToString().LastIndexOf(','), 1);
                }

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateConstructor(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"constructor(private http:HttpClient)");
            builder.AppendLine("{");
            {
                functionBody.AppendLine($"this.{table.LowerCaseName} = [{table.Name}.GetDefault{table.SingularName}()];");
                functionBody.AppendLine($"this.Get{table.Name}();");
            }
            builder.AppendLine(Helpers.AddIndentation(functionBody.ToString(), 1));
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


        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder classBody = new StringBuilder();

            builder.AppendLine($"import {{HttpClient}} from '@angular/common/http';");
            builder.AppendLine($"import {{ ServerUrl }} from './ServerUrl'");
            builder.AppendLine($"import {{ {table.SingularName} }} from '../app/Models/{table.SingularName}'");

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"import {{ {parentTable.SingularName} }} from '../app/Models/{parentTable.SingularName}'");
                builder.AppendLine($"import {{ {parentTable.Name} }} from './{parentTable.Name}'");
            }

            builder.AppendLine($"export class {table.Name}");
            builder.AppendLine("{");
            {


                classBody.AppendLine($"public {table.LowerCaseName} : {table.SingularName}[];");

                //builder.AppendLine(GenerateListToObjectFunction(table));
                classBody.AppendLine(GenerateGetFunction(table));
                classBody.AppendLine(GenerateGetDefaultValueFunction(table));
                classBody.AppendLine(GenerateConstructor(table));
                //builder.AppendLine(GenerateGetByIdFunction(table));
                //builder.AppendLine(GenerateGetParentFunction(table));
                //builder.AppendLine(GenerateAddFunction(table));
                //builder.AppendLine(GenerateTestAddFunction(table));
                //builder.AppendLine(GenerateGetRequest(table));
            }
            builder.AppendLine(Helpers.AddIndentation(classBody.ToString(), 1));
            builder.AppendLine("}");

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
