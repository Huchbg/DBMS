using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Structures
{
    public class Table
    {
        public string Name;
        public CustomLinkedList<ColElement> Columns=new CustomLinkedList<ColElement>();
        public CustomLinkedList<RowElement> Rows = new CustomLinkedList<RowElement>();

        public Table(string name, CustomLinkedList<ColElement> columns)
        {
            Name = name;
            Columns = columns;
        }

        public string[] GetColumnsNames()
        {
            string[] colNames = new string[Columns.Count];
            var current = Columns.First();
            int i = 0;
            while (current != null)
            {
                colNames[i] = current.Value.GetName();
                current = current.NextNode;
                i++;
            }
            return colNames;
        }
    }
}
