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

                builder.Append($"{column.ParentTable.LowerCaseName} = relationship({column.ParentTable.SingularName},");
                builder.AppendLine($"backref = backref('{table.LowerCaseName}'))");
                builder.AppendLine($"{column.ParentTable.LowerCaseSingularName} = null");
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

        private string GenerateCompleteParentFunctions(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();

            foreach (Column column in table.ForeignKeyColumns)
            {
                Table parentTable = column.ParentTable;
                function = new StringBuilder();

                builder.AppendLine($"#complete {parentTable.LowerCaseName} funtion");

                builder.AppendLine($"def complete{parentTable.Name}(session, {table.LowerCaseName}):");

                function.AppendLine($"{parentTable.LowerCaseName} = get{parentTable.Name}(session)");
                function.AppendLine($"for row in {table.LowerCaseName}:");
                function.AppendLine($"\tstart = 0");
                function.AppendLine($"\tend = len({parentTable.LowerCaseName})");
                function.AppendLine($"\twhile True:");
                {
                    StringBuilder whileBlock = new StringBuilder();

                    whileBlock.AppendLine("mid = floor((start + end) / 2)");
                    whileBlock.AppendLine("if(row.phoneId > phones[mid].phoneId):");
                    whileBlock.AppendLine("\tstart = mid + 1");

                    whileBlock.AppendLine("elif(row.phoneId < phones[mid].phoneId):");
                    whileBlock.AppendLine("\tend = mid - 1");

                    whileBlock.AppendLine("elif(row.phoneId == phones[mid].phoneId):");
                    whileBlock.AppendLine("\tstart = mid + 1");
                    whileBlock.AppendLine("\tend = mid - 1");
                    whileBlock.AppendLine($"\trow.{parentTable.LowerCaseSingularName} = {parentTable.LowerCaseName}[mid]");
                    whileBlock.AppendLine();

                    whileBlock.AppendLine("if(start > end):");
                    whileBlock.AppendLine("\tbreak");


                    function.AppendLine(Helpers.AddIndentation(whileBlock, 2));
                }
                function.AppendLine($"return {table.LowerCaseName}");

                builder.AppendLine(Helpers.AddIndentation(function, 1));
            }

            return builder.ToString();
        }

        private string GenerateGetFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#get funtion");

            builder.AppendLine($"def get{table.Name}(session):");

            function.AppendLine($"result = session.query({table.SingularName}).all()");

            foreach(Column column in table.ForeignKeyColumns)
            {
                function.AppendLine($"result = complete{column.ParentTable.Name}(session, result)");
            }

            function.AppendLine($"return result");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateGetDedicatedRequestFunctions(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#get dedicated request funtions");
            foreach (DedicatedGetRequest request in table.DedicatedGetRequests)
            {
                StringBuilder function;

                function = new StringBuilder();

                builder.AppendLine($"def get{table.Name}By{request.ToString("")}(session, {request.ToString(", ", true)}):");

                function.Append($"result = session.query({table.Name}).filter(");
                foreach(Column column in request.Columns)
                    function.Append($"{table.Name}.{column.LowerCaseName} == {column.LowerCaseName}, ");
                Helpers.RemoveLastApparition(function, ", ");

                function.AppendLine($").all()");

                foreach (Column column in table.ForeignKeyColumns)
                {
                    function.AppendLine($"result = complete{column.ParentTable.Name}(session, result)");
                }

                function.AppendLine($"return result");

                builder.AppendLine(Helpers.AddIndentation(function, 1));
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
            function.AppendLine($"{table.LowerCaseSingularName}.{table.CreationTimeColumn.LowerCaseName} = datetime.datetime.utcnow()");
            function.AppendLine($"session.add({table.LowerCaseSingularName})");
            function.AppendLine($"session.commit()");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateUpdateFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#update funtion");

            builder.AppendLine($"def update{table.SingularName}(session, {table.LowerCaseSingularName}):");
            function.AppendLine($"result = session.query({table.Name}).filter({table.Name}.{table.PrimaryKeyColumn.LowerCaseName} == {table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}).first()");
            function.AppendLine($"result = {table.LowerCaseSingularName}");
            function.AppendLine($"session.commit()");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateDeleteFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#delete funtion");

            builder.AppendLine($"def delete{table.SingularName}(session, {table.LowerCaseSingularName}):");
            function.AppendLine($"result = session.query({table.Name}).filter({table.Name}.{table.PrimaryKeyColumn.LowerCaseName} == {table.LowerCaseSingularName}.{table.PrimaryKeyColumn.LowerCaseName}).first()");
            function.AppendLine($"session.delete(result)");
            function.AppendLine($"session.commit()");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateRequestParserArgumentsFunction(Table table)
        {
            StringBuilder builder;
            StringBuilder function;

            builder = new StringBuilder();
            function = new StringBuilder();

            builder.AppendLine("#request parser funtion");

            builder.AppendLine($"def get{table.LowerCaseSingularName}RequestArguments():");
            function.AppendLine($"parser = reqparse.RequestParser()");
            foreach (Column column in table.EditableColumns)
            {
                function.AppendLine($"parser.add_argument('{column.LowerCaseName}')");
            }
            function.AppendLine($"return parser");

            builder.AppendLine(Helpers.AddIndentation(function, 1));

            return builder.ToString();
        }

        private string GenerateFunctions(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#Functions");

            builder.AppendLine(GenerateCompleteParentFunctions(table));
            builder.AppendLine(GenerateGetFunction(table));
            builder.AppendLine(GenerateGetDedicatedRequestFunctions(table));
            builder.AppendLine(GenerateAddFunction(table));
            builder.AppendLine(GenerateUpdateFunction(table));
            builder.AppendLine(GenerateDeleteFunction(table));

            return builder.ToString();
        }

        private string GenerateAPIEndpoints(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("#API endpoints");
            builder.AppendLine(GenerateRequestParserArgumentsFunction(table));

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
            builder.AppendLine("from flask_restful import reqparse");
            builder.AppendLine("import datetime");
            builder.AppendLine("from math import floor");
            //import parent tables
            foreach (Table parentTable in table.Parents)
                builder.AppendLine($"from {parentTable.SingularName} import {parentTable.SingularName}, get{parentTable.Name}");

            builder.AppendLine($"class {table.SingularName}(Base):");
            //class content
            {
                classBuilder.AppendLine("@declared_attr");
                classBuilder.AppendLine("def __tablename__(cls):");
                classBuilder.AppendLine($"\treturn '{table.LowerCaseName}'");

                //generate fields
                classBuilder.AppendLine(GenerateFields(table));

                //validation
                classBuilder.AppendLine(GenerateValidations(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                     1));
            }

            //functions
            builder.AppendLine(GenerateFunctions(table));
            //endpoints
            builder.AppendLine(GenerateAPIEndpoints(table));


            IO.WriteFile($"{path}\\{table.SingularName}.py", (builder.ToString()));

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
