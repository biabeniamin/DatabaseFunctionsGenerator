using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    public class PythonModelsGenerator : IGenerator
    {
        private Database _database;

        public PythonModelsGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateFields(Table table)
        {
            StringBuilder builder;

            builder = new StringBuilder();

            foreach(Column column in table.Columns)
            {
                builder.AppendLine($"{column.LowerCaseName} : {column.Type.GetPythonDataClassType()}");
            }

            return builder.ToString();
        }


        private void GenerateModel(Table table, string path, string packageName)
        {
            StringBuilder builder;
            StringBuilder classBuilder;

            builder = new StringBuilder();
            classBuilder = new StringBuilder();

            builder.AppendLine("#generated automatically");
            
            builder.AppendLine();

            builder.AppendLine("@dataclass_json");
            builder.AppendLine("@dataclass");
            builder.AppendLine($"class {table.SingularName}:");
            {
                classBuilder.AppendLine(GenerateFields(table));

                builder.AppendLine(Helpers.AddIndentation(classBuilder.ToString(),
                        1));
            }

            Helpers.WriteFile($"{path}\\{table.SingularName}.py", (builder.ToString()));

            //return builder.ToString();
        }

        public void Generate(string path)
        {
            string modelsPath;

            modelsPath = $"{path}\\Models";

            Directory.CreateDirectory(modelsPath);

            foreach (Table table in _database.Tables)
            {
                GenerateModel(table, modelsPath, _database.JavaPackageName);
            }
        }
    }
}
