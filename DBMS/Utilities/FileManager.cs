using DBMS.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBMS.Utilities
{
    public static class FileManager
    {
        public const string tablePath = "../../../SavedInfo/Tables";
        public const string indexesPath = "../../../../SavedInfo/Indexes";

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
        public static void DeleteTableFile(string Name) {
            if (!File.Exists($"{tablePath}/{Name}.txt"))
            {
                Console.WriteLine("A Table with that Name doesn't exist.");
                return;
            }

            File.Delete($"{tablePath}/{Name}.txt");
            Console.WriteLine($"\nRemoved Table {Name}\n");
        }

        public static void InsertIntoTable(string Name, string[] selectedCols, string[] selectedValues)
        {
            // Check if the specified table file exists
            if (!File.Exists($"{tablePath}/{Name}.txt"))
            {
                Console.WriteLine("A Table with that Name doesn't exist.");
                return;
            }

            // Read the column definitions (header line) from the table file
            string[] colLines;
            using (StreamReader sr = new StreamReader($"{tablePath}/{Name}.txt"))
            {
                colLines = Utils.Split(sr.ReadLine(), '\t');  // Read and split columns by tab
            }

            // Initialize a custom linked list to hold column information for the table
            var tableCols = new CustomLinkedList<ColElement>();

            // Iterate through each column definition to parse and store column name, type, and default value
            for (int i = 0; i < colLines.Length; i++)
            {
                var colValues = Utils.Split(colLines[i], ':');  // Split by ':' to get column name and type/default
                var colDefaultValue = Utils.Split(colValues[1], ' ');  // Further split by space for default value if present

                // Determine the column's data type
                Type type = null;
                switch (colValues[1])
                {
                    case "System.Int32":
                        type = typeof(int);
                        break;
                    case "System.String":
                        type = typeof(string);
                        break;
                    case "System.DateTime":
                        type = typeof(DateTime);
                        break;
                }

                // Check if a default value is specified; if so, initialize ColElement with it
                if (colDefaultValue.Length != 1)
                {
                    tableCols.AddLast(new ColElement(colValues[0], type, colDefaultValue[1]));
                    tableCols.ElementAt(i).Value.SetDefaultData(colDefaultValue[1]);  // Set the default data in the element
                }
                else
                {
                    tableCols.AddLast(new ColElement(colValues[0], type));  // Add column without a default value
                }
            }

            // Create a linked list to store the row's data (values for each column)
            bool validCol = true;
            var rowValues = new CustomLinkedList<ColElement>();

            // Loop through each column in tableCols to find and add corresponding values from selectedCols
            for (int i = 0; i < tableCols.Count; i++)
            {
                for (int k = 0; k < selectedCols.Length; k++)
                {
                    // Check if selected column name matches current table column name
                    if (selectedCols[k] != tableCols.ElementAt(i).Value.GetName())
                    {
                        validCol = false;
                    }
                    else
                    {
                        // Create a new ColElement with the matched name, type, and provided value
                        ColElement col = new ColElement(tableCols.ElementAt(i).Value.GetName(), tableCols.ElementAt(i).Value.GetType(), selectedValues[k]);
                        rowValues.AddLast(col);  // Add to row values
                        validCol = true;
                        break;  // Move to the next column in tableCols
                    }
                }

                // If no match found and no default value exists, print error and exit
                if (!validCol)
                {
                    if (tableCols.ElementAt(i).Value.DefaultData != null)
                    {
                        ColElement col = new ColElement(tableCols.ElementAt(i).Value.GetName(), tableCols.ElementAt(i).Value.GetType(), tableCols.ElementAt(i).Value.DefaultData);
                        rowValues.AddLast(col);
                    }
                    else
                    {
                        Console.WriteLine("Wrong Input");
                        return;
                    }
                }
            }

            // Append the new row values to the table file
            using (StreamWriter sw = File.AppendText($"{tablePath}/{Name}.txt"))
            {
                sw.WriteLine();  // Start a new line for the new row
                for (int i = 0; i < rowValues.Count; i++)
                {
                    if (rowValues.ElementAt(i).NextNode == null)  // Check if it's the last column in the row
                        sw.Write(rowValues.ElementAt(i).Value.Data);
                    else
                        sw.Write(rowValues.ElementAt(i).Value.Data + "\t");  // Separate values by tabs
                }

                Console.WriteLine("\nEntry Added \n");  // Confirm addition
            }

            // Update any table indexes if needed
            //UpdateTableIndexes(Name);
        }
        public static void CreateIndex(string input)
        {
            //bd_index ON Sample (BirthDate)

            var splitInput = Utils.Split(input, ' ');

            if (Utils.ToUpper(splitInput[1]) != "ON")
            {
                Console.WriteLine("ON not detected");
                return;
            }

            string filePath = $"{tablePath}/{splitInput[2]}.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("This Table doesn't exist");
                return;
            }

            //splitinput[3] = splitinput[3].Trim('(', ')');
            char[] chars = new char[2] { '(', ')' };
            splitInput[3] = Utils.Trim(splitInput[3], chars);

            // Get the column information in the Table

            string[] colLines;

            using (StreamReader sr = new StreamReader($"{tablePath}/{splitInput[2]}.txt"))
            {
                colLines = Utils.Split(sr.ReadLine(), '\t');
            }

            bool foundCol = false;
            int colIndex = -1;

            // Checks if the column is present in the selected table and raises a flag if it is
            for (int i = 0; i < colLines.Length; i++)
                if (splitInput[3] == Utils.Split(colLines[i], ':')[0])
                {
                    foundCol = true;
                    colIndex = i;
                    break;
                }

            if (!foundCol)
            {
                Console.WriteLine("Indexing Error: Column Name not found");
                return;
            }

            // Check if there is an existing index and returns if it does
            if (File.Exists($"{indexesPath}/{splitInput[2]}_{splitInput[3]}_{splitInput[0]}.txt"))
            {
                Console.WriteLine("Indexing Error: The Index already exists");
                return;
            }

            // Saves the index 
            using (StreamWriter sw = File.CreateText($"{indexesPath}/{splitInput[2]}_{splitInput[3]}_{splitInput[0]}.txt"))
            {
                using (StreamReader sr = new StreamReader($"{tablePath}/{splitInput[2]}.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        // Writes directly into the new file the index selecte while splitting each line
                        sw.WriteLine(Utils.Split(sr.ReadLine(), '\t')[colIndex]);
                    }
                }
            }

            Console.WriteLine("Index Created");
        }

        public static void DeleteIndex(string input)
        {
            //bd_index ON Sample (BirthDate)

            var splitInput = Utils.Split(input, ' ');

            if (Utils.ToUpper(splitInput[1]) != "ON")
            {
                Console.WriteLine("ON not detected");
                return;
            }

            //splitinput[3] = splitinput[3].Trim('(', ')');
            char[] chars = new char[2] { '(', ')' };
            splitInput[3] = Utils.Trim(splitInput[3], chars);

            if (File.Exists($"{indexesPath}/{splitInput[2]}_{splitInput[3]}_{splitInput[0]}.txt"))
            {
                File.Delete($"{indexesPath}/{splitInput[2]}_{splitInput[3]}_{splitInput[0]}.txt");
                Console.WriteLine("Index Deleted");
                return;
            }

            Console.WriteLine("Index not Found");
        }
        public static void UpdateTableIndexes(string Name)
        {
            var fileNames = Directory.GetFiles(indexesPath);

            for (int i = 0; i < fileNames.Length; i++)
            {
                // using the Path command we get the full file name without extensions and paths
                var splitName = Utils.Split(Path.GetFileNameWithoutExtension(fileNames[i]), '_', 3);

                // Sample BirthDate bd_index

                if (splitName[0] == Name)
                {
                    string[] colLines;
                    using (StreamReader sr = new StreamReader($"{tablePath}/{Name}.txt"))
                    {
                        colLines = Utils.Split(sr.ReadLine(), '\t');
                    }

                    for (int k = 0; k < colLines.Length; k++){
                        if (splitName[1] == Utils.Split(colLines[k], ':')[0])
                        {
                            File.Delete(fileNames[i]);
                            // Saves the index 
                            using (StreamWriter sw = File.CreateText($"{indexesPath}/{Name}_{splitName[1]}_{splitName[2]}.txt"))
                            {
                                using (StreamReader sr = new StreamReader($"{tablePath}/{Name}.txt"))
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        // Writes directly into the new file the index selecte while splitting each line
                                        sw.WriteLine(Utils.Split(sr.ReadLine(), '\t')[k]);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static int TableFilesCount()
        {
            return Directory.GetFiles(tablePath).Length;
        }

        public static string? GetTableNames()
        {
            if (FileManager.TableFilesCount() == 0)
                return null;

            var files = Directory.GetFiles(tablePath);

            string tableInfo = "The Available Tables are: ";

            for (int i = 0; i < files.Length; i++)
            {
                tableInfo += Path.GetFileNameWithoutExtension(files[i]);
                if (i != files.Length - 1)
                    tableInfo += ", ";
            }

            return tableInfo;
        }

        public static string[]? GetTableInfo(string Name, out string info)
        {
            if (!File.Exists($"{tablePath}/{Name}.txt"))
            {
                Console.WriteLine("This Table doesn't exist");
                info = null;
                return null;
            }

            var lines = File.ReadAllLines($"{tablePath}/{Name}.txt");

            info = new FileInfo($"{tablePath}/{Name}.txt").Length.ToString();

            return lines;
        }
    }
}
