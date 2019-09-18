using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonFlaskInstanceGenerator : IGenerator
    {
        private Database _database;

        public PythonFlaskInstanceGenerator(Database database)
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
            builder.AppendLine("from ValidationError import ValidationError");
            builder.AppendLine("from SqlAlchemy import Base, engine");

            IO.WriteFile($"{path}\\main.py", (builder.ToString()));
        }
    }
}
