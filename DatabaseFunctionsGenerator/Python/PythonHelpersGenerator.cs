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
            string authentication;
            string restfulHelpers;
            string validationError;
            string websocketsHelpers;
            string tokenAuthenticationEndpoints;
            string tokenAuthenticationWebSockets;

            builder = new StringBuilder();
            sqlAlchemy = IO.ReadFile("CodeHelpers\\SqlAlchemy.py");
            authentication = IO.ReadFile("CodeHelpers\\Authentication.py");
            validationError = IO.ReadFile("CodeHelpers\\ValidationError.py"); ;
            restfulHelpers = IO.ReadFile("CodeHelpers\\FlaskRestfulHelpers.py");
            websocketsHelpers = IO.ReadFile("CodeHelpers\\WebSocketsHelpers.py");
            tokenAuthenticationEndpoints = IO.ReadFile("CodeHelpers\\TokenAuthenticationEndpoints.py");
            tokenAuthenticationWebSockets = IO.ReadFile("CodeHelpers\\TokenAuthenticationWebSockets.py");

            //replace databaseName
            sqlAlchemy = sqlAlchemy.Replace("!--databaseName--!", _database.DatabaseName);

            IO.WriteFile($"{path}\\SqlAlchemy.py", sqlAlchemy);
            IO.WriteFile($"{path}\\FlaskRestfulHelpers.py", restfulHelpers);
            IO.WriteFile($"{path}\\ValidationError.py", validationError);
            IO.WriteFile($"{path}\\WebSocketsHelpers.py", websocketsHelpers);
            IO.WriteFile($"{path}\\TokenAuthenticationEndpoints.py", tokenAuthenticationEndpoints);
            IO.WriteFile($"{path}\\TokenAuthenticationWebSockets.py", tokenAuthenticationWebSockets);

            if (_database.HasAuthenticationSystem)
                IO.WriteFile($"{path}\\Authentication.py", authentication);

            return builder.ToString();
        }
    }
}
