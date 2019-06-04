using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Deployment
{
    public class AngularDeployer:IDeployer
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

        private void DeployComponents()
        {
            IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Components",
                $@"{Constants.AngularPath}");
        }

        private void DeployControllers()
        {
            IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Controllers",
                $@"{Constants.AngularPath}");
        }

        private void DeployHelpers()
        {
            IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Helpers",
                $@"{Constants.AngularPath}");
        }

        public void Deploy()
        {
            DeployModels();
            DeployComponents();
            DeployControllers();
            DeployHelpers();
        }
    }
}
