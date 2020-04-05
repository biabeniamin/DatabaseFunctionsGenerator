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
            builder.AppendLine();

            builder.AppendLine("Session = sessionmaker(bind = engine, autocommit = False, autoflush = False)");
            builder.AppendLine("session = scoped_session(Session)");

            //create database
            builder.AppendLine("def createDatabase():");
            builder.AppendLine("\tBase.metadata.bind = engine");
            builder.AppendLine("\tBase.metadata.create_all()");

            //create database on main
            builder.AppendLine("if __name__ == '__main__':");
            builder.AppendLine("\tcreateDatabase()");

            IO.WriteFile($"{path}\\SqlAlchemyMain.py", (builder.ToString()));
        }
    }
}
