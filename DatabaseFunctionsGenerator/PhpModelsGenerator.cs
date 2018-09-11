using System;
using System.Collections.Generic;
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

        private void GenerateModel(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("<?php");
            builder.AppendLine("//generated automatically");

            builder.AppendLine($"class {table.Name}");
            builder.AppendLine("{");

            builder.AppendLine(Helpers.AddIndentation(GenerateFields(table),
                1));
            builder.AppendLine(Helpers.AddIndentation(GenerateGettersSetters(table),
                1));

            builder.AppendLine("}");
            builder.AppendLine("?>");

            System.Windows.MessageBox.Show(builder.ToString());

            //return builder.ToString();
        }

        public void Generate()
        {
            foreach(Table table in _database.Tables)
            {
                GenerateModel(table);
            }
        }
    }
}
