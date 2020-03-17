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

        public static string AddIndentation(StringBuilder textBuilder, int indentation)
        {
            return AddIndentation(textBuilder.ToString(), indentation);
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
                case Types.Integer:
                    return "0";
                case Types.Double:
                    return "0";
                case Types.Text:
                case Types.Varchar:
                    return "\'Test\'";
                case Types.GUID:
                    return "\'00000000-0000-0000-0000-000000000000\'";
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
                case Types.Integer:
                case Types.Double:
                    return "0";
                case Types.Text:
                case Types.Varchar:
                    return "\"Test\"";
                default:
                    return "";
            }
        }

        public static string GetDefaultPythonColumnData(Types type)
        {
            switch (type)
            {
                case Types.DateTime:
                    return "new DateTime(1970, 1, 1, 0, 0, 0)";
                case Types.Integer:
                case Types.Double:
                    return "0";
                case Types.Text:
                case Types.Varchar:
                    return "\"Test\"";
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
                case Types.Integer:
                case Types.Double:
                    return "0";
                case Types.Text:
                case Types.Varchar:
                    return "\"Test\"";
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
                case Types.Integer:
                case Types.Double:
                    return "0";
                case Types.Text:
                    return "\'\'";
                case Types.Varchar:
                case Types.GUID:
                    return "\'\'";
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
                case Types.Double:
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

        public static string ConcatenateList<T>(IEnumerable<T> list, string separator)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in list)
            {
                stringBuilder.Append($"{item}{separator} ");
            }
            RemoveLastApparition(stringBuilder, $"{separator} ");

            return stringBuilder.ToString();
        }

        public static string ConcatenateList<T>(IEnumerable<T> list, string separator, string start)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in list)
            {
                stringBuilder.Append($"{start}{item}{separator} ");
            }
            RemoveLastApparition(stringBuilder, $"{separator} ");

            return stringBuilder.ToString();
        }
    }

}
