using DBMS.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Utilities
{
    public static class FileManager
    {
        public const string tablePath = "../../../Tables";
        public const string indexesPath = "../../../Indexes";

        public static void CreateTableFile(Table table)
        {
            if (File.Exists($"{tablePath}/{table.Name}.txt"))
            {
                Console.WriteLine("A Table with that Name already exists.");
                return;
            }

            using (StreamWriter sw = File.CreateText($"{tablePath}/{table.Name}.txt"))
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    var defaultValue=string.Empty;
                    if (table.Columns.ElementAt(i).Value.GetDefaultData() != null) {
                        defaultValue = $" {table.Columns.ElementAt(i).Value.GetDefaultData()}";
                    }
                        
                    if(table.Columns.ElementAt(i).NextNode!=null){
                        sw.Write(table.Columns.ElementAt(i).Value.GetName() + $":{table.Columns.ElementAt(i).Value.GetType()}" + defaultValue + "\t");
                    }
                    else
                    {
                        sw.Write(table.Columns.ElementAt(i).Value.GetName() + $":{table.Columns.ElementAt(i).Value.GetType()}" + defaultValue);
                    }
                }
            }

            Console.WriteLine("\nTable Created\n");
        }
    }
}
