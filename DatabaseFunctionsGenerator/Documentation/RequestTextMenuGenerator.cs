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

        private string GenerateGetAllData(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //get request to get data
            builder.AppendLine($"{serverUrl}{table.Name}.php?cmd=get{table.Name} - GET request -return all {table.Name}");

            return builder.ToString();
        }

        private string GenerateDedicatedRequests(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //dedicated get requests

            foreach (DedicatedGetRequest request in table.DedicatedGetRequests)
            {
                builder.Append($"{serverUrl}{table.Name}.php?cmd=get{table.Name}By{request.ToString("")}");

                foreach (Column column in request.Columns)
                {
                    builder.Append($"&{column.LowerCaseName}=value");
                }

                builder.AppendLine($" - GET request -return {table.Name} filtered by {request.ToString(", ")}");
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private string GenerateGetAddData(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //get request to add data

            builder.Append($"{serverUrl}{table.Name}.php?cmd=add{table.SingularName}");
            foreach (Column column in table.EditableColumns)
            {
                builder.Append($"&{column.LowerCaseName}={Helpers.GetDefaultColumnDataWithoutApostrophe(column.Type.Type)}");
            }
            builder.AppendLine($" - GET request -add a new {table.SingularName} with specified data and return the item with id != 0 if was added");

            return builder.ToString();
        }

        private string GeneratePostAddData(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //generate post request
            builder.AppendLine();
            builder.Append($"{serverUrl}{table.Name}.php?cmd=add{table.SingularName}");
            builder.AppendLine($"- POST request with following parameters");
            foreach (Column column in table.EditableColumns)
            {
                builder.AppendLine($"{column.LowerCaseName} : {column.Type.GetMysqlType()}");
            }
            builder.AppendLine($"add a new {table.SingularName} with specified data and returns the {table.SingularName} with id != 0 if succeeded");

            return builder.ToString();
        }

        private string GenerateUpdateData(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //generate post request
            builder.AppendLine();
            builder.Append($"{serverUrl}{table.Name}.php?cmd=update{table.SingularName}");
            builder.AppendLine($"- PUT request with following parameters");
            //add primary key
            builder.AppendLine($"{table.PrimaryKeyColumn.LowerCaseName} : {table.PrimaryKeyColumn.Type.GetMysqlType()}");

            foreach (Column column in table.EditableColumns)
            {
                builder.AppendLine($"{column.LowerCaseName} : {column.Type.GetMysqlType()}");
            }
            builder.AppendLine($"update a {table.SingularName} by id and returns the {table.SingularName} with id != 0 if succeeded");

            return builder.ToString();
        }

        private string GenerateDeleteData(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            //generate post request
            builder.AppendLine();
            builder.Append($"{serverUrl}{table.Name}.php?cmd=update{table.SingularName}&{table.PrimaryKeyColumn.LowerCaseName}={Helpers.GetDefaultColumnDataWithoutApostrophe(table.PrimaryKeyColumn.Type.Type)}");
            builder.AppendLine($" - DELETE request");
            builder.AppendLine($"deletes a {table.SingularName} by id and returns the {table.SingularName} with id == 0 if succeeded");

            return builder.ToString();
        }

        private string GenerateRequestMenu(Table table, string serverUrl)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            builder.AppendLine($"------------{table.Name}----------");

            builder.AppendLine(GenerateGetAllData(table, serverUrl));
            builder.AppendLine(GenerateDedicatedRequests(table, serverUrl));
            builder.AppendLine(GenerateGetAddData(table, serverUrl));
            builder.AppendLine(GeneratePostAddData(table, serverUrl));
            builder.AppendLine(GenerateUpdateData(table, serverUrl));
            builder.AppendLine(GenerateDeleteData(table, serverUrl));

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
