using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Generator
    {
        private Database _database;

        private SqlGenerator _sqlGenerator;
        //private PhpHelpers _phps
        private PhpModelsGenerator _phpModelsGenerator;

        public Generator(Database database)
        {
            _database = database;

            _sqlGenerator = new SqlGenerator(_database);
            _phpModelsGenerator = new PhpModelsGenerator(_database);
        }

        public void Generate()
        {
            //System.Windows.MessageBox.Show(_sqlGenerator.Generate());
            _phpModelsGenerator.Generate();
        }
    }
}
