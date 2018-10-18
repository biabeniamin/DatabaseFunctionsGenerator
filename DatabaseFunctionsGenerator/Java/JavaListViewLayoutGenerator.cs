using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    public class JavaListViewLayoutGenerator : IGenerator
    {
        private Database _database;

        public JavaListViewLayoutGenerator(Database database)
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

        private void GenerateListViewLayout(Table table, string path)
        {
            StringBuilder layoutBuilder;
            StringBuilder linearLayoutBuilder;

            layoutBuilder = new StringBuilder();
            linearLayoutBuilder = new StringBuilder();


            layoutBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            layoutBuilder.AppendLine("<LinearLayout xmlns:android=\"http://schemas.android.com/apk/res/android\"");
            { 
                linearLayoutBuilder.AppendLine("android:layout_width=\"match_parent\"");
                linearLayoutBuilder.AppendLine("android:layout_height=\"match_parent\" >");

                foreach(Column column in table.Columns)
                {
                    linearLayoutBuilder.AppendLine($"<TextView");
                    linearLayoutBuilder.AppendLine($"\tandroid:id=\"@+id/{column.LowerCaseName}TextBox\"");
                    linearLayoutBuilder.AppendLine($"\tandroid:layout_width=\"wrap_content\"");
                    linearLayoutBuilder.AppendLine($"\tandroid:layout_height=\"wrap_content\"");
                    linearLayoutBuilder.AppendLine($"\tandroid:text=\"Name\"/>");
                }

                layoutBuilder.AppendLine(Helpers.AddIndentation(linearLayoutBuilder.ToString(),
                        1));
            }
            layoutBuilder.AppendLine("</LinearLayout>");  

            Helpers.WriteFile($"{path}\\{table.SingularName}View.xml", (layoutBuilder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Layouts";

            Directory.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateListViewLayout(table, modelsPath);
            }
        }
    }
}
