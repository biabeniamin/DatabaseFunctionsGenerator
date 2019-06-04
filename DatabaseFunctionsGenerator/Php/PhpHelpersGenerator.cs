using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class PhpHelpersGenerator
    {
        private Database _database;
        public PhpHelpersGenerator(Database database)
        {
            _database = database;
        }
        public string Generate(string path)
        {
            StringBuilder builder;
            string databaseOperations;
            string helpers;

            builder = new StringBuilder();
            databaseOperations = IO.ReadFile("CodeHelpers\\DatabaseOperations.php");
            helpers = IO.ReadFile("CodeHelpers\\Helpers.php");

            //replace databaseName
            databaseOperations = databaseOperations.Replace("!--databaseName--!", _database.DatabaseName);

            IO.WriteFile($"{path}\\Php\\DatabaseOperations.php", databaseOperations);
            IO.WriteFile($"{path}\\Php\\Helpers.php", helpers);


            //builder.AppendLine()

            return builder.ToString();
        }
    }
}
