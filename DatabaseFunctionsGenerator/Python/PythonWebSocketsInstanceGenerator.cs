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

            IO.WriteFile($"{path}\\WebSockets.py", (builder.ToString()));
        }
    }
}
