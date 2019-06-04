using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Deployment
{
    public class AngularDeployer
    {
        private Generator _generator;

        public AngularDeployer(Generator generator)
        {
            _generator = generator;
        }

        private void DeployModels()
        {
            IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Models",
                $@"{Constants.AngularPath}\Models");
        }

        private void DeployControllers()
        {
            IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Components",
                $@"{Constants.AngularPath}");
        }

        public void Deploy()
        {
            DeployModels();
            DeployControllers();
        }
    }
}
