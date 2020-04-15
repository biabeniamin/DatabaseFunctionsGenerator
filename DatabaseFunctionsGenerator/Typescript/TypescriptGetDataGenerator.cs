using DatabaseFunctionsGenerator.Models;
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

                string url = "";
                if (_database.Type == DatabaseType.Php)
                    url = $"{table.Name}.php?cmd=get{ table.Name}";
                else if (_database.Type == DatabaseType.Phyton)
                    url = $"api/{table.LowerCaseName}";

                if (table.RequiresSecurityToken)
                    url += "&token=${this.token}";

                functionBody.AppendLine($"return this.http.get<{table.SingularName}[]>(ServerUrl.GetUrl()  + `{url}`).subscribe(data =>");
                functionBody.AppendLine("{");
                {
                    if (_database.Type == DatabaseType.Php)
                        functionBody.AppendLine($"\tthis.{table.LowerCaseName}.next(data);");
                    else if (_database.Type == DatabaseType.Phyton)
                        functionBody.AppendLine($"\tthis.{table.LowerCaseName} = data[\"objects\"];");
                }
                functionBody.AppendLine("});");

                builder.Append(Helpers.AddIndentation(functionBody.ToString(), 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetLastFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();

            builder.AppendLine($"GetLast{table.SingularName}()");
            builder.AppendLine("{");
            {
                string url = $"{table.Name}.php?cmd=getLast{table.SingularName}";
                if (table.RequiresSecurityToken)
                    url += "&token=${this.token}";

                functionBody.AppendLine($"return this.http.get<{table.SingularName}[]>(ServerUrl.GetUrl()  + `{url}`);");

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
                    if(_database.Type == DatabaseType.Php)
                        urlParameters.Append($"&{column.LowerCaseName}=${{{column.LowerCaseName}}}");
                    else if (_database.Type == DatabaseType.Phyton)
                        urlParameters.Append($"{{\"name\":\"{column.LowerCaseName}\",\"op\":\"eq\",\"val\":\"${{{column.LowerCaseName}}}\"}}, ");

                }
                Helpers.RemoveLastApparition(parameters, ", ");
                Helpers.RemoveLastApparition(urlParameters, ", ");

                string url = "";
                if (_database.Type == DatabaseType.Php)
                {
                    url = $"{table.Name}.php?cmd=get{table.Name}By{request.ToString("")}{urlParameters}";
                }
                else if (_database.Type == DatabaseType.Phyton)
                {
                    url = $"api/{table.LowerCaseName}?q={{\"filters\":[{urlParameters}]}}";
                }

                if (table.RequiresSecurityToken)
                    url += "&token=${this.token}";

                builder.AppendLine($"Get{table.Name}By{request.ToString("")}({parameters})");
                builder.AppendLine("{");
                {
                    functionBody.AppendLine($"return this.http.get<{table.SingularName}[]>(ServerUrl.GetUrl()  + `{url}`);");

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
                    if(_database.Type == DatabaseType.Php)
                        functionBody.AppendLine($"{parentTable.LowerCaseSingularName} : {parentTable.SingularName}Service.GetDefault{parentTable.SingularName}(),");
                    else if (_database.Type == DatabaseType.Phyton)
                        functionBody.AppendLine($"{parentTable.LowerCaseName} : {parentTable.SingularName}Service.GetDefault{parentTable.SingularName}(),");
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

            builder.Append($"constructor(private http:HttpClient, private webSockets : WebSockets");

            if(table.RequiresSecurityToken)
                builder.Append($", private auth : AuthenticationService");
            builder.AppendLine($")");
            builder.AppendLine("{");
            {
                if (table.RequiresSecurityToken)
                {
                    functionBody.AppendLine("this.auth.CheckToken();");
                    functionBody.AppendLine("this.token = this.auth.GetToken();");
                }
                functionBody.AppendLine($"this.{table.LowerCaseName} = new BehaviorSubject([{table.SingularName}Service.GetDefault{table.SingularName}()]);");
                functionBody.AppendLine($"this.Get{table.Name}();");
                functionBody.AppendLine("this.webSockets.SetOnConnectionEstablished(() => this.ConnectToWebSockets());");
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
                string url = "";
                string sentData = "";
                if (_database.Type == DatabaseType.Php)
                {
                    url = $"{table.Name}.php?cmd=add{table.SingularName}";
                    sentData = table.LowerCaseSingularName;
                }
                else if (_database.Type == DatabaseType.Phyton)
                {
                    url = $"api/{table.LowerCaseName}";
                    sentData = $"encode{table.SingularName}({table.LowerCaseSingularName})";
                }

                if (table.RequiresSecurityToken)
                    url += "&token=${this.token}";

                //websockets
                functionBody.AppendLine("if (this.webSocketsSubject!=null)");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tthis.webSocketsSubject.next(new Message(this.constructor.name, new Request('add', '{table.Name}', {table.LowerCaseSingularName})));");
                    functionBody.AppendLine("\treturn");
                }
                functionBody.AppendLine("}");
                functionBody.AppendLine();

                //http
                functionBody.AppendLine($"return this.http.post<{table.SingularName}>(ServerUrl.GetUrl()  + `{url}`, {sentData}).subscribe({table.LowerCaseSingularName} =>");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tconsole.log({table.LowerCaseSingularName});");
                    functionBody.AppendLine($"\tif(0 != {table.LowerCaseSingularName}.{Helpers.GetLowerCaseString(table.PrimaryKeyColumn.Name)})");
                    functionBody.AppendLine("\t{");
                    {
                        functionBody.AppendLine($"\t\tlet items = this.{table.LowerCaseName}.getValue()");
                        functionBody.AppendLine($"\t\titems.push({table.LowerCaseSingularName})");
                        functionBody.AppendLine($"\t\tthis.{table.LowerCaseName}.next(items)");

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

            string url = "";
            if (_database.Type == DatabaseType.Php)
            {
                url = $"{table.Name}.php?cmd=update{table.SingularName}";
            }
            else if (_database.Type == DatabaseType.Phyton)
            {
                url = $"\"api/{table.LowerCaseName}/\" + {table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}";
            }

            if (table.RequiresSecurityToken)
                url += "&token=${this.token}";

            builder.AppendLine($"Update{table.SingularName}({table.LowerCaseSingularName})");
            builder.AppendLine("{");
            {
                //websockets
                functionBody.AppendLine("if (this.webSocketsSubject!=null)");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tthis.webSocketsSubject.next(new Message(this.constructor.name, new Request('update', '{table.Name}', {table.LowerCaseSingularName})));");
                    functionBody.AppendLine("\treturn");
                }
                functionBody.AppendLine("}");
                functionBody.AppendLine();

                //http
                functionBody.AppendLine($"return this.http.put<{table.SingularName}>(ServerUrl.GetUrl()  + `{url}`, {table.LowerCaseSingularName}).subscribe({table.LowerCaseSingularName} =>");
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

            string url = "";
            if (_database.Type == DatabaseType.Php)
            {
                url = $"{table.Name}.php?cmd=delete{table.SingularName}";
                if (table.RequiresSecurityToken)
                    url += "&token=${this.token}";
                url += $"&{table.PrimaryKeyColumn.LowerCaseName}=";
            }
            else if (_database.Type == DatabaseType.Phyton)
            {
                url = $"api/{table.LowerCaseName}/";
            }

            builder.AppendLine($"Delete{table.SingularName}({table.LowerCaseSingularName})");
            builder.AppendLine("{");
            {
                //websockets
                functionBody.AppendLine("if (this.webSocketsSubject!=null)");
                functionBody.AppendLine("{");
                {
                    functionBody.AppendLine($"\tthis.webSocketsSubject.next(new Message(this.constructor.name, new Request('delete', '{table.Name}', {table.LowerCaseSingularName})));");
                    functionBody.AppendLine("\treturn");
                }
                functionBody.AppendLine("}");
                functionBody.AppendLine();

                //http
                functionBody.AppendLine($"return this.http.delete<{table.SingularName}>(ServerUrl.GetUrl()  + `{url}` +  {table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}).subscribe({table.LowerCaseSingularName} =>");
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

        private string GenerateWebSocketsConnectFunction(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder functionBody = new StringBuilder();
            string objectName;

            objectName = Helpers.GetLowerCaseString(table.SingularName);

            builder.AppendLine($"ConnectToWebSockets()");
            builder.AppendLine("{");
            {
                functionBody.AppendLine($"this.webSocketsSubject = this.webSockets.getSubject('{table.Name}');");
                functionBody.AppendLine($"this.webSocketsSubject.subscribe(message =>");
                functionBody.AppendLine("{");
                {
                    StringBuilder observer = new StringBuilder();

                    observer.AppendLine("\tif(message.sender != WebSockets.name)");
                    observer.AppendLine("\t\treturn");

                    observer.AppendLine("\tlet request = message.data;");
                    observer.AppendLine("\tconsole.log(request);");
                    observer.AppendLine("if(request.operation == 'get')");
                    observer.AppendLine("{");
                    {
                        observer.AppendLine($"\tthis.{table.LowerCaseName}.next(request.data);");
                    }
                    observer.AppendLine("}");
                    observer.AppendLine("else if(request.operation == 'add')");
                    observer.AppendLine("{");
                    {
                        observer.AppendLine($"\tlet items = this.{table.LowerCaseName}.getValue()");
                        observer.AppendLine("\titems.push(request.data);");
                        observer.AppendLine($"\tthis.{table.LowerCaseName}.next(items);");
                    }
                    observer.AppendLine("}");
                    observer.AppendLine("else if(request.operation == 'update')");
                    observer.AppendLine("{");
                    {
                        observer.AppendLine($"\tlet items = this.{table.LowerCaseName}.getValue()");
                        observer.AppendLine("\tfor(let i = 0; i < items.length; i++)");
                        observer.AppendLine("\t{");
                        {
                            observer.AppendLine($"\t\tif(items[i].{table.PrimaryKeyColumn.LowerCaseName} == request.data.{table.PrimaryKeyColumn.LowerCaseName})");
                            observer.AppendLine("\t\t{");
                            observer.AppendLine("\t\t\titems[i] = request.data;");
                            observer.AppendLine("\t\t\tbreak;");
                            observer.AppendLine("\t\t}");
                        }
                        observer.AppendLine("\t}");
                        observer.AppendLine($"\tthis.{table.LowerCaseName}.next(items);");
                    }
                    observer.AppendLine("}");
                    observer.AppendLine("else if(request.operation == 'delete')");
                    observer.AppendLine("{");
                    {
                        observer.AppendLine($"\tlet items = this.{table.LowerCaseName}.getValue()");
                        observer.AppendLine("\tfor(let i = 0; i < items.length; i++)");
                        observer.AppendLine("\t{");
                        {
                            observer.AppendLine($"\t\tif(items[i].{table.PrimaryKeyColumn.LowerCaseName} == request.data.{table.PrimaryKeyColumn.LowerCaseName})");
                            observer.AppendLine("\t\t{");
                            observer.AppendLine("\t\t\titems.splice(i, 1);");
                            observer.AppendLine("\t\t\tbreak;");
                            observer.AppendLine("\t\t}");
                        }
                        observer.AppendLine("\t}");
                        observer.AppendLine($"\tthis.{table.LowerCaseName}.next(items);");
                    }
                    observer.AppendLine("}");
                    //authentication token error
                    if (table.RequiresSecurityToken)
                    {
                        observer.AppendLine("else if(request.operation == 'tokenError')");
                        observer.AppendLine("{");
                        {
                            observer.AppendLine($"\tthis.auth.RemoveToken();");
                        }
                        observer.AppendLine("}");
                    }


                    functionBody.AppendLine(Helpers.AddIndentation(observer, 1));
                }
                functionBody.AppendLine("});");
                functionBody.AppendLine($"this.webSocketsSubject.next(new Message(this.constructor.name, new Request('subscribe', '{table.Name}', null)));");
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
            builder.AppendLine("import { BehaviorSubject, Subject } from 'rxjs';");
            builder.AppendLine("import { WebSockets, Message, Request } from './WebSockets';");
            builder.AppendLine($"import {{ {table.SingularName}, encode{table.SingularName} }} from '../app/Models/{table.SingularName}'");

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"import {{ {parentTable.SingularName} }} from '../app/Models/{parentTable.SingularName}'");
                builder.AppendLine($"import {{ {parentTable.SingularName}Service }} from './{parentTable.SingularName}Service'");
            }

            if (table.RequiresSecurityToken)
                builder.AppendLine("import { AuthenticationService } from './AuthenticationService';");


            builder.AppendLine();

            //make it injectable
            builder.AppendLine(@"@Injectable({
    providedIn : 'root'
})");

            builder.AppendLine($"export class {table.SingularName}Service");
            builder.AppendLine("{");
            {


                classBody.AppendLine($"public {table.LowerCaseName} : BehaviorSubject<{table.SingularName}[]>;");
                classBody.AppendLine($"private webSocketsSubject : Subject<Message>;");
                if (table.RequiresSecurityToken)
                    classBody.AppendLine($"private token : string;");

                classBody.AppendLine(GenerateGetFunction(table));
                classBody.AppendLine(GenerateGetLastFunction(table));
                classBody.AppendLine(GenerateGetDefaultValueFunction(table));
                classBody.AppendLine(GenerateDedicatedGetRequestsFunction(table));
                classBody.AppendLine(GenerateConstructor(table));
                classBody.AppendLine(GenerateAddFunction(table));
                classBody.AppendLine(GenerateUpdateFunction(table));
                classBody.AppendLine(GenerateDeleteFunction(table));
                classBody.AppendLine(GenerateWebSocketsConnectFunction(table));
            }
            builder.AppendLine(Helpers.AddIndentation(classBody.ToString(), 1));
            builder.AppendLine("}");

            IO.WriteFile($"{path}\\{table.SingularName}Service.ts",
                builder.ToString());

            return builder.ToString();
        }

        public void Generate(string path)
        {
            string controllersPath;

            controllersPath = $"{path}\\Controllers";

            IO.CreateDirectory(controllersPath);
            foreach (Table table in _database.Tables)
            {
                GenerateFunctionsForTable(controllersPath, table);
            }
        }
    }
}
