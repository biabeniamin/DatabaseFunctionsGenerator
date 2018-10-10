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
                builder.AppendLine($"function Get{column.Name}()");
                builder.AppendLine("{");
                builder.AppendLine($"\treturn $this->{Helpers.GetLowerCaseString(column.Name)};");
                builder.AppendLine("}");

                //setter
                builder.AppendLine($"function Set{column.Name}($value)");
                builder.AppendLine("{");
                builder.AppendLine($"\t$this->{Helpers.GetLowerCaseString(column.Name)} = $value;");
                builder.AppendLine("}");

                builder.AppendLine();
            }

            foreach (Table parentTable in table.Parents)
            {
                //getter
                builder.AppendLine($"function Get{parentTable.SingularName}()");
                builder.AppendLine("{");
                builder.AppendLine($"\treturn $this->{parentTable.LowerCaseSingularName};");
                builder.AppendLine("}");

                //setter
                builder.AppendLine($"function Set{parentTable.SingularName}($value)");
                builder.AppendLine("{");
                builder.AppendLine($"\t$this->{parentTable.LowerCaseSingularName} = $value;");
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
                    /*classBuilder.AppendLine(Helpers.AddIndentation(GenerateGettersSetters(table),
                        1));
                    classBuilder.AppendLine(Helpers.AddIndentation(GenerateConstructor(table),
                        1));*/

                    namespaceBuilder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(), 
                        1));
                }
                namespaceBuilder.AppendLine("}");
                builder.AppendLine(Helpers.AddIndentation(namespaceBuilder.ToString(),
                    1));
            }
            builder.AppendLine("}");

            Helpers.WriteFile($"{path}\\{table.SingularName}.php", (builder.ToString()));

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
