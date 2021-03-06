﻿using DatabaseFunctionsGenerator.Typescript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class TypescriptComponentGenerator
    {
        private Database _database;

        public TypescriptComponentGenerator(Database database)
        {
            _database = database;
        }

        private void GenerateComponentForTable(Table table, string path)
        {

            TypescriptControllerComponentGenerator.GenerateControllerComponentForTable(table, path);
            TypescriptComponentSpecGenerator.GenerateComponentSpec(table, path);
            TypescriptComponentViewGenerator.GenerateViewForTable(table, _database.Type, path);
            TypescriptComponentStyleGenerator.GenerateStyleForTable(table, path);
        }

        public void Generate(string path)
        {
            string componentPath;

            componentPath = $"{path}\\Components";

            IO.CreateDirectory(componentPath);
            foreach (Table table in _database.Tables)
            {
                string tableComponentPath;

                tableComponentPath = $"{componentPath}\\{table.LowerCaseSingularName}";

                IO.CreateDirectory(tableComponentPath);
                GenerateComponentForTable(table, tableComponentPath);
            }
        }
    }
}
