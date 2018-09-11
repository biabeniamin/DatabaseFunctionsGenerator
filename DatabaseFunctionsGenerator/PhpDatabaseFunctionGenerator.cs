using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class PhpDatabaseFunctionGenerator
    {
        private Database _database;

        public PhpDatabaseFunctionGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateFunctionsForTable(string path, Table table)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"function Get{table.Name}($database)");
            builder.AppendLine("{");

            builder.AppendLine($"${table.LowerCaseName} = [];");
            builder.AppendLine($"$data = $database->ReadData(\"SELECT * FROM {table.Name}\");");
            builder.AppendLine();

            builder.AppendLine("foreach($data as $row)");
            builder.AppendLine("{");
            builder.AppendLine($"${Helpers.GetLowerCaseString(table.SingularName)} = new {table.SingularName}(");
            foreach (Column column in table.Columns)
            {
                builder.AppendLine($"$row[\"{column.Name}\"], ");
            }
            //to remove , \r\n
            builder.Remove(builder.Length - 4, 4);
            builder.AppendLine(");");
            builder.AppendLine();

            builder.AppendLine($"${table.LowerCaseName}[count(${table.LowerCaseName})] = ${Helpers.GetLowerCaseString(table.SingularName)};");


            builder.AppendLine("}");
            builder.AppendLine();

            builder.AppendLine($"return ${table.LowerCaseName};");
            builder.AppendLine("}");

            return builder.ToString();
        }

        public void Generate(string path)
        {
            foreach(Table table in _database.Tables)
            {
                GenerateFunctionsForTable(path, table);
            }
        }
    }
}
