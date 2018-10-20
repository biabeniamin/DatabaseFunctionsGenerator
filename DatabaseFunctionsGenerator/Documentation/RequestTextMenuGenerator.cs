using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    class RequestTextMenuGenerator : IGenerator
    {
        private Database _database;

        public RequestTextMenuGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateRequestMenu(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //get request to get data
            builder.AppendLine($"------------{table.Name}----------");
            builder.AppendLine($"{serverUrl}{table.Name}.php?cmd=get{table.Name} - GET request -return all {table.Name}");

            //dedicated get requests

            foreach(DedicatedGetRequest request in table.DedicatedGetRequests)
            {
                builder.Append($"{serverUrl}{table.Name}.php?cmd=get{table.Name}By{request.ToString("")}");

                foreach (Column column in request.Columns)
                {
                    builder.Append($"&{column.LowerCaseName}=value");
                }

                builder.AppendLine($" - GET request -return {table.Name} with specified criteria");
            }

            //get request to add data

            builder.Append($"{serverUrl}{table.Name}.php?cmd=add{table.SingularName}");
            foreach(Column column in table.EditableColumns)
            {
                builder.Append($"&{column.LowerCaseName}={Helpers.GetDefaultColumnDataWithoutApostrophe(column.Type.Type)}");
            }
            builder.AppendLine($" - GET request -add a new {table.SingularName} with specified data and return the item if was added");


            //generate post request
            builder.AppendLine();
            builder.Append($"{serverUrl}{table.Name}.php?cmd=add{table.SingularName}");
            builder.AppendLine($"- POST request with following parameters");
            foreach (Column column in table.EditableColumns)
            {
                builder.AppendLine($"{column.LowerCaseName} : {column.Type.GetMysqlType()}");
            }
            builder.AppendLine($"add a new {table.SingularName} with specified data and returns a new {table.SingularName} if succeeded");

            return builder.ToString();
        }

        public void Generate(string path)
        {
            StringBuilder requestMenu;

            requestMenu = new StringBuilder();

            foreach (Table table in _database.Tables)
            {
                requestMenu.AppendLine(GenerateRequestMenu(table, _database.ServerUrl));
            }

            Helpers.WriteFile($"{path}\\Requests.txt", requestMenu.ToString());
        }
    }
}
