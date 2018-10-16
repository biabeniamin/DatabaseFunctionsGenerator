using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    public class JavaControllerGenerator
    {
        private Database _database;

        public JavaControllerGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateGetMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine($"public static async Task<List<{table.SingularName}>> Get{table.Name}()");
            builder.AppendLine("{");
            {
                //declaration
                methodBody.AppendLine($"List<{table.SingularName}> {table.LowerCaseName};");
                methodBody.AppendLine($"string data;");
                methodBody.AppendLine();

                //initialization
                methodBody.AppendLine($"{table.LowerCaseName} = new List<{table.SingularName}>();");
                methodBody.AppendLine("data = \"\";");
                methodBody.AppendLine();

                //try
                methodBody.AppendLine("try");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\tdata = await HttpRequestClient.GetRequest(\"get{table.Name}\");");
                    methodBody.AppendLine($"\t{table.LowerCaseName} = JsonConvert.DeserializeObject<List<{table.SingularName}>>(data);");
                }
                methodBody.AppendLine("}");

                //catch
                methodBody.AppendLine("catch(Exception ee)");
                methodBody.AppendLine("{");
                {
                    methodBody.AppendLine($"\tConsole.WriteLine(ee.Message);");
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

        private void GenerateController(Table table, string path)
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
            builder.AppendLine("using Newtonsoft.Json;");

            builder.AppendLine("namespace DatabaseFunctionsGenerator");
            builder.AppendLine("{");
            {

                namespaceBuilder.AppendLine($"public static class {table.Name}");
                namespaceBuilder.AppendLine("{");
                {
                    classBuilder.AppendLine(GenerateGetMethod(table));

                    namespaceBuilder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
                }
                namespaceBuilder.AppendLine("}");
                builder.AppendLine(Helpers.AddIndentation(namespaceBuilder.ToString(),
                    1));
            }
            builder.AppendLine("}");

            Helpers.WriteFile($"{path}\\{table.Name}.cs", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Controllers";

            Directory.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, modelsPath);
            }
        }
    }
}
