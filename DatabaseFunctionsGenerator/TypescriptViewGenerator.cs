using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptViewGenerator
    {
        private Database _database;

        public TypescriptViewGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateListView(Table table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();

            builder.AppendLine($"<tbody>");
            {
                tableBody.AppendLine($"<tr *ngFor=\"let {table.LowerCaseSingularName} of {table.LowerCaseName}Controller; let i = index\">");
                {
                    foreach (Column column in table.Columns)
                    {
                        tableBody.AppendLine("/t<td>");
                        {
                            tableBody.AppendLine("/t<td>");
                        }
                        tableBody.AppendLine("/t</td>");
                    }
                }
                tableBody.AppendLine("</tr>");

                builder.Append(Helpers.AddIndentation(tableBody.ToString(), 1));
            }
            builder.AppendLine($"</tbody>");

            return builder.ToString();
        }


        private string GenerateViewForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(GenerateListView(table));

            return builder.ToString();
        }

        public void Generate(string path)
        {
            foreach (Table table in _database.Tables)
            {
                GenerateViewForTable(path, table);
            }
        }
    }
}
