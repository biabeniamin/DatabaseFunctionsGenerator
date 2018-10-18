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
        public static string GetSingular(string plural)
        {
            string singular;

            singular = plural;

            if ("s" == singular.Substring(singular.Length - 1, 1))
            {
                singular = singular.Substring(0, singular.Length - 1); 
            }

            return singular;
        }

        public static string GenerateTimeStamp()
        {
            return "20180916213520426";
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public static string GetLowerCaseString(string text)
        {
            if(1 > text.Length)
            {
                return "";
            }

                //transform to lower
            text = text.Substring(0, 1).ToLower() + text.Substring(1);

            return text;
        }

        public static string GetDefaultColumnData(Types type)
        {
            switch (type)
            {
                case Types.DateTime:
                    return "\'2000-01-01 00:00:00\'";
                    break;
                case Types.Integer:
                    return "0";
                    break;
                case Types.Text:
                case Types.Varchar:
                    return "\'Test\'";
                    break;
                default:
                    return "";
            }
        }

        public static string GetDefaultColumnDataWithoutApostrophe(Types type)
        {
            return GetDefaultColumnData(type).Replace("'", "");
        }

        public static string GetDefaultCSharpColumnData(Types type)
        {
            switch (type)
            {
                case Types.DateTime:
                    return "new DateTime(1970, 1, 1, 0, 0, 0)";
                    break;
                case Types.Integer:
                    return "0";
                    break;
                case Types.Text:
                case Types.Varchar:
                    return "\"Test\"";
                    break;
                default:
                    return "";
            }
        }

        public static string GetDefaultJavaColumnData(Types type)
        {
            switch (type)
            {
                case Types.DateTime:
                    return "new Date(0)";
                    break;
                case Types.Integer:
                    return "0";
                    break;
                case Types.Text:
                case Types.Varchar:
                    return "\"Test\"";
                    break;
                default:
                    return "";
            }
        }

        public static string GetEmptyColumnData(Types type)
        {
            switch (type)
            {
                case Types.DateTime:
                    return "\'2000-01-01 00:00:00\'";
                    break;
                case Types.Integer:
                    return "0";
                    break;
                case Types.Text:
                    return "\'\'";
                    break;
                case Types.Varchar:
                    return "\'\'";
                    break;
                default:
                    return "";
            }
        }

        public static string ConvertToSql(string input, Types type)
        {
            return ConvertToSql("", input, type);
        }
        public static string ConvertToSql(string startCharacter, string input, Types type)
        {
            switch (type)
            {
                case Types.DateTime:
                case Types.Text:
                case Types.Varchar:
                    return $"\'{startCharacter}{input}\'";
                case Types.Integer:
                    return $"{startCharacter}{input}";
            }

            return "";
        }

        public static void RemoveLastApparition(StringBuilder builder, string deleted)

        {

            if (builder.ToString().Contains(deleted))

            {

                builder.Remove(builder.ToString().LastIndexOf(deleted), deleted.Length);

            }

        }
    }

}
