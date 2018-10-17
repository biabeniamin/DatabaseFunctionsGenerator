using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    public class JavaAdapterGenerator : IGenerator
    {
        private Database _database;

        public JavaAdapterGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateSyncGetMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine($"public static List<{table.SingularName}> get{table.Name}(Call<List<{table.SingularName}>> call)");
            builder.AppendLine("{");
            {
                //declaration
                methodBody.AppendLine($"List<{table.SingularName}> {table.LowerCaseName};");
                methodBody.AppendLine();

                //initialization
                methodBody.AppendLine($"{table.LowerCaseName} = null;");
                methodBody.AppendLine();

                //try
                methodBody.AppendLine("try");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\t{table.LowerCaseName} = call.execute().body();");
                }
                methodBody.AppendLine("}");

                //catch
                methodBody.AppendLine("catch(Exception ee)");
                methodBody.AppendLine("{");
                {
                    methodBody.AppendLine($"\tSystem.out.println(ee.getMessage());");
                }
                methodBody.AppendLine("}");

                methodBody.AppendLine();

                //return
                methodBody.AppendLine($"return {table.LowerCaseName};");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");
            methodBody.Clear();

            //generate the get to get all data
            builder.AppendLine($"public static List<{table.SingularName}> get{table.Name}()");
            builder.AppendLine("{");
            {
                //declaration
                methodBody.AppendLine($"List<{table.SingularName}> {table.LowerCaseName};");
                methodBody.AppendLine($"{table.SingularName}Service service;");
                methodBody.AppendLine($"Call<List<{table.SingularName}>> call;");
                methodBody.AppendLine();

                //initialization
                methodBody.AppendLine($"{table.LowerCaseName} = null;");
                methodBody.AppendLine();

                methodBody.AppendLine($"service = RetrofitInstance.GetRetrofitInstance().create({table.SingularName}Service.class);");

                //try
                methodBody.AppendLine("try");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\tcall = service.getUsers();");
                    methodBody.AppendLine($"\t{table.LowerCaseName} = get{table.Name}(call);");
                }
                methodBody.AppendLine("}");

                //catch
                methodBody.AppendLine("catch(Exception ee)");
                methodBody.AppendLine("{");
                {
                    methodBody.AppendLine($"\tSystem.out.println(ee.getMessage());");
                }
                methodBody.AppendLine("}");

                methodBody.AppendLine();

                //return
                methodBody.AppendLine($"return {table.LowerCaseName};");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateController(Table table, string path, string packageName)
        {
            StringBuilder builder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            classBuilder = new StringBuilder();


            builder.AppendLine("//generated automatically");
            builder.AppendLine($"package {packageName};");

            builder.AppendLine("import android.content.Context;");
            builder.AppendLine("import android.view.LayoutInflater;");
            builder.AppendLine("import android.view.View;");
            builder.AppendLine("import android.view.ViewGroup;");
            builder.AppendLine("import android.widget.BaseAdapter;");
            builder.AppendLine("import android.widget.TextView;");
            builder.AppendLine("import java.util.List;");

            builder.AppendLine($"public class {table.SingularName}Adapter extends BaseAdapter");
            builder.AppendLine("{");
            {
                classBuilder.AppendLine(GenerateSyncGetMethod(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
            }
            builder.AppendLine("}");

            Helpers.WriteFile($"{path}\\{table.SingularName}Adapter.java", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Controllers";

            Directory.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, modelsPath, _database.JavaPackageName);
            }
        }
    }
}
