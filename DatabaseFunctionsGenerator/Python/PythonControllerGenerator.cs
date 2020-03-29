using System;
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

                builder.Append($", ForeignKey(\"{column.ParentTable.LowerCaseName}.{column.ParentTable.PrimaryKeyColumn.LowerCaseName}\")");
                builder.AppendLine(")");

                builder.Append($"{column.ParentTable.LowerCaseName} = relationship({column.ParentTable.Name},");
                builder.AppendLine($"backref = backref('{table.LowerCaseName}'))");
            }

            return builder.ToString();
        }

        private string GenerateFields(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Fields");

            foreach (Column column in table.Columns.Where(col => !col.Type.IsForeignKey))
            {
                builder.Append($"{column.LowerCaseName} = Column({column.Type.GetSqlAlchemyType()}");

                if (column.Type.IsPrimaryKey)
                {
                    builder.Append(", primary_key=True");
                }

                if (column == table.CreationTimeColumn)
                    builder.Append(", default=datetime.datetime.utcnow");

                builder.AppendLine(")");
            }
            builder.AppendLine(GenerateForeignKeyFields(table));

            return builder.ToString();
        }

        private string GenerateValidations(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Validation");

            foreach (Column column in table.Columns.Where(col => col.Type.Type == Types.Integer && !col.Type.IsPrimaryKey))
            {
                builder.AppendLine($"@validates('{column.LowerCaseName}')");
                builder.AppendLine($"def validate_{column.LowerCaseName}(self, key, value):");
                builder.AppendLine($"\treturn validate_integer(key, value)");

            }

            return builder.ToString();
        }

        private string GenerateAddFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder(); 

            builder.AppendLine("#add funtion");

            builder.AppendLine($"def add{table.SingularName}(session, {table.LowerCaseSingularName}):");
            function.AppendLine($"session.add({table.LowerCaseSingularName})");
            function.AppendLine($"session.commit()");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateFunctions(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Functions");

            builder.AppendLine(GenerateAddFunction(table)); 

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
            builder.AppendLine("from sqlalchemy.orm import validates");
            builder.AppendLine("from SqlAlchemy import Base");
            builder.AppendLine("from sqlalchemy.ext.declarative import declared_attr");
            builder.AppendLine("from sqlalchemy import *");
            builder.AppendLine("from sqlalchemy.dialects.mysql import DOUBLE");
            builder.AppendLine("from ValidationError import ValidationError, validate_integer");
            builder.AppendLine("import datetime");
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

                //validation
                classBuilder.AppendLine(GenerateValidations(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                     1));
            }

            //functions
            builder.AppendLine(GenerateFunctions(table));


            IO.WriteFile($"{path}\\{table.Name}.py", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string controllersPath;

            controllersPath = $"{path}";

            IO.CreateDirectory(controllersPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, controllersPath);
            }
        }
    }
}
