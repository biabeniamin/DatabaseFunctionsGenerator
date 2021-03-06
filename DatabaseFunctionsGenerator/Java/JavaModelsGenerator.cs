﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    public class JavaModelsGenerator : IGenerator
    {
        private Database _database;

        public JavaModelsGenerator(Database database)
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
                builder.AppendLine($"private {column.Type.GetJavaType()} {column.LowerCaseName};");
            }

            foreach (Table parentTable in table.Parents)
            {
                builder.AppendLine($"private {parentTable.SingularName} {parentTable.LowerCaseSingularName};");
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
                builder.AppendLine($"public {column.Type.GetJavaType()} get{column.Name}()");
                builder.AppendLine("{");
                {
                    builder.AppendLine($"\treturn this.{column.LowerCaseName};");
                }
                builder.AppendLine("}");

                builder.AppendLine();

                //setter
                builder.AppendLine($"public void set{column.Name}({column.Type.GetJavaType()} {column.LowerCaseName})");
                builder.AppendLine("{");
                {
                    builder.AppendLine($"\tthis.{column.LowerCaseName} = {column.LowerCaseName};");
                }
                builder.AppendLine("}");

                builder.AppendLine();
            }

            foreach (Table parentTable in table.Parents)
            {
                //getter
                builder.AppendLine($"public {parentTable.SingularName} get{parentTable.SingularName}()");
                builder.AppendLine("{");
                {
                    builder.AppendLine($"\treturn this.{parentTable.LowerCaseSingularName};");
                }
                builder.AppendLine("}");

                builder.AppendLine();

                //setter
                builder.AppendLine($"public void set{parentTable.SingularName}({parentTable.SingularName} {parentTable.LowerCaseSingularName})");
                builder.AppendLine("{");
                {
                    builder.AppendLine($"\tthis.{parentTable.LowerCaseSingularName} = {parentTable.LowerCaseSingularName};");
                }
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
                columnsCommaSeparated.Append($"{column.Type.GetJavaType()} {column.LowerCaseName}, ");
            }

            if (1 < columnsCommaSeparated.Length)
            {
                columnsCommaSeparated = columnsCommaSeparated.Remove(columnsCommaSeparated.Length - 2, 2);
            }

            builder.AppendLine($"public {table.SingularName}({columnsCommaSeparated.ToString()})");
            builder.AppendLine("{");
            {

                foreach (Column column in table.EditableColumns)
                {
                    builder.AppendLine($"\tthis.{column.LowerCaseName} = {column.LowerCaseName};");
                }

            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateEmptyConstructor(Table table)
        {
            StringBuilder builder;
            StringBuilder constructorBuilder;

            builder = new StringBuilder();
            constructorBuilder = new StringBuilder();

            builder.AppendLine($"public {table.SingularName}()");

            

            builder.AppendLine("{");
            {
                constructorBuilder.AppendLine("this(");
                {
                    foreach (Column column in table.EditableColumns)
                    {
                        constructorBuilder.AppendLine($"\t{Helpers.GetDefaultJavaColumnData(column.Type.Type)}, //{column.Name}");
                    }
                    Helpers.RemoveLastApparition(constructorBuilder, ",");
                }
                constructorBuilder.AppendLine(");");

                foreach (Column column in table.NonEditableColumns)
                {
                    constructorBuilder.AppendLine($"this.{column.LowerCaseName} = {Helpers.GetDefaultJavaColumnData(column.Type.Type)};");
                }
                builder.AppendLine(Helpers.AddIndentation(constructorBuilder.ToString(),
                    1));

            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateConstructorWithParents(Table table)
        {
            StringBuilder builder;
            StringBuilder parametersCommaSeparated;
            StringBuilder constructorBuilder;

            builder = new StringBuilder();
            parametersCommaSeparated = new StringBuilder();
            constructorBuilder = new StringBuilder();

            //generate columnsCommaSeparated
            foreach (Column column in table.EditableColumns)
            {
                parametersCommaSeparated.Append($"{column.Type.GetJavaType()} {column.LowerCaseName}, ");
            }

            //add parents
            foreach (Table parentTable in table.Parents)
            {
                parametersCommaSeparated.Append($"{parentTable.SingularName} {parentTable.LowerCaseSingularName}, ");
            }
            if (1 < parametersCommaSeparated.Length)
            {
                parametersCommaSeparated = parametersCommaSeparated.Remove(parametersCommaSeparated.Length - 2, 2);
            }


            builder.AppendLine($"public {table.SingularName}({parametersCommaSeparated.ToString()})");
            builder.AppendLine("{");
            {

                //generate the call of main constructor
                constructorBuilder.AppendLine("this(");
                {
                    foreach (Column column in table.EditableColumns)
                    {
                        constructorBuilder.AppendLine($"\t{Helpers.GetDefaultJavaColumnData(column.Type.Type)}, //{column.Name}");
                    }
                    Helpers.RemoveLastApparition(constructorBuilder, ",");
                }
                constructorBuilder.AppendLine(");");

                foreach (Table parentTable in table.Parents)
                {
                    constructorBuilder.AppendLine($"this.{parentTable.LowerCaseSingularName} = {parentTable.LowerCaseSingularName};");
                }

                builder.AppendLine(Helpers.AddIndentation(constructorBuilder.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateModel(Table table, string path, string packageName)
        {
            StringBuilder builder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            classBuilder = new StringBuilder();

            builder.AppendLine("//generated automatically");
            builder.AppendLine($"package {packageName}.Models;");
            builder.AppendLine("import java.util.List;");
            builder.AppendLine("import retrofit2.Call;");
            builder.AppendLine("import retrofit2.Callback;");
            builder.AppendLine("import retrofit2.Response;");
            builder.AppendLine("import retrofit2.Retrofit;");
            builder.AppendLine("import retrofit2.converter.gson.GsonConverterFactory;");
            builder.AppendLine("import retrofit2.http.GET;");
            builder.AppendLine("import retrofit2.http.Query;");
            builder.AppendLine("import retrofit2.http.POST;");
            builder.AppendLine("import retrofit2.http.Body;");
            builder.AppendLine("import java.util.Date;");
            builder.AppendLine();

            builder.AppendLine($"public class {table.SingularName}");
            builder.AppendLine("{");
            {
                classBuilder.AppendLine(GenerateFields(table));
                classBuilder.AppendLine(GenerateGettersSetters(table));
                classBuilder.AppendLine(GenerateConstructor(table));

                if (0 < table.Parents.Count)
                {
                    classBuilder.AppendLine(GenerateConstructorWithParents(table));
                }

                classBuilder.AppendLine(GenerateEmptyConstructor(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
            }
            builder.AppendLine("}");

            IO.WriteFile($"{path}\\{table.SingularName}.java", (builder.ToString()));

            //return builder.ToString();
        }

        private void GenerateModelResponse(Table table, string path, string packageName)
        {
            StringBuilder builder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            classBuilder = new StringBuilder();

            builder.AppendLine("//generated automatically");
            builder.AppendLine($"package {packageName}.Models;");
            builder.AppendLine("import java.util.ArrayList;");
            builder.AppendLine();

            builder.AppendLine($"public class {table.SingularName}Response");
            builder.AppendLine("{");
            {
                classBuilder.AppendLine("private float num_results;");
                classBuilder.AppendLine("public ArrayList< Message > objects = new ArrayList< Message >();");
                classBuilder.AppendLine("private float page;");
                classBuilder.AppendLine("private float total_pages;");

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
            }
            builder.AppendLine("}");

            IO.WriteFile($"{path}\\{table.SingularName}Response.java", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Models";

            IO.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateModel(table, modelsPath, _database.JavaPackageName);
                if(_database.Type == Models.DatabaseType.PhytonFlaskRestless)
                    GenerateModelResponse(table, modelsPath, _database.JavaPackageName);
            }
        }
    }
}
