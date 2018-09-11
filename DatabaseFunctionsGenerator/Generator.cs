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
        private PhpHelpersGenerator _phpHelpersGenerator;
        private PhpModelsGenerator _phpModelsGenerator;
        private NotificationSystem _notificationSystem;
        private PhpDatabaseFunctionGenerator _phpDatabaseFunctionGenerator;

        public Generator(Database database)
        {
            _database = database;

            _sqlGenerator = new SqlGenerator(_database);
            _phpModelsGenerator = new PhpModelsGenerator(_database);
            _phpHelpersGenerator = new PhpHelpersGenerator();
            _phpDatabaseFunctionGenerator = new PhpDatabaseFunctionGenerator(_database);
            _notificationSystem = new NotificationSystem();
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
            }
        }

        public void Generate()
        {
            string path;

            path = $"GeneratorResult\\{Helpers.GenerateTimeStamp()}";

            if (!Directory.Exists("GeneratorResult"))
            {
                Directory.CreateDirectory("GeneratorResult");
            }

            Directory.CreateDirectory(path);

            //add missing fields
            AddMissingFields();

            _sqlGenerator.Generate(path);
            _phpDatabaseFunctionGenerator.Generate(path);
            _phpModelsGenerator.Generate(path);
            _phpHelpersGenerator.Generate(path);
        }
    }
}
