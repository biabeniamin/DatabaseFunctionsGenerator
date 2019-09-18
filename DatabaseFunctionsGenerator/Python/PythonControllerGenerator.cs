﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonControllerGenerator : IGenerator
    {
        private Database _database;

        public PythonControllerGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateForeignKeyFields(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Foreign Fields");

            foreach (Column column in table.Columns.Where(col => col.Type.IsForeignKey))
            {
                builder.Append($"{column.LowerCaseName} = Column({column.Type.GetSqlAlchemyType()}");

                builder.Append($", ForeignKey(\"{column.ParentTable.Name}.{column.ParentTable.PrimaryKeyColumn}\")");
                builder.AppendLine(")");

                builder.Append($"{column.ParentTable.LowerCaseName} = relationship({column.ParentTable.Name},");
                builder.Append($"backref = backref('{table.Name}'))");
            }

            return builder.ToString();
        }

        private string GenerateFields(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Fields");

            foreach(Column column in table.Columns.Where(col => !col.Type.IsForeignKey))
            {
                builder.Append($"{column.LowerCaseName} = Column({column.Type.GetSqlAlchemyType()}");

                if (column.Type.IsPrimaryKey)
                {
                    builder.Append(", primary_key=True");
                }

                builder.AppendLine(")");
            }
            builder.AppendLine(GenerateForeignKeyFields(table));

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
            builder.AppendLine("from sqlalchemy.orm import backref, relationship");
            builder.AppendLine("from ValidationError import ValidationError");
            builder.AppendLine("from sqlalchemy.orm import validates");
            builder.AppendLine("from SqlAlchemy import Base");
            builder.AppendLine("from sqlalchemy.ext.declarative import declared_attr");
            builder.AppendLine("from sqlalchemy import *");
            builder.AppendLine("from sqlalchemy.dialects.mysql import DOUBLE");
            //import parent tables
            foreach (Table parentTable in table.Parents)
                builder.AppendLine($"from {parentTable.Name} import {parentTable.Name}");

            builder.AppendLine($"class {table.Name}(Base):");
            //class content
            {
                classBuilder.AppendLine("@declared_attr");
                classBuilder.AppendLine("def __tablename__(cls):");
                classBuilder.AppendLine("\treturn cls.__name__.lower()");

                //generate fields
                classBuilder.AppendLine(GenerateFields(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                     1));
            }


            IO.WriteFile($"{path}\\{table.Name}.py", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string controllersPath;

            controllersPath = $"{path}\\Controllers";

            IO.CreateDirectory(controllersPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, controllersPath);
            }
        }
    }
}
