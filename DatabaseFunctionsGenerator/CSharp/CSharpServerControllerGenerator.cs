﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpServerControllerGenerator : IGenerator
    {
        private Database _database;

        public CSharpServerControllerGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateGetRequest(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine($"// GET {table.LowerCaseName}/values");
            builder.AppendLine($"public IEnumerable<{table.SingularName}> Get()");
            builder.AppendLine("{");
            {
                StringBuilder blockBuilder = new StringBuilder();

                blockBuilder.AppendLine($"MySqlDataReader reader = new DatabaseOperations().GetReader(\"SELECT * FROM {table.Name}\");");
                blockBuilder.AppendLine($"List<{table.SingularName}> list = new List<{table.SingularName}>();");
                blockBuilder.AppendLine();

                blockBuilder.AppendLine("while(reader.Read())");
                blockBuilder.AppendLine("{");
                {
                    blockBuilder.AppendLine($"\tlist.Add(new {table.SingularName}(");

                    foreach(Column column in table.EditableColumns)
                    {
                        blockBuilder.AppendLine($"\t\t({column.Type.GetCSharpType()})reader[\"{column.Name}\"],");
                    }
                    Helpers.RemoveLastApparition(blockBuilder, ",");

                    blockBuilder.AppendLine($"\t));");
                }
                blockBuilder.AppendLine("}");

                blockBuilder.AppendLine("");
                blockBuilder.AppendLine("return list;");
                builder.Append(Helpers.AddIndentation(blockBuilder, 1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateController(Table table, string path)
        {
            StringBuilder builder;
            StringBuilder namespaceBuilder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            namespaceBuilder = new StringBuilder();
            classBuilder = new StringBuilder();

            builder.AppendLine("//generated automatically");
            builder.AppendLine("using MySql.Data.MySqlClient;");
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Collections.ObjectModel;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Net;");
            builder.AppendLine("using System.Net.Http;");


            builder.AppendLine("namespace DatabaseFunctionsGenerator");
            builder.AppendLine("{");
            {

                namespaceBuilder.AppendLine($"public class {table.SingularName}Controller : ApiController");
                namespaceBuilder.AppendLine("{");
                {
                    classBuilder.AppendLine(GenerateGetRequest(table));
                    

                    namespaceBuilder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
                }
                namespaceBuilder.AppendLine("}");
                builder.AppendLine(Helpers.AddIndentation(namespaceBuilder.ToString(),
                    1));
            }
            builder.AppendLine("}");

            IO.WriteFile($"{path}\\{table.SingularName}.cs", (builder.ToString()));

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