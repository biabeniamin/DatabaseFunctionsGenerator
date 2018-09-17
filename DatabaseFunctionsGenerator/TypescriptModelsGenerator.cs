using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptModelsGenerator
    {
        private Database _database;

        public TypescriptModelsGenerator(Database database)
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
                builder.AppendLine($"{Helpers.GetLowerCaseString(column.Name)} : {column.Type.GetTypescriptType()};");
            }

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"{parentTable.LowerCaseSingularName} : {parentTable.SingularName};");
            }


            return builder.ToString();
        }
        
        private void GenerateModel(Table table, string path)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine("//generated automatically");
            builder.AppendLine("import { log } from 'util';");
            builder.AppendLine("import { Injectable } from '@angular/core'");

            foreach (Table parentTable in table.Parents)
            {
                //builder.AppendLine($"require '{parentTable.SingularName}.php';");
            }

            builder.AppendLine($"export interface {table.SingularName}");
            builder.AppendLine("{");
            {

                builder.AppendLine(Helpers.AddIndentation(GenerateFields(table),
                    1));

                builder.AppendLine("}");
            }

            Helpers.WriteFile($"{path}\\{table.SingularName}.ts", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Typescript\\Models";

            Directory.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateModel(table, modelsPath);
            }
        }
    }
}
