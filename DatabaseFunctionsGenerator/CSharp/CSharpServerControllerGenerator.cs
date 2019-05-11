using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.CSharp
{
    public class CSharpServerControllerGenerator : IGenerator
    {
        private Database _database;

        public CSharpServerControllerGenerator(Database database)
        {
            _database = database;
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
            builder.AppendLine("using Newtonsoft.Json;");
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Collections.ObjectModel;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Threading.Tasks; ");


            builder.AppendLine("namespace DatabaseFunctionsGenerator");
            builder.AppendLine("{");
            {

                namespaceBuilder.AppendLine($"public class {table.SingularName}");
                namespaceBuilder.AppendLine("{");
                {
                    classBuilder.AppendLine(GenerateFields(table));
                    classBuilder.AppendLine(GenerateGettersSetters(table));
                    classBuilder.AppendLine(GenerateConstructor(table));

                    if (0 < table.Parents.Count)
                    {
                        classBuilder.AppendLine(GenerateConstructorWithParents(table));
                    }

                    classBuilder.AppendLine(GenerateEmptyConstructor(table));

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
