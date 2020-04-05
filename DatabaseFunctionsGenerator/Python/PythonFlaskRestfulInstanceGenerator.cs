using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonFlaskRestfulInstanceGenerator : IGenerator
    {
        private Database _database;

        public PythonFlaskRestfulInstanceGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");
            builder.AppendLine("from flask import Flask, make_response");
            builder.AppendLine("from flask_restful import Api");
            builder.AppendLine("from SqlAlchemyMain import createDatabase, session");
            builder.AppendLine("from SqlAlchemy import convertToJson");
            if(_database.HasAuthenticationSystem)
                builder.AppendLine("import Authentication");
            foreach (Table table in _database.Tables)
                builder.AppendLine($"from {table.SingularName}Endpoints import {table.SingularName}Endpoints");
            builder.AppendLine();

           //create session
            builder.AppendLine("app = Flask(__name__)");
            builder.AppendLine("api = Api(app)");
            builder.AppendLine("createDatabase()");
            builder.AppendLine();

            foreach (Table table in _database.Tables)
            {
                builder.AppendLine($"api.add_resource({table.SingularName}Endpoints, '/{table.LowerCaseName}', resource_class_kwargs ={{ 'session' : session}}) ");
            }
            builder.AppendLine();

            //start the server
            builder.AppendLine("if __name__ == '__main__':");
            builder.AppendLine("\tapp.run(debug=True, host='0.0.0.0', port=5000)");

            IO.WriteFile($"{path}\\FlaskRestful.py", (builder.ToString()));
        }
    }
}
