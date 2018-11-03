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

        private string GenerateDedicatedGetRequestsFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder functionBody;

            builder = new StringBuilder();
            functionBody = new StringBuilder();


            foreach (DedicatedGetRequest request in table.DedicatedGetRequests)
            {
                StringBuilder parameters;
                StringBuilder urlParameters;

                parameters = new StringBuilder();
                urlParameters = new StringBuilder();

                foreach (Column column in request.Columns)
                {
                    parameters.Append($"{column.LowerCaseName}, ");
                    urlParameters.Append($"&{column.LowerCaseName}=${{{column.LowerCaseName}}}");
                }
                Helpers.RemoveLastApparition(parameters, ", ");

                builder.AppendLine($"Get{table.Name}By{request.ToString("")}({parameters})");
                builder.AppendLine("{");
                {

                    functionBody.AppendLine($"return this.http.get<{table.SingularName}[]>(ServerUrl.GetUrl()  + `{table.Name}.php?cmd=get{table.Name}By{request.ToString("")}{urlParameters}`);");

                    builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
                }
                builder.AppendLine("}");
                functionBody.Clear();
            }

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
                    functionBody.AppendLine($"{parentTable.LowerCaseSingularName} : {parentTable.SingularName}Service.GetDefault{parentTable.SingularName}(),");
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
                functionBody.AppendLine($"this.{table.LowerCaseName} = [{table.SingularName}Service.GetDefault{table.SingularName}()];");
                functionBody.AppendLine($"this.Get{table.Name}();");
            }
            builder.AppendLine(Helpers.AddIndentation(functionBody.ToString(), 1));
            builder.AppendLine("}");

            return builder.ToString();
        }


        private string GenerateAddFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"Add{table.SingularName}({table.LowerCaseSingularName})");
            builder.AppendLine("{");
            {

                functionBody.AppendLine($"return this.http.post<{table.SingularName}>(ServerUrl.GetUrl()  + \"{table.Name}.php?cmd=add{table.SingularName}\", {table.LowerCaseSingularName}).subscribe({table.LowerCaseSingularName} =>");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tconsole.log({table.LowerCaseSingularName});");
                    functionBody.AppendLine($"\tif(0 != {table.LowerCaseSingularName}.{Helpers.GetLowerCaseString(table.PrimaryKeyColumn.Name)})");
                    functionBody.AppendLine("\t{");
                    {
                        functionBody.AppendLine($"\t\tthis.{table.LowerCaseName}.push({table.LowerCaseSingularName})");
                    }
                    functionBody.AppendLine("\t}");
                }
                functionBody.AppendLine("});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateUpdateFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"Update{table.SingularName}({table.LowerCaseSingularName})");
            builder.AppendLine("{");
            {

                functionBody.AppendLine($"return this.http.put<{table.SingularName}>(ServerUrl.GetUrl()  + \"{table.Name}.php?cmd=update{table.SingularName}\", {table.LowerCaseSingularName}).subscribe({table.LowerCaseSingularName} =>");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tconsole.log({table.LowerCaseSingularName});");
                    functionBody.AppendLine($"\treturn {table.LowerCaseSingularName};");
                }
                functionBody.AppendLine("});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateDeleteFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"Delete{table.SingularName}({table.LowerCaseSingularName})");
            builder.AppendLine("{");
            {

                functionBody.AppendLine($"return this.http.delete<{table.SingularName}>(ServerUrl.GetUrl()  + \"{table.Name}.php?cmd=delete{table.SingularName}&{table.PrimaryKeyColumn.LowerCaseName}=\" +  {table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}, ).subscribe({table.LowerCaseSingularName} =>");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tconsole.log({table.LowerCaseSingularName});");
                    functionBody.AppendLine($"\treturn {table.LowerCaseSingularName};");
                }
                functionBody.AppendLine("});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }


        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder classBody = new StringBuilder();

            builder.AppendLine($"import {{HttpClient}} from '@angular/common/http';");
            builder.AppendLine($"import {{ ServerUrl }} from './ServerUrl'");
            builder.AppendLine("import { Injectable } from '@angular/core';");
            builder.AppendLine($"import {{ {table.SingularName} }} from '../app/Models/{table.SingularName}'");

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"import {{ {parentTable.SingularName} }} from '../app/Models/{parentTable.SingularName}'");
                builder.AppendLine($"import {{ {parentTable.SingularName}Service }} from './{parentTable.SingularName}Service'");
            }
            builder.AppendLine();

            //make it injectable
            builder.AppendLine(@"@Injectable({
    providedIn : 'root'
})");

            builder.AppendLine($"export class {table.SingularName}Service");
            builder.AppendLine("{");
            {


                classBody.AppendLine($"public {table.LowerCaseName} : {table.SingularName}[];");

                classBody.AppendLine(GenerateGetFunction(table));
                classBody.AppendLine(GenerateGetDefaultValueFunction(table));
                classBody.AppendLine(GenerateConstructor(table));
                classBody.AppendLine(GenerateAddFunction(table));
                classBody.AppendLine(GenerateUpdateFunction(table));
                classBody.AppendLine(GenerateDeleteFunction(table));
                classBody.AppendLine(GenerateDedicatedGetRequestsFunction(table));
            }
            builder.AppendLine(Helpers.AddIndentation(classBody.ToString(), 1));
            builder.AppendLine("}");

            Helpers.WriteFile($"{path}\\{table.SingularName}Service.ts",
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
