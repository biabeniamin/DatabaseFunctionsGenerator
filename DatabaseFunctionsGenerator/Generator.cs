using DatabaseFunctionsGenerator.Java;
using DatabaseFunctionsGenerator.Python;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Generator
    {
        private Database _database;

        private SqlGenerator _sqlGenerator;
        private TypescriptGenerator _typescriptGenerator;
        private PhpGenerator _phpGenerator;
        private NotificationSystem _notificationSystem;
        private CSharpGenerator _cSharpGenerator;
        private DocumentationGenerator _documentationGenerator;
        private JavaGenerator _javaGenerator;
        private PythonGenerator _pythonGenerator;
        private GeneratorConfigGenerator _generatorConfigGenerator;
        private string _timestamp;

        public string Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public Generator(Database database)
        {
            _database = database;

            _sqlGenerator = new SqlGenerator(_database);
            _phpGenerator = new PhpGenerator(_database);
            _typescriptGenerator = new TypescriptGenerator(_database);
            _notificationSystem = new NotificationSystem();
            _cSharpGenerator = new CSharpGenerator(_database);
            _documentationGenerator = new DocumentationGenerator(_database);
            _javaGenerator = new JavaGenerator(_database);
            _pythonGenerator = new PythonGenerator(_database);
            _generatorConfigGenerator = new GeneratorConfigGenerator(_database);
        }

        private void AddMissingFields()
        {
            //add notification system
            if(!_database.HasNotificationSystem)
            {
                _database.Tables.Add(_notificationSystem.GenerateNotificationTable());
            }

            foreach(Table table in _database.Tables)
            {
                if (!table.HasPrimaryKey)
                {
                    Column column;
                    ColumnType type;

                    type = new ColumnType(Types.Integer, true, true);
                    column = new Column($"{table.SingularName}Id", type);

                    table.Columns.Insert(0, column);
                }

                if (!table.HasCreationTime)
                {
                    Column column;
                    ColumnType type;

                    type = new ColumnType(Types.DateTime);
                    column = new Column($"CreationTime", type);

                    table.Columns.Add(column);
                }

                if(!table.HasDedicatedGetRequestForPrimaryKey)
                {
                    DedicatedGetRequest dedicatedRequest;

                    dedicatedRequest = new DedicatedGetRequest();

                    dedicatedRequest.Columns.Add(table.PrimaryKeyColumn);
                    table.DedicatedGetRequests.Add(dedicatedRequest);
                }
            }
        }

        private void SetParentChildsFields()
        {
            foreach(Relation relation in _database.Relations)
            {
                switch(relation.Type)
                {
                    case RelationType.OneToMany:
                        relation.Table1.Childs.Add(relation.Table2);
                        relation.Table2.Parents.Add(relation.Table1);

                        relation.Table2.Columns.Insert(1, new Column($"{relation.Table1.SingularName}Id", new ColumnType(Types.Integer, true)));
                        break;
                }
            }
        }

        public void Generate()
        {
            string path;

            _timestamp = Helpers.GenerateTimeStamp();
            path = $"GeneratorResult\\{_timestamp}";

            if (!Helpers.DoesDirectoryExists("GeneratorResult"))
            {
                Helpers.CreateDirectory("GeneratorResult");
            }

            Helpers.CreateDirectory(path);
            Helpers.CreateDirectory($"{path}\\Php");
            Helpers.CreateDirectory($"{path}\\Typescript");
            Helpers.CreateDirectory($"{path}\\Java");

            //add missing fields
            AddMissingFields();
            SetParentChildsFields();

            _sqlGenerator.Generate(path);
            _phpGenerator.Generate(path);
            _typescriptGenerator.Generate(path);
            _cSharpGenerator.Generate(path);
            _documentationGenerator.Generate(path);
            _javaGenerator.Generate(path);
            _pythonGenerator.Generate(path);
            _generatorConfigGenerator.Generate(path);

            /*
            foreach (string file in Directory.EnumerateFiles($"{path}\\Php"))
            {
                File.Copy(file, $"d:\\xampp\\htdocs\\generator\\Test\\{Path.GetFileName(file)}", true);
            }

            foreach (string file in Directory.EnumerateFiles($"{path}\\Php\\Models"))
            {
                File.Copy(file, $"d:\\xampp\\htdocs\\generator\\Test\\Models\\{Path.GetFileName(file)}", true);
            }*/

        }
    }
}
