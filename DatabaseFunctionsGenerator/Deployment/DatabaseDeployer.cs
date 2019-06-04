using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Deployment
{
    public class DatabaseDeployer : IDeployer
    {
        private Generator _generator;
        private DatabaseOperations _database;

        public DatabaseDeployer(Generator generator)
        {
            _generator = generator;
            _database = new DatabaseOperations();
        }

        public void Deploy()
        {
            _database.ExecuteQuery(IO.ReadFile("GeneratorResult\\20180916213520426\\sqlDatabase.sql"));
        }
    }
}
