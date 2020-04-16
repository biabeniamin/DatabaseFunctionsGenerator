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

        private string GenerateFields(Table table, bool includePrimaryKey = true, bool includeParentFields = true)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //generate fields
            foreach (Column column in table.Columns)
            {
                if (!includePrimaryKey && column.Type.IsPrimaryKey)
                    continue;
                builder.AppendLine($"{Helpers.GetLowerCaseString(column.Name)} : {column.Type.GetTypescriptType()};");
            }

            if (includeParentFields)
            {
                foreach (Table parentTable in table.Parents)
                {
                    if(_database.Type == Models.DatabaseType.Php)
                        builder.AppendLine($"{parentTable.LowerCaseSingularName} : {parentTable.SingularName};");
                    else if (_database.Type == Models.DatabaseType.PhytonFlaskRestless)
                        builder.AppendLine($"{parentTable.LowerCaseName} : {parentTable.SingularName};");
                }
            }


            return builder.ToString();
        }

        /*
         export function encodeMessage(message: Message): MessageJSON {
	return {
	  content:    message.content,
	  source:     message.source,
	  creationTime: message.creationTime
	};
  }

             */

        private string GenerateConvertFunction(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine($"export function encode{table.SingularName}({table.LowerCaseSingularName}: {table.SingularName}): {table.SingularName}JSON {{");
            {
                builder.AppendLine($"\treturn {{");
                //generate fields
                foreach (Column column in table.Columns.Where(col => !col.Type.IsPrimaryKey))
                {
                    builder.AppendLine($"\t\t{column.LowerCaseName}:\t{table.LowerCaseSingularName}.{column.LowerCaseName},");
                }

                builder.AppendLine($"\t}}");
            }
            builder.AppendLine($"}}");

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
                builder.AppendLine($"import {{ {parentTable.SingularName} }} from './/{parentTable.SingularName}'");
            }

            //generate main model
            builder.AppendLine($"export interface {table.SingularName}");
            builder.AppendLine("{");
            {

                builder.Append(Helpers.AddIndentation(GenerateFields(table),
                    1));

                builder.AppendLine("}");
            }
            builder.AppendLine();

            //generate json model
            builder.AppendLine($"export interface {table.SingularName}JSON");
            builder.AppendLine("{");
            {

                builder.Append(Helpers.AddIndentation(GenerateFields(table,false, false),
                    1));

                builder.AppendLine("}");
            }
            builder.AppendLine();

            builder.AppendLine(GenerateConvertFunction(table));

            IO.WriteFile($"{path}\\{table.SingularName}.ts", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Models";

            IO.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateModel(table, modelsPath);
            }
        }
    }
}
