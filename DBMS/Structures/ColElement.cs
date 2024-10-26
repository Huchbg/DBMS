using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Structures
{
    public class ColElement
    {
        private string Name;
        public object Data;
        private object DefaultData;
        private Type Type;

        public ColElement(string name,Type type,object data = null)
        {
            Name = name;
            Type=type;
            Data = data;
            if(data != null )
            {
                Data = DefaultData;
            }
        }

        public void SetDefaultData(object data)
        {
            this.DefaultData = data;
        }

        public string GetName() { return Name; }
        public Type GetType() { return Type; }

        public object GetDefaultData() { return DefaultData; }


    }
}
