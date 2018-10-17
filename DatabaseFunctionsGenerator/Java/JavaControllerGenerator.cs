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

        private string GenerateSyncDedicatedGetMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            foreach (DedicatedGetRequest dedicatedRequest in table.DedicatedGetRequests)
            {
                builder.Append($"public static List<{table.SingularName}> get{table.Name}By{dedicatedRequest.ToString("")}(");

                foreach (Column column in dedicatedRequest.Columns)
                {
                    builder.Append($"{column.Type.GetJavaType()} {column.LowerCaseName}, ");
                }
                Helpers.RemoveLastApparition(builder, ", ");

                builder.AppendLine($")");
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
                        methodBody.Append($"\tcall = service.getUsersBy{dedicatedRequest.ToString("")}(");

                        foreach (Column column in dedicatedRequest.Columns)
                        {
                            methodBody.Append($"{column.LowerCaseName}, ");
                        }
                        Helpers.RemoveLastApparition(methodBody, ", ");

                        methodBody.AppendLine($");");
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
                builder.AppendLine();
                methodBody.Clear();
            }

            return builder.ToString();
        }

        private string GenerateAsyncGetMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine($"public static void get{table.Name}(Call<List<{table.SingularName}>> call, Callback<List<{table.SingularName}>> callback)");
            builder.AppendLine("{");
            {
                //declaration

                //initialization

                //try
                methodBody.AppendLine("try");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\tcall.enqueue(callback);");
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
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");
            methodBody.Clear();

            //generate the get to get all data
            builder.AppendLine($"public static void get{table.Name}(Callback<List<{table.SingularName}>> callback)");
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
                    methodBody.AppendLine($"\tget{table.Name}(call, callback);");
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

                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateAsyncDedicatedGetMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            foreach (DedicatedGetRequest dedicatedRequest in table.DedicatedGetRequests)
            {
                builder.Append($"public static void get{table.Name}By{dedicatedRequest.ToString("")}(");

                foreach (Column column in dedicatedRequest.Columns)
                {
                    builder.Append($"{column.Type.GetJavaType()} {column.LowerCaseName}, ");
                }

                //add callback parameter
                builder.Append($"Callback<List<{table.SingularName}>> callback, ");

                Helpers.RemoveLastApparition(builder, ", ");

                builder.AppendLine($")");
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
                        methodBody.Append($"\tcall = service.getUsersBy{dedicatedRequest.ToString("")}(");

                        foreach (Column column in dedicatedRequest.Columns)
                        {
                            methodBody.Append($"{column.LowerCaseName}, ");
                        }
                        Helpers.RemoveLastApparition(methodBody, ", ");

                        methodBody.AppendLine($");");
                        methodBody.AppendLine($"\tget{table.Name}(call, callback);");
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

                    builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                        1));
                }
                builder.AppendLine("}");
                builder.AppendLine();
                methodBody.Clear();
            }

            return builder.ToString();
        }

        private string GenerateSyncAddMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine($"public static {table.SingularName} add{table.SingularName}({table.SingularName} {table.LowerCaseSingularName})");
            builder.AppendLine("{");
            {
                //declaration
                methodBody.AppendLine($"{table.SingularName}Service service;");
                methodBody.AppendLine($"Call<{table.SingularName}> call;");
                methodBody.AppendLine();

                //initialization
                methodBody.AppendLine();

                methodBody.AppendLine($"service = RetrofitInstance.GetRetrofitInstance().create({table.SingularName}Service.class);");

                //try
                methodBody.AppendLine("try");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\tcall = service.add{table.SingularName}({table.LowerCaseSingularName});");
                    methodBody.AppendLine($"\t{table.LowerCaseSingularName} = call.execute().body();");
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
                methodBody.AppendLine($"return {table.LowerCaseSingularName};");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateAsyncAddMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine($"public static void add{table.SingularName}({table.SingularName} {table.LowerCaseSingularName}, Callback<{table.SingularName}> callback)");
            builder.AppendLine("{");
            {
                //declaration
                methodBody.AppendLine($"{table.SingularName}Service service;");
                methodBody.AppendLine($"Call<{table.SingularName}> call;");
                methodBody.AppendLine();

                //initialization
                methodBody.AppendLine();

                methodBody.AppendLine($"service = RetrofitInstance.GetRetrofitInstance().create({table.SingularName}Service.class);");

                //try
                methodBody.AppendLine("try");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\tcall = service.add{table.SingularName}({table.LowerCaseSingularName});");
                    methodBody.AppendLine($"\tcall.enqueue(callback);");
                }
                methodBody.AppendLine("}");

                //catch
                methodBody.AppendLine("catch(Exception ee)");
                methodBody.AppendLine("{");
                {
                    methodBody.AppendLine($"\tSystem.out.println(ee.getMessage());");
                }
                methodBody.AppendLine("}");

                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }


        private string GenerateApiInterface(Table table)
        {
            StringBuilder builder;
            StringBuilder interfaceBuilder;

            builder = new StringBuilder();
            interfaceBuilder = new StringBuilder();

            builder.AppendLine($"interface {table.SingularName}Service");
            builder.AppendLine("{");
            {
                //get all data
                interfaceBuilder.AppendLine($"@GET(\"{table.Name}.php?cmd=get{table.Name}\")");
                interfaceBuilder.AppendLine($"Call<List<{table.SingularName}>> get{table.Name}();");
                interfaceBuilder.AppendLine();

                //dedicated request
                foreach(DedicatedGetRequest dedicatedRequest in table.DedicatedGetRequests)
                {
                    interfaceBuilder.AppendLine($"@GET(\"{table.Name}.php?cmd=get{table.Name}By{dedicatedRequest.ToString("")}\")");
                    interfaceBuilder.Append($"Call<List<{table.SingularName}>> get{table.Name}By{dedicatedRequest.ToString("")}(");

                    foreach (Column column in dedicatedRequest.Columns)
                    {
                        interfaceBuilder.Append($"@Query(\"{column.LowerCaseName}\"){column.Type.GetJavaType()} {column.LowerCaseName}, ");
                    }
                    Helpers.RemoveLastApparition(interfaceBuilder, ", ");

                    interfaceBuilder.AppendLine(");");
                    interfaceBuilder.AppendLine();
                }

                //add metrhod
                //@POST("Users.php?cmd=addUser")
                //Call<User> addUser(@Body User user);
                interfaceBuilder.AppendLine($"@POST(\"{table.Name}.php?cmd=add{table.SingularName}\")");
                interfaceBuilder.Append($"Call<{table.SingularName}> add{table.SingularName}(@Body {table.SingularName} {table.LowerCaseSingularName});");
                interfaceBuilder.AppendLine();

                builder.AppendLine(Helpers.AddIndentation(interfaceBuilder.ToString(),
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

            builder.AppendLine(GenerateApiInterface(table));

            builder.AppendLine($"public class {table.Name}");
            builder.AppendLine("{");
            {
                classBuilder.AppendLine(GenerateSyncGetMethod(table));
                classBuilder.AppendLine(GenerateSyncDedicatedGetMethod(table));
                classBuilder.AppendLine(GenerateAsyncGetMethod(table));
                classBuilder.AppendLine(GenerateAsyncDedicatedGetMethod(table));
                classBuilder.AppendLine(GenerateSyncAddMethod(table));
                classBuilder.AppendLine(GenerateAsyncAddMethod(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
            }
            builder.AppendLine("}");

            Helpers.WriteFile($"{path}\\{table.Name}.java", (builder.ToString()));

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
