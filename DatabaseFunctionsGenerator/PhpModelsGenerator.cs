using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class PhpModelsGenerator
    {
        private Database _database;

        public PhpModelsGenerator(Database database)
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
                builder.AppendLine($"var ${column.Name};");
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
                builder.AppendLine($"\treturn $this->{column.Name};");
                builder.AppendLine("}");

                //setter
                builder.AppendLine($"function Set{column.Name}($value)");
                builder.AppendLine("{");
                builder.AppendLine($"\t$this->{column.Name} = $value;");
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
            foreach (Column column in table.Columns)
            {
                columnsCommaSeparated.Append($"${column.Name}, ");
            }

            if (1 < columnsCommaSeparated.Length)
            {
                columnsCommaSeparated = columnsCommaSeparated.Remove(columnsCommaSeparated.Length - 2, 2);
            }

            builder.AppendLine($"function {table.SingularName}({columnsCommaSeparated.ToString()})");
            builder.AppendLine("{");

            foreach (Column column in table.Columns)
            {
                builder.AppendLine($"\t$this->{column.Name} = ${column.Name};");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateModel(Table table, string path)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("<?php");
            builder.AppendLine("//generated automatically");

            builder.AppendLine($"class {table.SingularName}");
            builder.AppendLine("{");

            builder.AppendLine(Helpers.AddIndentation(GenerateFields(table),
                1));
            builder.AppendLine(Helpers.AddIndentation(GenerateGettersSetters(table),
                1));
            builder.AppendLine(Helpers.AddIndentation(GenerateConstructor(table),
                1));

            builder.AppendLine("}");
            builder.AppendLine("?>");

            Helpers.WriteFile($"{path}\\{table.SingularName}.php", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Models";

            Directory.CreateDirectory(modelsPath);

            foreach(Table table in _database.Tables)
            {
                GenerateModel(table, modelsPath);
            }
        }
    }
}
