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
            builder.AppendLine($"response = convertToJson({{'operation' : 'get', 'data' : {table.LowerCaseName}}})");

            builder.AppendLine($"await websocket.send(response)");

            return builder.ToString();
        }

        private string GenerateEndpoints(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Websockets endpoints");
            builder.AppendLine("if request['action'] == 'get':");
            builder.AppendLine(Helpers.AddIndentation(GenerateGetEndpoint(table), 1));

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
            builder.AppendLine("from WebSocketHelpers import checkArguments, removeClosedConnection");
            builder.AppendLine($"import {table.SingularName}");

            builder.AppendLine("async def requestReceived(websocket, session, request):");
            //endpoints
            functionBuilder.AppendLine(GenerateEndpoints(table));
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
