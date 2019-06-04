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
        private DatabaseOperations _database;
        private AngularDeployer _angularDeployer;
        private PhpDeployer _phpDeployer;
        

        public Deployer(Generator generator)
        {
            _generator = generator;
            _database = new DatabaseOperations();
            _angularDeployer = new AngularDeployer(_generator);
            _phpDeployer = new PhpDeployer(_generator);
        }


        public void Deploy()
        {
            _angularDeployer.Deploy();
            _phpDeployer.Deploy();
            //IO.CreateDirectory($@"{Constants.ServerPath}\\{_generator.Timestamp}");
            //    IO.CopyFile($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\user\\user.component.html",
            //     $@"{Constants.AngularPath}\user\\user.component.html");
            //    IO.CopyFile($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\user\\user.component.ts",
            //    $@"{Constants.AngularPath}\user\\user.component.ts");
            //IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\prezenta\\prezenta.component.html",
            //  $@"{Constants.AngularPath}\prezenta\\prezenta.component.html");
            //IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\prezenta\\prezenta.component.ts",
            //$@"{Constants.AngularPath}\prezenta\\prezenta.component.ts");

            //IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\Models\\Prezenta.ts",
            //$@"{Constants.AngularPath}\Models\Prezenta.ts");

            //              IO.CopyFile($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\UserService.ts",
            //                   $@"{Constants.AngularPath}\UserService.ts");
            //IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Typescript\\PrezentaService.ts",
            //$@"{Constants.AngularPath}\PrezentaService.ts");

            //IO.CopyFile($"GeneratorResult\\{_generator.Timestamp}\\C#\\Server\\Controllers\\Message.cs",
            //       $@"D:\Beni\C#\GhidulPolitic\Server\Controllers\ValuesController.cs");
            //IO.CopyDirectory($"GeneratorResult\\{_generator.Timestamp}\\Php",
            //  Constants.ServerPath);
            //IO.CopyFile($"GeneratorResult\\{_generator.Timestamp}\\Php\\Users.php",
            //      $@"d:\xampp\htdocs\gen\Users.php");
            //    IO.CopyFile($"GeneratorResult\\{_generator.Timestamp}\\Php\\Models\\User.php",
            //          $@"d:\xampp\htdocs\gen\Models\User.php");
            //IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Php\\Prezenta.php",
            //$@"d:\xampp\htdocs\gen\Prezenta.php");
            //            IO.Copy($"GeneratorResult\\{_generator.Timestamp}\\Php\\Models\\Prezenta.php",
            //$@"d:\xampp\htdocs\gen\Models\Prezenta.php");

            // _database.ExecuteQuery(IO.ReadFile("GeneratorResult\\20180916213520426\\sqlDatabase.sql"));
        }
    }
}
