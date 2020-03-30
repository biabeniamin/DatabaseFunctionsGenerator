﻿using System;
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
            function.AppendLine($"return {table.SingularName}.get{table.Name}(session)");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateAPIEndpoints(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#API endpoints");
            builder.AppendLine(GenerateGetEndpoint(table));

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
            builder.AppendLine("from SqlAlchemyMain import *");
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

