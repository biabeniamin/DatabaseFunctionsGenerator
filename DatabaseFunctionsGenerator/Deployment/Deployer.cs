using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Deployment
{
    public class Deployer:IDeployer
    {
        private Generator _generator;
        private AngularDeployer _angularDeployer;
        private PhpDeployer _phpDeployer;
        private DatabaseDeployer _databaseDeployer;
        

        public Deployer(Generator generator)
        {
            _generator = generator;
            _angularDeployer = new AngularDeployer(_generator);
            _phpDeployer = new PhpDeployer(_generator);
            _databaseDeployer = new DatabaseDeployer(_generator);
        }


        public void Deploy()
        {
            _angularDeployer.Deploy();
            _phpDeployer.Deploy();
            _databaseDeployer.Deploy();
        }
    }
}
