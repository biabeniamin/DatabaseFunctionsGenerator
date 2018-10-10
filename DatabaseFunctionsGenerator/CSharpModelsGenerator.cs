using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpModelsGenerator
    {
        private Database _database;

        public CSharpModelsGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateFields(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //generate fields
            foreach (Column column in table.Columns)
            {
                builder.AppendLine($"private {column.Type.GetCSharpType()} _{Helpers.GetLowerCaseString(column.Name)};");
            }

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"private {parentTable.SingularName} _{parentTable.LowerCaseSingularName};");
            }


            return builder.ToString();
        }

        private string GenerateGettersSetters(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //generate getters and setters
            foreach (Column column in table.Columns)
            {
                //getter
                builder.AppendLine($"public {column.Type.GetCSharpType()} {column.Name}");
                builder.AppendLine("{");
                builder.AppendLine("\tget");
                builder.AppendLine("\t{");
                {
                    builder.AppendLine($"\t\treturn _{column.LowerCaseName};");
                }
                builder.AppendLine("\t}");

                //set
                builder.AppendLine("\tset");
                builder.AppendLine("\t{");
                {
                    builder.AppendLine($"\t\t_{column.LowerCaseName} = value;");
                }
                builder.AppendLine("\t}");
                builder.AppendLine("}");

                builder.AppendLine();
            }

            foreach (Table parentTable in table.Parents)
            {
                //getter
                builder.AppendLine($"public {parentTable.SingularName} {parentTable.SingularName}()");
                builder.AppendLine("{");
                builder.AppendLine("\tget");
                builder.AppendLine("\t{");
                {
                    builder.AppendLine($"\t\treturn _{parentTable.SingularName};");
                }
                builder.AppendLine("\t}");

                //set
                builder.AppendLine("\tset");
                builder.AppendLine("\t{");
                {
                    builder.AppendLine($"\t\t_{parentTable.LowerCaseSingularName} = value;");
                }
                builder.AppendLine("\t}");
                builder.AppendLine("}");

                builder.AppendLine();
            }


            return builder.ToString();
        }

        private string GenerateConstructor(Table table)
        {
            StringBuilder builder;
            StringBuilder columnsCommaSeparated;

            builder = new StringBuilder();
            columnsCommaSeparated = new StringBuilder();

            //generate columnsCommaSeparated
            foreach (Column column in table.EditableColumns)
            {
                columnsCommaSeparated.Append($"${column.Name}, ");
            }

            if (1 < columnsCommaSeparated.Length)
            {
                columnsCommaSeparated = columnsCommaSeparated.Remove(columnsCommaSeparated.Length - 2, 2);
            }

            builder.AppendLine($"function {table.SingularName}({columnsCommaSeparated.ToString()})");
            builder.AppendLine("{");

            foreach (Column column in table.EditableColumns)
            {
                builder.AppendLine($"\t$this->{Helpers.GetLowerCaseString(column.Name)} = ${column.Name};");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateModel(Table table, string path)
        {
            StringBuilder builder;
            StringBuilder namespaceBuilder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            namespaceBuilder = new StringBuilder();
            classBuilder = new StringBuilder();

            builder.AppendLine("//generated automatically");
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
                    /*classBuilder.AppendLine(Helpers.AddIndentation(GenerateConstructor(table),
                        1));*/

                    namespaceBuilder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(), 
                        1));
                }
                namespaceBuilder.AppendLine("}");
                builder.AppendLine(Helpers.AddIndentation(namespaceBuilder.ToString(),
                    1));
            }
            builder.AppendLine("}");

            Helpers.WriteFile($"{path}\\{table.SingularName}.cs", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Models";

            Directory.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateModel(table, modelsPath);
            }
        }
    }
}
