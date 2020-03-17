using DatabaseFunctionsGenerator.Deployment;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseFunctionsGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Database _database;
        private Table _selectedTable;
        private DelegateCommand _generateDelegateCommand;
        private DelegateCommand _importDelegateCommand;
        private bool _securityToken = true;

        public bool SecurityToken
        {
            get { return _securityToken; }
            set 
            {
                _securityToken = value;
                OnPropertyChanged("SecurityToken");
            }
        }

        public DelegateCommand ImportDelegateCommand
        {
            get { return _importDelegateCommand; }
            set { _importDelegateCommand = value; }
        }
        

        public DelegateCommand GenerateDelegateCommand
        {
            get { return _generateDelegateCommand; }
            set { _generateDelegateCommand = value; }
        }

        public Table SelectedTable
        {
            get { return _selectedTable; }
            set
            {
                _selectedTable = value;
                OnPropertyChanged("SelectedTable");
            }
        }

        public Database Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnPropertyChanged("Database");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Database = new Database();
            Database.DatabaseName = "scd";
            Database.Type = Models.DatabaseType.Php;
            Database.HasAuthenticationSystem = true;
            /*
                        Database.Tables.Add(new Table("Users"));


                        Database.Tables[0].Columns.Add(new Column("Nume", new ColumnType(Types.Varchar, 20)));
                        Database.Tables[0].Columns.Add(new Column("Prenume", new ColumnType(Types.Varchar, 20)));
                        //Database.Tables[0].Columns.Add(new Column("Password", new ColumnType(Types.Varchar, 20)));
                        Database.Tables[0].Columns.Add(new Column("Email", new ColumnType(Types.Varchar, 20)));
                        //Database.Tables[0].Columns.Add(new Column("Age", new ColumnType(Types.Integer)));
                        //Database.Tables[0].Columns.Add(new Column("Description", new ColumnType(Types.Text)));
                        Database.Tables[0].Columns.Add(new Column("CNP", new ColumnType(Types.Varchar, 20)));
                        Database.Tables[0].Columns.Add(new Column("NumberTelefon", new ColumnType(Types.Varchar, 20 )));*/

            Database.Tables.Add(new Table("Locations"));
            Database.Tables.Last().RequiresSecurityToken = true;
            Database.Tables.Last().Columns.Add(new Column("TerminalId", new ColumnType(Types.Integer)));
            Database.Tables.Last().Columns.Add(new Column("Latitude", new ColumnType(Types.Double)));
            Database.Tables.Last().Columns.Add(new Column("Longitude", new ColumnType(Types.Double)));
            //Database.Tables.Add(new Table("Prezenta"));
            //Database.Tables.Add(new Table("Locations"));
            //Database.Tables.Add(new Table("AccessLogs"));
            //Database.Tables.Add(new Table("Doors"));
            //Database.Tables.Add(new Table("Pins"));
            //Database.Tables.Add(new Table("Empty"));

            //            Database.Tables[0].Columns.Add(new Column("Id", new ColumnType(Types.Integer, true, true)));


            //Database.Relations.Add(new Relation(Database.Tables[0], Database.Tables[1], RelationType.OneToMany));
            //Database.Relations.Add(new Relation(Database.Tables[3], Database.Tables[2], RelationType.OneToMany));
            //Database.Relations.Add(new Relation(Database.Tables[2], Database.Tables[4], RelationType.OneToMany));
            //Database.Relations.Add(new Relation(Database.Tables[4], Database.Tables[5], RelationType.OneToMany));


            //SelectedTable = Database.Tables[0];


            Generator generator = new Generator(_database);
            generator.Generate();

            Deployer deployer = new Deployer(generator);
            deployer.Deploy();

            _generateDelegateCommand = new DelegateCommand(GenerateCommand);

            _importDelegateCommand = new DelegateCommand(ImportFromJsonCommand);


        }

        private void ImportFromJsonCommand()
        {
            OpenFileDialog openFileDialog;

            openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "Config.json";

            if (true == openFileDialog.ShowDialog())
            {
                Database = Database.ImportFromJson(IO.ReadFile(openFileDialog.FileName));
            }
        }

        private void GenerateCommand()
        {
            Generator generator = new Generator(_database);
            Database.HasAuthenticationSystem = SecurityToken;
            generator.Generate();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
