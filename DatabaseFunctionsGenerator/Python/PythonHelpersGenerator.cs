using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    class PythonHelpersGenerator
    {
        private Database _database;
        public PythonHelpersGenerator(Database database)
        {
            _database = database;
        }
        public string Generate(string path)
        {
            StringBuilder builder;
            string sqlAlchemy;

            builder = new StringBuilder();
            sqlAlchemy = IO.ReadFile("CodeHelpers\\SqlAlchemy.py");

            //replace databaseName
            sqlAlchemy = sqlAlchemy.Replace("!--databaseName--!", _database.DatabaseName);

            IO.WriteFile($"{path}\\SqlAlchemy.py", sqlAlchemy);

            return builder.ToString();
        }
    }
}
