using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    public class JavaHelpersGenerator : IGenerator
    {
        private Database _database;

        public JavaHelpersGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder;
            string retrofitInstace;

            builder = new StringBuilder();
            retrofitInstace = Helpers.ReadFile("CodeHelpers\\RetrofitInstance.java");

            //replace url
            retrofitInstace = retrofitInstace.Replace("!--url--!", _database.ServerUrl);
            //replace package name
            retrofitInstace = retrofitInstace.Replace("!--packageName--!", _database.JavaPackageName);

            Helpers.WriteFile($"{path}\\Controllers\\RetrofitInstance.java", retrofitInstace);
        }
    }
}
