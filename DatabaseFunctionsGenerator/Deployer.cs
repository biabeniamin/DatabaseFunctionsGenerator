﻿using System;
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
            IO.CreateDirectory($"{_serverPath}\\{_generator.Timestamp}");
            IO.Copy("GeneratorResult\\20180916213520426\\Typescript\\user\\user.component.html",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\user\\user.component.html");
            IO.Copy("GeneratorResult\\20180916213520426\\Typescript\\user\\user.component.ts",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\user\\user.component.ts");
            IO.Copy("GeneratorResult\\20180916213520426\\Typescript\\Models\\User.ts",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\Models\User.ts");
        }
    }
}
