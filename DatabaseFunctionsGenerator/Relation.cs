using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public enum RelationType
    {
        OneToMany = 0,
        OneToOne = 1,
        ManyToMany = 2
    }

    public class Relation
    {
        private Table _table1;
        private Table _table2;
        private RelationType _type;

        public RelationType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Table Table2
        {
            get { return _table2; }
            set { _table2 = value; }
        }

        public Table Table1
        {
            get { return _table1; }
            set { _table1 = value; }
        }

        public Relation(Table table1, Table table2, RelationType type)
        {
            _table1 = table1;
            _table2 = table2;
            _type = type;
        }
    }
}
