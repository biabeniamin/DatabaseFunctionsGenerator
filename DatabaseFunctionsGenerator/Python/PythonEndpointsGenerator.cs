using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonEndpointsGenerator : IGenerator
    {
        private Database _database;

        public PythonEndpointsGenerator(Database database)
        {
            _database = database;
        }


        private string GenerateGetEndpoint(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#get endpoint");

            builder.AppendLine($"def get(self):");
            function.AppendLine($"requestedArgs = getArguments(['cmd', '{table.ToString("', '", true)}'])");
            function.AppendLine($"args  = requestedArgs.parse_args()");

            //dedicated requests
            foreach(DedicatedGetRequest request in table.DedicatedGetRequests)
            {
                function.AppendLine($"if args['cmd'] == 'get{table.Name}By{request.ToString("")}':");
                function.AppendLine($"\treturn {table.SingularName}.get{table.Name}By{request.ToString("")}(session, args['{request.ToString(", args['",true)}'])");
            }

            function.AppendLine($"return {table.SingularName}.get{table.Name}(session)");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GeneratePostEndpoint(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#post endpoint");

            builder.AppendLine($"def post(self):");
            function.AppendLine($"requestedArgs = getArguments(['{table.ToString("', '", true, onlyEditable:true)}'])");
            function.AppendLine($"args  = requestedArgs.parse_args()");
            function.AppendLine($"{table.LowerCaseSingularName}  = dict_as_obj(args, {table.SingularName}.{table.SingularName}())");
            function.AppendLine($"return [{table.SingularName}.add{table.SingularName}(session, {table.LowerCaseSingularName})]");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateDeleteEndpoint(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#delete endpoint");

            builder.AppendLine($"def delete(self):");
            function.AppendLine($"requestedArgs = getArguments(['{table.PrimaryKeyColumn.LowerCaseName}'])");
            function.AppendLine($"args  = requestedArgs.parse_args()");
            function.AppendLine($"return [{table.SingularName}.delete{table.SingularName}(session, args['{table.PrimaryKeyColumn.LowerCaseName}'])]");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GeneratePatchEndpoint(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#patch endpoint");

            builder.AppendLine($"def patch(self):");
            function.AppendLine($"requestedArgs = getArguments(['{table.ToString("', '", true, onlyEditable: true)}'])");
            function.AppendLine($"args  = requestedArgs.parse_args()");
            function.AppendLine($"{table.LowerCaseSingularName}  = dict_as_obj(args, {table.SingularName}.{table.SingularName}())");
            function.AppendLine($"return []");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateAPIEndpoints(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#API endpoints");
            builder.AppendLine(GenerateGetEndpoint(table));
            builder.AppendLine(GeneratePostEndpoint(table));
            builder.AppendLine(GenerateDeleteEndpoint(table));
            builder.AppendLine(GeneratePatchEndpoint(table));

            return builder.ToString();
        }

        private void GenerateController(Table table, string path)
        {
            StringBuilder builder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            classBuilder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");
            builder.AppendLine("from flask_restful import Resource");
            builder.AppendLine("from SqlAlchemy import dict_as_obj");
            builder.AppendLine("from SqlAlchemyMain import *");
            builder.AppendLine("from FlaskRestfulHelpers import getArguments");
            builder.AppendLine($"import {table.SingularName}");

            builder.AppendLine($"class {table.SingularName}List(Resource):");
            //endpoints
            classBuilder.AppendLine(GenerateAPIEndpoints(table));
            builder.AppendLine(Helpers.AddIndentation(classBuilder, 1));

            IO.WriteFile($"{path}\\{table.SingularName}Endpoints.py", (builder.ToString()));
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

