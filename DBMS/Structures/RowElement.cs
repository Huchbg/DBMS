using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Structures
{
    public class RowElement
    {
        public CustomLinkedList<ColElement> Values;
        public RowElement(CustomLinkedList<ColElement> values)
        {
            this.Values = values;
        }
    }
}
