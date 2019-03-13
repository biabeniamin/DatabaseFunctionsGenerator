using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class GeneratorConfigGenerator : IGenerator
    {
        private Database _database;

        public GeneratorConfigGenerator(Database database)
        {
            _database = database;
        }

        private void GenerateConfigurationFile(string path)
        {
            string config;

            config = "";

            config = JsonConvert.SerializeObject(_database);

            IO.WriteFile($"{path}\\Config.json", config);
        }

        public void Generate(string path)
        {
            GenerateConfigurationFile(path);
        }
    }
}
