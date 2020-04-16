using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonWebSocketsEndpointsGenerator : IGenerator
    {
        private Database _database;

        public PythonWebSocketsEndpointsGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateGetEndpoint(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#get endpoint");

            builder.AppendLine($"{table.LowerCaseName} = {table.SingularName}.get{table.Name}(session)");
            builder.AppendLine($"response = convertToJson({{'operation' : 'get', 'table' : '{table.Name}', 'data' : {table.LowerCaseName}}})");

            builder.AppendLine($"await websocket.send(response)");

            return builder.ToString();
        }

        private string GenerateSubscriptionEndpoint(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#subscription endpoint");

            builder.AppendLine($"{table.LowerCaseName} = {table.SingularName}.get{table.Name}(session)");
            builder.AppendLine($"response = convertToJson({{'operation' : 'get', 'table' : '{table.Name}', 'data' : {table.LowerCaseName}}})");

            builder.AppendLine($"{table.LowerCaseName}Subscribers.add(websocket)");
            builder.AppendLine($"await websocket.send(response)");

            return builder.ToString();
        }

        private string GenerateAddEndpoint(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#add endpoint");

            builder.AppendLine($"if checkArguments(request, ['{table.ToString("', '", withLowerCase: true, onlyEditable: true)}']) == False:");
            {
                builder.AppendLine($"\tprint('Not all parameters were provided for ADD in {table.Name}')");
                builder.AppendLine($"\tawait websocket.send(convertToJson({{'error' : 'Invalid request'}}))");
                builder.AppendLine($"\treturn");
            }

            builder.AppendLine($"{table.LowerCaseSingularName} = dict_as_obj(request['data'], {table.SingularName}.{table.SingularName}(), ['{table.ToString("', '", withLowerCase: true, onlyNonEditable: true)}'])");
            builder.AppendLine($"{table.LowerCaseSingularName} = {table.SingularName}.add{table.SingularName}(session, {table.LowerCaseSingularName})");
            builder.AppendLine($"response = convertToJson({{'operation' : 'add', 'table' : '{table.Name}', 'data' : {table.LowerCaseSingularName}}})");
            builder.AppendLine($"{table.LowerCaseName}Subscribers = set(filter(removeClosedConnection, {table.LowerCaseName}Subscribers))");
            builder.AppendLine($"for subscriber in {table.LowerCaseName}Subscribers:");
            builder.AppendLine($"\t await subscriber.send(response)");


            return builder.ToString();
        }

        private string GenerateUpdateEndpoint(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#update endpoint");

            builder.AppendLine($"if checkArguments(request, ['{table.PrimaryKeyColumn.LowerCaseName}']) == False:");
            {
                builder.AppendLine($"\tprint('Not all parameters were provided for UPDATE in {table.Name}')");
                builder.AppendLine($"\tawait websocket.send(convertToJson({{'error' : 'Invalid request'}}))");
                builder.AppendLine($"\treturn");
            }

            builder.AppendLine("data = request['data']");
            builder.AppendLine($"{table.LowerCaseSingularName} = {table.SingularName}.get{table.Name}By{table.PrimaryKeyColumn.Name}(session, data['{table.PrimaryKeyColumn.LowerCaseName}'])[0]");
            builder.AppendLine($"{table.LowerCaseSingularName} = dict_as_obj(data, {table.LowerCaseSingularName})");
            builder.AppendLine($"{table.LowerCaseSingularName} = {table.SingularName}.update{table.SingularName}(session, {table.LowerCaseSingularName})");
            builder.AppendLine($"response = convertToJson({{'operation' : 'update', 'table' : '{table.Name}', 'data' : {table.LowerCaseSingularName}}})");
            builder.AppendLine($"{table.LowerCaseName}Subscribers = set(filter(removeClosedConnection, {table.LowerCaseName}Subscribers))");
            builder.AppendLine($"for subscriber in {table.LowerCaseName}Subscribers:");
            builder.AppendLine($"\t await subscriber.send(response)");


            return builder.ToString();
        }

        private string GenerateDeleteEndpoint(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#delete endpoint");

            builder.AppendLine($"if checkArguments(request, ['{table.PrimaryKeyColumn.LowerCaseName}']) == False:");
            {
                builder.AppendLine($"\tprint('Not all parameters were provided for DELETE in {table.Name}')");
                builder.AppendLine($"\tawait websocket.send(convertToJson({{'error' : 'Invalid request'}}))");
                builder.AppendLine($"\treturn");
            }

            builder.AppendLine($"{table.LowerCaseSingularName} = {table.SingularName}.delete{table.SingularName}(session, request['data']['{table.PrimaryKeyColumn.LowerCaseName}'])");
            builder.AppendLine($"response = convertToJson({{'operation' : 'delete', 'table' : '{table.Name}', 'data' : {table.LowerCaseSingularName}}})");
            builder.AppendLine($"{table.LowerCaseName}Subscribers = set(filter(removeClosedConnection, {table.LowerCaseName}Subscribers))");
            builder.AppendLine($"for subscriber in {table.LowerCaseName}Subscribers:");
            builder.AppendLine($"\t await subscriber.send(response)");


            return builder.ToString();
        }

        private string GenerateEndpoints(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Websockets endpoints");
            builder.AppendLine("if request['operation'] == 'get':");
            builder.AppendLine(Helpers.AddIndentation(GenerateGetEndpoint(table), 1));

            builder.AppendLine("elif request['operation'] == 'subscribe':");
            builder.AppendLine(Helpers.AddIndentation(GenerateSubscriptionEndpoint(table), 1));

            builder.AppendLine("elif request['operation'] == 'add':");
            builder.AppendLine(Helpers.AddIndentation(GenerateAddEndpoint(table), 1));

            builder.AppendLine("elif request['operation'] == 'update':");
            builder.AppendLine(Helpers.AddIndentation(GenerateUpdateEndpoint(table), 1));

            builder.AppendLine("elif request['operation'] == 'delete':");
            builder.AppendLine(Helpers.AddIndentation(GenerateDeleteEndpoint(table), 1));

            return builder.ToString();
        }

        private void GenerateController(Table table, string path)
        {
            StringBuilder builder;
            StringBuilder functionBuilder;

            builder = new StringBuilder();
            functionBuilder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");
            builder.AppendLine("from SqlAlchemy import convertToJson, dict_as_obj");
            builder.AppendLine("from WebSocketsHelpers import checkArguments, removeClosedConnection");
            builder.AppendLine($"import {table.SingularName}");

            builder.AppendLine($"{table.LowerCaseName}Subscribers = set()");

            builder.AppendLine("async def requestReceived(websocket, session, request):");
            {
                functionBuilder.AppendLine($"global {table.LowerCaseName}Subscribers");

                //authentication
                if (table.RequiresSecurityToken)
                {
                    functionBuilder.AppendLine($"if websocket.authenticated == False:");
                    {
                        functionBuilder.AppendLine($"\tawait websocket.send(convertToJson({{'operation' : 'tokenError', 'table' : 'TokenAuthentication'}}))");
                        functionBuilder.AppendLine($"\treturn");
                    }
                }

                //endpoints
                functionBuilder.AppendLine(GenerateEndpoints(table));
            }
            builder.AppendLine(Helpers.AddIndentation(functionBuilder, 1));

            IO.WriteFile($"{path}\\{table.SingularName}WebSockets.py", (builder.ToString()));
        }

        public void Generate(string path)
        {
            string endpointsPath;

            endpointsPath = $"{path}";

            IO.CreateDirectory(endpointsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, endpointsPath);
            }
        }
    }
}
