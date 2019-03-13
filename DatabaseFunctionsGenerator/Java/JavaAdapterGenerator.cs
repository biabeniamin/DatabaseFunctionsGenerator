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

        private string GenerateGetCountMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine("@Override");
            builder.AppendLine($"public int getCount()");
            builder.AppendLine("{");
            {
                //return
                methodBody.AppendLine($"return {table.LowerCaseName}.size();");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetItemMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine("@Override");
            builder.AppendLine($"public {table.SingularName} getItem(int position)");
            builder.AppendLine("{");
            {
                //return
                methodBody.AppendLine($"return {table.LowerCaseName}.get(position);");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetItemIdMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine("@Override");
            builder.AppendLine($"public long getItemId(int position)");
            builder.AppendLine("{");
            {
                //return
                methodBody.AppendLine($"return {table.LowerCaseName}.get(position).get{table.PrimaryKeyColumn.Name}();");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateGetViewMethod(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine("@Override");
            builder.AppendLine($"public View getView(int position, View convertView, ViewGroup parent)");
            builder.AppendLine("{");
            {
                //declaration
                methodBody.AppendLine($"{table.SingularName} {table.LowerCaseSingularName};");
                foreach (Column column in table.Columns)
                {
                    methodBody.AppendLine($"TextView {column.LowerCaseName}TextBox;");
                }
                methodBody.AppendLine();

                //initialization
                methodBody.AppendLine($"{table.LowerCaseSingularName} = getItem(position);");
                methodBody.AppendLine();

                //try
                methodBody.AppendLine("if(null == convertView)");
                methodBody.AppendLine("{");
                {
                    //get data from server
                    methodBody.AppendLine($"\tconvertView = LayoutInflater.from(context).inflate(R.layout.{table.LowerCaseSingularName.ToLower()}_view, parent, false);");
                }
                methodBody.AppendLine("}");
                methodBody.AppendLine();

                //get text boxes by id
                foreach (Column column in table.Columns)
                {
                    methodBody.AppendLine($"{column.LowerCaseName}TextBox = (TextView) convertView.findViewById(R.id.{column.LowerCaseName}TextBox);");
                }
                methodBody.AppendLine();

                //populate text boxes
                foreach (Column column in table.Columns)
                {
                    if (Types.Text == column.Type.Type
                        || Types.Varchar == column.Type.Type)
                    {
                        methodBody.AppendLine($"{column.LowerCaseName}TextBox.setText({table.LowerCaseSingularName}.get{column.Name}());");
                    }
                    else
                    {
                        methodBody.AppendLine($"{column.LowerCaseName}TextBox.setText({table.LowerCaseSingularName}.get{column.Name}().toString());");
                    }
                }
                methodBody.AppendLine();

                //return
                methodBody.AppendLine($"return convertView;");
                builder.AppendLine(Helpers.AddIndentation(methodBody.ToString(),
                    1));
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GenerateConstuctor(Table table)
        {
            StringBuilder builder;
            StringBuilder methodBody;

            builder = new StringBuilder();
            methodBody = new StringBuilder();

            builder.AppendLine($"public {table.SingularName}Adapter(List<{table.SingularName}> {table.LowerCaseName}, Context context)");
            builder.AppendLine("{");
            {
                //return
                methodBody.AppendLine($"this.{table.LowerCaseName} = {table.LowerCaseName};");
                methodBody.AppendLine($"this.context = context;");
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
            builder.AppendLine($"import {packageName}.Models.*;");
            
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
                //declaration
                classBuilder.AppendLine($"List<{table.SingularName}> {table.LowerCaseName};");
                classBuilder.AppendLine($"Context context;");
                classBuilder.AppendLine();

                classBuilder.AppendLine(GenerateGetCountMethod(table));
                classBuilder.AppendLine(GenerateGetViewMethod(table));
                classBuilder.AppendLine(GenerateGetItemMethod(table));
                classBuilder.AppendLine(GenerateGetItemIdMethod(table));
                classBuilder.AppendLine(GenerateConstuctor(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
            }
            builder.AppendLine("}");

            IO.WriteFile($"{path}\\{table.SingularName}Adapter.java", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Controllers";

            IO.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateController(table, modelsPath, _database.JavaPackageName);
            }
        }
    }
}
