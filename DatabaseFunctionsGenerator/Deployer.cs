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
        

        public Deployer(Generator generator)
        {
            _generator = generator;
            _database = new DatabaseOperations();
        }

        public void Deploy()
        {
            IO.CreateDirectory($@"{Constants.ServerPath}\\{_generator.Timestamp}");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\user\\user.component.html",
                $@"{Constants.AngularPath}\user\\user.component.html");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\user\\user.component.ts",
                $@"{Constants.AngularPath}\user\\user.component.ts");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Models\\User.ts",
                $@"{Constants.AngularPath}\Models\User.ts");

            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\UserService.ts",
                $@"{Constants.AngularPath}\UserService.ts");


            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Php\\Users.php",
                $@"d:\xampp\htdocs\gen\Users.php");
            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Php\\Models\\User.php",
                $@"d:\xampp\htdocs\gen\Models\User.php");

            _database.ExecuteQuery(IO.ReadFile("GeneratorResult\\20180916213520426\\sqlDatabase.sql"));
        }
    }
}
