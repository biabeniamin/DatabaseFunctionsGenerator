using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.Python
{
    public class PythonGenerator : IGenerator
    {
        private Database _database;

        private PythonModelsGenerator _pythonModelsGenerator;
        private PythonControllerGenerator _pythonControllerGenerator;
        private PythonHelpersGenerator _pythonHelpersGenerator;
        private PythonFlaskRestlessInstanceGenerator _pythonFlaskRestlessInstanceGenerator;
        private PythonSqlAlchemyInstanceGenerator _pythonSqlAlchemyInstanceGenerator;
        private PythonFlaskRestfulEndpointsGenerator _pythonEndpointsGenerator;
        private PythonFlaskRestfulInstanceGenerator _pythonFlaskRestfulInstanceGenerator;

        public PythonGenerator(Database database)
        {
            _database = database;

            _pythonModelsGenerator = new PythonModelsGenerator(_database);
            _pythonControllerGenerator = new PythonControllerGenerator(_database);
            _pythonHelpersGenerator = new PythonHelpersGenerator(_database);
            _pythonFlaskRestlessInstanceGenerator = new PythonFlaskRestlessInstanceGenerator(_database);
            _pythonSqlAlchemyInstanceGenerator = new PythonSqlAlchemyInstanceGenerator(_database);
            _pythonEndpointsGenerator = new PythonFlaskRestfulEndpointsGenerator(_database);
            _pythonFlaskRestfulInstanceGenerator = new PythonFlaskRestfulInstanceGenerator(_database);
        }

        public void Generate(string path)
        {
            string pythonPath;
            string pythonServerPath;
            string pythonClientPath;

            pythonPath = $"{path}\\Python";
            pythonServerPath = $"{pythonPath}\\Server";
            pythonClientPath = $"{pythonPath}\\Client";
            IO.CreateDirectory(pythonPath);
            IO.CreateDirectory(pythonServerPath);
            IO.CreateDirectory(pythonClientPath);

            _pythonModelsGenerator.Generate(pythonClientPath);
            _pythonControllerGenerator.Generate(pythonServerPath);
            _pythonHelpersGenerator.Generate(pythonServerPath);
            _pythonFlaskRestlessInstanceGenerator.Generate(pythonServerPath);
            _pythonSqlAlchemyInstanceGenerator.Generate(pythonServerPath);
            _pythonEndpointsGenerator.Generate(pythonServerPath);
            _pythonFlaskRestfulInstanceGenerator.Generate(pythonServerPath);
        }
    }
}
