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
        private DatabaseOperations _database;
        private string _serverPath = @"D:\xampp\htdocs";

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }


        public Deployer(Generator generator)
        {
            _generator = generator;
            _database = new DatabaseOperations();
        }

        public void Deploy()
        {
            IO.CreateDirectory($"{_serverPath}\\{_generator.Timestamp}");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\user\\user.component.html",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\user\\user.component.html");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\user\\user.component.ts",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\user\\user.component.ts");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Models\\User.ts",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\Models\User.ts");

            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\UserService.ts",
                @"D:\Beni\angular\BackEndGeneratorAngularSample\src\app\UserService.ts");

            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Php\\Users.php",
                @"d:\xampp\htdocs\gen\Users.php");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Php\\Models\\User.php",
                @"d:\xampp\htdocs\gen\Models\User.php");

            _database.ExecuteQuery(IO.ReadFile("GeneratorResult\\20180916213520426\\sqlDatabase.sql"));
        }
    }
}
