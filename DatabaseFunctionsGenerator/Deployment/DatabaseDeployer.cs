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

        public DatabaseDeployer(Generator generator)
        {
            _generator = generator;
        }

        public void Deploy()
        {
            
        }
    }
}
