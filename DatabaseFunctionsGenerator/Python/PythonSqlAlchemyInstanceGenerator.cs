using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonSqlAlchemyInstanceGenerator
    {
        private Database _database;

        public PythonSqlAlchemyInstanceGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder = new StringBuilder();

            //imports
            builder.AppendLine("#generated automatically");
            builder.AppendLine("from sqlalchemy.ext.declarative import declarative_base, declared_attr");
            builder.AppendLine("from sqlalchemy.orm import sessionmaker, scoped_session, validates");
            builder.AppendLine("from sqlalchemy import *");
            builder.AppendLine("from SqlAlchemy import Base, engine");
            builder.AppendLine("from ValidationError import ValidationError");
            foreach (Table table in _database.Tables)
                builder.AppendLine($"from {table.Name} import {table.Name}");
            builder.AppendLine();

            builder.AppendLine("Session = sessionmaker(bind = engine, autocommit = False, autoflush = False)");
            builder.AppendLine("s = scoped_session(Session)");

            //create database
            builder.AppendLine("Base.metadata.bind = engine");
            builder.AppendLine("Base.metadata.create_all()");

            IO.WriteFile($"{path}\\SqlAlchemyMain.py", (builder.ToString()));
        }
    }
}
