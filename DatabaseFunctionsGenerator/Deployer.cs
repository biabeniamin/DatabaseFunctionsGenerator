using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Deployer
    {
        private Generator _generator;
        
        public Deployer(Generator generator)
        {
            _generator = generator;
        }
    }
}
