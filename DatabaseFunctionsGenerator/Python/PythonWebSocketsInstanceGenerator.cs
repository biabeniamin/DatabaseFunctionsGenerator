using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonWebSocketsInstanceGenerator : IGenerator
    {
        private Database _database;

        public PythonWebSocketsInstanceGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateRequestHandle()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("request = json.loads(requestJson)");
            builder.AppendLine("print(request)");

            for (int i = 0; i < _database.Tables.Count; i++)
            {
                Table table = _database.Tables[i];

                //generate else if
                if (i > 0)
                    builder.Append("el");

                builder.AppendLine($"if request['table'] == '{table.Name}':");
                builder.AppendLine($"\tawait {table.SingularName}WebSockets.requestReceived(websocket, session, request)");
            }

            return builder.ToString();
        }

        public void Generate(string path)
        {
            StringBuilder builder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");
            builder.AppendLine("import asyncio");
            builder.AppendLine("import websockets");
            builder.AppendLine("import json");
            builder.AppendLine("from SqlAlchemyMain import session");
            
            foreach (Table table in _database.Tables)
                builder.AppendLine($"import {table.SingularName}WebSockets");
            builder.AppendLine();

            builder.AppendLine("users = set()");

            builder.AppendLine("async def requestReceived(websocket, path):");
            {
                StringBuilder functionBuilder = new StringBuilder();

                functionBuilder.AppendLine("users.add(websocket)");
                functionBuilder.AppendLine("try:");
                functionBuilder.AppendLine("\tprint('client connected')");
                functionBuilder.AppendLine("\tasync for requestJson in websocket:");
                {
                    functionBuilder.AppendLine(Helpers.AddIndentation(GenerateRequestHandle(), 2));
                }
                functionBuilder.AppendLine("finally:");
                functionBuilder.AppendLine("\tprint('client disconnected')");
                functionBuilder.AppendLine("\tusers.remove(websocket)");
                functionBuilder.AppendLine();

                builder.AppendLine(Helpers.AddIndentation(functionBuilder, 1));
            }

            //start the server
            builder.AppendLine("start_server = websockets.serve(requestReceived, 'localhost', 6789)");
            builder.AppendLine("asyncio.get_event_loop().run_until_complete(start_server)");
            builder.AppendLine("asyncio.get_event_loop().run_forever()");

            IO.WriteFile($"{path}\\WebSockets.py", (builder.ToString()));
        }
    }
}
