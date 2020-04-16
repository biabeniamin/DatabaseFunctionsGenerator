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

        public string GenerateJsonSerializer()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder function = new StringBuilder();

            builder.AppendLine("@api.representation('application/json')");
            builder.AppendLine("def output_json(data, code, headers=None):");
            {
                function.AppendLine("print(data)");
                function.AppendLine("resp = make_response(convertToJson(data), code)");
                function.AppendLine("resp.headers.extend(headers or {})");
                function.AppendLine("return resp");
            }

            builder.AppendLine(Helpers.AddIndentation(function, 1));
            return builder.ToString();
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
            builder.AppendLine("from flask_cors import CORS");
            if (_database.HasAuthenticationSystem)
            {
                builder.AppendLine("import Authentication");
                builder.AppendLine("from TokenAuthenticationEndpoints import TokenAuthenticationEndpoints");
            }
            foreach (Table table in _database.Tables)
                builder.AppendLine($"from {table.SingularName}Endpoints import {table.SingularName}Endpoints");
            builder.AppendLine();

           //create session
            builder.AppendLine("app = Flask(__name__)");
            builder.AppendLine("CORS(app)");
            builder.AppendLine("api = Api(app)");
            builder.AppendLine("createDatabase()");
            builder.AppendLine();

            foreach (Table table in _database.Tables)
            {
                builder.AppendLine($"api.add_resource({table.SingularName}Endpoints, '/{table.Name}', resource_class_kwargs ={{ 'session' : session}}) ");
            }
            if (_database.HasAuthenticationSystem)
                builder.AppendLine("api.add_resource(TokenAuthenticationEndpoints, '/TokenAuthentication', resource_class_kwargs ={ 'session' : session})");
            builder.AppendLine();

            builder.AppendLine(GenerateJsonSerializer());

            //start the server
            builder.AppendLine("if __name__ == '__main__':");
            builder.AppendLine("\tapp.run(debug=True, host='0.0.0.0', port=5000)");

            IO.WriteFile($"{path}\\FlaskRestful.py", (builder.ToString()));
        }
    }
}
