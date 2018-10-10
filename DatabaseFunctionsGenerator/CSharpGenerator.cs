using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class CSharpGenerator : IGenerator
    {
        private Database _database;

        public CSharpGenerator(Database database)
        {
            _database = database;

        }

        public void Generate(string path)
        {
            string cSharpPath;

            cSharpPath = $"{path}\\C#";

        }
    }
}
