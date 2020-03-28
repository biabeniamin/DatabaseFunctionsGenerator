using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Deployment
{
    public class PythonDeployer : IDeployer
    {
        private Generator _generator;

        public PythonDeployer(Generator generator)
        {
            _generator = generator;
        }

        public void Deploy()
        {
            IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Python",
                $@"{Constants.PythonServerPath}");
        }
    }
}
