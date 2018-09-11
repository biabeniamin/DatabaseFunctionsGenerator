using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public static class Helpers
    {
        public static string ReadFile(string path)
        {
            String text;
            StreamReader reader;

            reader = new StreamReader(path);
            text = "";

            text = reader.ReadToEnd();

            reader.Dispose();

            return text;
        }

        public static void WriteFile(string path, string text)
        {
            StreamWriter writer;

            writer = new StreamWriter(path);

            writer.Write(text);

            writer.Dispose();
        }

        public static string AddIndentation(string text, int indentation)
        {
            StringBuilder builder;
            StringReader reader;
            string line;

            builder = new StringBuilder();
            reader = new StringReader(text);

            //generate getters

            while(null != (line = reader.ReadLine()))
            {
                for (int i = 0; i < indentation; i++)
                {
                    builder.Append("\t");
                }

                builder.AppendLine(line);
            }


            return builder.ToString();
        }
    }
}
