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

            //create session
            builder.AppendLine("async def requestReceived(websocket, path):");
            {
                StringBuilder functionBuilder = new StringBuilder();

                functionBuilder.AppendLine("try:");
                functionBuilder.AppendLine("\tprint('client connected')");
                functionBuilder.AppendLine("finally:");
                functionBuilder.AppendLine("\tprint('client disconnected')");
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
