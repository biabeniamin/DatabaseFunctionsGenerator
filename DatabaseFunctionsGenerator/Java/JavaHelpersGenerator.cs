﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Java
{
    public class JavaHelpersGenerator : IGenerator
    {
        private Database _database;

        public JavaHelpersGenerator(Database database)
        {
            _database = database;
        }

        public void Generate(string path)
        {
            StringBuilder builder;
            string retrofitInstace;

            builder = new StringBuilder();
            retrofitInstace = Helpers.ReadFile("CodeHelpers\\RetrofitInstace.java");

            //replace url
            retrofitInstace = retrofitInstace.Replace("!--url--!", _database.ServerUrl);

            Helpers.WriteFile($"{path}\\Controllers\\RetrofitInstace.java", retrofitInstace);
        }
    }
}