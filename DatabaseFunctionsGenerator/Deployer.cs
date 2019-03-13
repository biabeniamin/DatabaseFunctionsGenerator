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
        private string _serverPath = @"D:\xampp\htdocs";

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }


        public Deployer(Generator generator)
        {
            _generator = generator;
        }

        public void Deploy()
        {
            Helpers.CreateDirectory($"{_serverPath}\\{_generator.Timestamp}");
        }
    }
}
