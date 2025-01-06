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
        private static bool distinctFlag = false;
        private static int orderByFlag = 0;
        private static Type orderColType = null;
        private static int orderColIndex = -1;
        private static CustomLinkedList<ulong> rowHash = null;

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
            UpdateTableIndexes(Name);
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

        public static void GetTableNames()
        {
            if (FileManager.TableFilesCount() == 0)
            {
                Console.WriteLine("\nThere are no available Tables\n");
                return;
            }

            Console.WriteLine("\nThe Available Tables are:\n");

            var files = Directory.GetFiles(tablePath);

            for (int i = 0; i < files.Length; i++)
                Console.WriteLine(Path.GetFileNameWithoutExtension(files[i]));
            Console.WriteLine();
        }

        public static void GetTableInfo(string Name)
        {
            if (!File.Exists($"{tablePath}/{Name}.txt"))
            {
                Console.WriteLine("This Table doesn't exist");
                return;
            }

            var lines = File.ReadAllLines($"{tablePath}/{Name}.txt");

            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine(lines[i]);
            }

            Console.WriteLine($"The Entries in the Table are {lines.Length - 1}");

            FileInfo info = new FileInfo($"{tablePath}/{Name}.txt");

            Console.WriteLine($"File Size is : {info.Length} bytes");
        }

        public static void SelectInTable(string Name, string[] inputValues, string[]? conditions = null)
        {
            string filepath = $"{tablePath}/{Name}.txt";

            if (!File.Exists(filepath))
            {
                Console.WriteLine("This Table doesn't exist");
                return;
            }

            distinctFlag = false;
            // flag = 0 -> No order by
            // flag = 1 -> order by ASC
            // flag = -1 -> order by DESC
            orderByFlag = 0;
            string[] inputCols;
            string orderColName = string.Empty;
            orderColType = null;
            // Order By Check
            if (conditions != null)
            {
                if (Utils.ToUpper(conditions[conditions.Length - 3]) == "ORDER" || Utils.ToUpper(conditions[conditions.Length - 3]) == "BY")
                {
                    orderByFlag = 1;
                    // 0 = ASC(Default) || 1 = DESC             
                    // Checks if ASC OR DESC is available
                    if (Utils.ToUpper(conditions[conditions.Length - 1]) != "DESC" && Utils.ToUpper(conditions[conditions.Length - 1]) != "ASC")
                    {
                        orderColName = conditions[conditions.Length - 1];
                        conditions = Utils.Slice(conditions, 0, conditions.Length - 3);
                        if (conditions.Length == 0)
                            conditions = null;
                    }
                    else
                    {
                        orderColName = conditions[conditions.Length - 2];
                        if (Utils.ToUpper(conditions[conditions.Length - 1]) == "DESC")
                            orderByFlag = -1;
                        conditions = Utils.Slice(conditions, 0, conditions.Length - 4);
                        if (conditions.Length == 0)
                            conditions = null;
                    }
                }
            }

            // Distinct Check
            if (Utils.ToUpper(inputValues[0]) == "DISTINCT")
            {
                distinctFlag = true;
                inputCols = new string[inputValues.Length - 1];
                for (int i = 0; i < inputCols.Length; i++)
                {
                    inputCols[i] = inputValues[i + 1];
                }
            }
            else
            {
                inputCols = inputValues;
            }

            string[] colLines;
            using (StreamReader sr = new StreamReader(filepath))
            {
                colLines = Utils.Split(sr.ReadLine(), '\t');
            }

            string[] colNames = new string[colLines.Length];
            string[] colTypes = new string[colLines.Length];
            for (int i = 0; i < colLines.Length; i++)
            {
                var colValues = Utils.Split(colLines[i], ':');

                colNames[i] = colValues[0];
                colTypes[i] = Utils.Split(colValues[1], ' ')[0];
            }

            // Check for Order by Col Index
            orderColIndex = -1;
            if (!Utils.Contains(colNames, orderColName) && orderByFlag != 0)
            {
                Console.WriteLine($"{orderColName} is not available in the given Table");
                return;
            }
            else
            {
                for (int k = 0; k < colNames.Length; k++)
                {
                    if (orderColName == colNames[k])
                    {
                        orderColIndex = k;
                        switch (colTypes[k])
                        {
                            case "System.Int32":
                                orderColType = typeof(int);
                                break;
                            case "System.String":
                                orderColType = typeof(string);
                                break;
                            case "System.DateTime":
                                orderColType = typeof(DateTime);
                                break;
                        }
                        break;
                    }
                }
            }

            int[] indexes;
            if (inputCols[0] == "*")
            {
                indexes = new int[colNames.Length];

                for (int i = 0; i < colNames.Length; i++)
                    indexes[i] = i;
            }
            else
            {
                indexes = new int[inputCols.Length];

                for (int i = 0; i < inputCols.Length; i++)
                {
                    if (!Utils.Contains(colNames, inputCols[i]))
                    {
                        Console.WriteLine($"{inputCols[i]} is not available in the given Table");
                        return;
                    }

                    for (int k = 0; k < colNames.Length; k++)
                    {
                        if (inputCols[i] == colNames[k])
                        {
                            indexes[i] = k;
                            break;
                        }
                    }
                }
            }
            rowHash = null;

            // Select..
            if (conditions == null)
            {
                Select(filepath, indexes, inputCols[0]);
            }
            else // Select... Where
            {

              
                Console.WriteLine("select ... where is not supported ");
                //  SelectWhere(filepath, collines, indexes, inputcols[0], conditions);
            }



        }

        private static void Select(string filepath, int[] indexes, string inputcol)
        {
            var lines = File.ReadAllLines(filepath);

            // Checks if Order by has Been Selected
            if (orderByFlag != 0)
            {
                CustomLinkedList<int> selectedIndexes = new CustomLinkedList<int>();
                CustomLinkedList<string> rows = new CustomLinkedList<string>();

                if (distinctFlag)
                {
                    rowHash = new CustomLinkedList<ulong>();
                    for (int i = 1; i < lines.Length; i++)
                    {
                        Distinct(lines[i], i - 1, Utils.Split(lines[i], '\t'), indexes, inputcol, ref rowHash, ref rows, ref selectedIndexes);
                    }
                }
                else
                {
                    if (inputcol == "*")
                    {
                        for (int i = 1; i < lines.Length; i++)
                            rows.AddLast(lines[i]);
                    }
                    else
                    {

                        for (int i = 1; i < lines.Length; i++)
                        {
                            var line = Utils.Split(lines[i], '\t');
                            string rowline = string.Empty;

                            for (int k = 0; k < indexes.Length; k++)
                                rowline += line[indexes[k]] + '\t';

                            rows.AddLast(rowline);
                        }
                    }

                }

                Utils.Sort(orderColType, Utils.Slice(lines, 1, lines.Length), rows, orderColIndex, selectedIndexes, orderByFlag);

                for (int i = 0; i < rows.Count; i++)
                {
                    Console.WriteLine(rows.ElementAt(i).Value);
                }

            }
            else
            {
                // Checks if Distinct has been Selected
                if (distinctFlag)
                {
                    CustomLinkedList<string> a = null;
                    CustomLinkedList<int> b = null;
                    rowHash = new CustomLinkedList<ulong>();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Distinct(lines[i], i, Utils.Split(lines[i], '\t'), indexes, inputcol, ref rowHash, ref a, ref b);
                    }
                }
                else
                {
                    if (inputcol == "*")
                    {

                        for (int i = 0; i < lines.Length; i++)
                            Console.WriteLine(lines[i]);
                    }
                    else
                    {

                        for (int i = 0; i < lines.Length; i++)
                        {
                            var line = Utils.Split(lines[i], '\t');

                            for (int k = 0; k < indexes.Length; k++)
                            {
                                Console.Write(line[indexes[k]] + '\t');
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private static void Distinct(string row, int rowIndex, string[]? rowValues, int[] indexes, string inputcol, ref CustomLinkedList<ulong> rowHash, ref CustomLinkedList<string> rows, ref CustomLinkedList<int> selectedIndexes)
        {
            if (rows != null)
            {
                if (!Utils.Contains(rowHash, UniqueHash(row)))
                {
                    rowHash?.AddLast(UniqueHash(row));
                    string rowline = string.Empty;
                    if (inputcol == "*")
                    {
                        for (int i = 0; i < rowValues.Length; i++)
                        {
                            rowline += rowValues[i] + "\t";
                        }

                        rows.AddLast(rowline);
                        selectedIndexes.AddLast(rowIndex);
                    }
                    else
                    {
                        for (int k = 0; k < indexes.Length; k++)
                        {
                            rowline += rowValues[indexes[k]] + '\t';
                        }

                        rows.AddLast(rowline);
                        selectedIndexes.AddLast(rowIndex);
                    }
                }
                else
                {

                }
            }
            else
            {
                if (!Utils.Contains(rowHash, UniqueHash(row)))
                {
                    rowHash?.AddLast(UniqueHash(row));

                    if (inputcol == "*")
                        for (int i = 0; i < rowValues.Length; i++)
                            Console.Write(rowValues[i] + "\t");
                    else
                        for (int k = 0; k < indexes.Length; k++)
                            Console.Write(rowValues[indexes[k]] + '\t');
                    Console.WriteLine();
                }
            }
        }

        static ulong UniqueHash(string s)
        {
            // Initialize the hash value to a prime number
            ulong hash = 17;

            // For each character in the string, update the hash value
            // using the following formula:
            // hash = (hash * 31) + c
            // The hash value is multiplied by the index of the character
            // in the string, so that the order of the characters matters
            // The hash value is also XORed with the Unicode code of the
            // character, to make the hash value more difficult to predict
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                hash = (hash * 31 * ((ulong)i + 1)) ^ c;
            }

            // Return the final hash value
            return hash;
        }

       
    }
}
