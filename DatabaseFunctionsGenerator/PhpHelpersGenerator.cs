using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class PhpHelpersGenerator
    {
        public string Generate(string path)
        {
            StringBuilder builder;
            string databaseOperations;
            string helpers;

            builder = new StringBuilder();
            databaseOperations = Helpers.ReadFile("DatabaseOperations.php");
            helpers = Helpers.ReadFile("Helpers.php");

            Helpers.WriteFile($"{path}\\Php\\DatabaseOperations.php", databaseOperations);
            Helpers.WriteFile($"{path}\\Php\\Helpers.php", helpers);


            //builder.AppendLine()

            return builder.ToString();
        }
    }
}
