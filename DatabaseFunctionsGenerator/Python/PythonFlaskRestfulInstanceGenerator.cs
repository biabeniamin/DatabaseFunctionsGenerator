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
                builder.AppendLine($"from {table.SingularName} import {table.SingularName}");
            builder.AppendLine();

          

            IO.WriteFile($"{path}\\FlaskRestful.py", (builder.ToString()));
        }
    }
}
