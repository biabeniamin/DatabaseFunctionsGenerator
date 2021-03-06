﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonFlaskRestlessInstanceGenerator : IGenerator
    {
        private Database _database;

        public PythonFlaskRestlessInstanceGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");
            builder.AppendLine("import flask");
            builder.AppendLine("import flask_restless");
            builder.AppendLine("from sqlalchemy.ext.declarative import declarative_base, declared_attr");
            builder.AppendLine("from sqlalchemy.orm import sessionmaker, scoped_session, validates");
            builder.AppendLine("from sqlalchemy import *");
            builder.AppendLine("from SqlAlchemy import Base, engine");
            builder.AppendLine("from ValidationError import ValidationError");
            foreach (Table table in _database.Tables)
                builder.AppendLine($"from {table.Name} import {table.Name}");
            builder.AppendLine();

            //add cors function

            builder.AppendLine("def add_cors_headers(response):");
            builder.AppendLine("\tresponse.headers['Access-Control-Allow-Origin'] = '*'");
            builder.AppendLine("\tresponse.headers['Access-Control-Allow-Credentials'] = 'true'");
            builder.AppendLine("\tresponse.headers['Access-Control-Allow-Headers'] = '*'");
            builder.AppendLine("\tresponse.headers['Access-Control-Allow-Methods'] = '*'");
            //Access-Control-Allow-Headers
            builder.AppendLine("\treturn response");
            builder.AppendLine();

            //create session
            builder.AppendLine("app = flask.Flask(__name__)");
            builder.AppendLine("app.after_request(add_cors_headers)");
            builder.AppendLine("Session = sessionmaker(bind = engine, autocommit = False, autoflush = False)");
            builder.AppendLine("s = scoped_session(Session)");
            builder.AppendLine("manager = flask_restless.APIManager(app, session = s)");

            foreach(Table table in _database.Tables)
            {
                builder.AppendLine($"manager.create_api({table.Name},");
                builder.AppendLine("\tmethods =['GET', 'PUT', 'POST', 'DELETE'], validation_exceptions=[ValidationError], results_per_page=-1)");
            }
            builder.AppendLine();

            //create database
            builder.AppendLine("Base.metadata.bind = engine");
            builder.AppendLine("Base.metadata.create_all()");

            //start the server
            builder.AppendLine("app.run(debug=True, host='0.0.0.0', port=5000)");

            IO.WriteFile($"{path}\\flaskRestless.py", (builder.ToString()));
        }
    }
}
