using DBMS.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DBMS.Utilities
{
    internal class FrontendCommands
    {
        static public void CreateTable(string input)
        {
            //  Sample2(Id:int, Name:string, BirthDate:date default “01.01.2022”)
            //becomes [Sample2, "Id:int, Name:string, BirthDate:date default “01.01.2022”"]
            var trimmedInput =Utils.Split(input,new char[] { '(',')' });

            //becomes [ Id:int, Name:string, BirthDate:date default “01.01.2022” ]
            var colAttributes = Utils.Split(trimmedInput[1], ',');

            var cols = new CustomLinkedList<ColElement>();

            for(int i = 0;i<colAttributes.Length;i++)
            {
                colAttributes[i] = Utils.TrimStart(colAttributes[i]);

                //becomes [ BirthDate, date , default , “01.01.2022” ]
                var col = Utils.Split(colAttributes[i], new char[] { ':', ' ' });

                Type type=null;

                switch (Utils.ToUpper(col[1]))
                {
                    case "INT":
                        type=typeof(int);
                        break;
                    case "STRING":
                        type=typeof(string);
                        break;
                    case "DATA":
                        type=typeof(DateTime);
                        break;
                    default:
                        break;
                }

                if(col.Length>2 && Utils.ToUpper(col[2]) == "DEFAULT")
                {
                    ColElement col1 = new ColElement(col[0], type);
                    col1.SetDefaultData(col[3]);
                    cols.AddLast(col1);
                    continue;
                }

                ColElement colElement = new ColElement(col[0], type);
                cols.AddLast(colElement);
            }
            FileManager.CreateTableFile(new Table( trimmedInput[0], cols));

        }
        static public void Insert(string input)
        {
            // INTO Sample (Id,Name) VALUES (1,“Kris”)

            // becomes[INTO,Sample, (Id,Name) , Values , (1,"Kris")]
            var splitInput = Utils.Split(input, ' ');

            if (Utils.ToUpper(splitInput[0]) != "INTO")
            {
                Console.WriteLine("Invalid Input, Into expected");
                return;
            }

            if (Utils.ToUpper(splitInput[3]) != "VALUES")
            {
                Console.WriteLine("Invalid Input, Values expected");
                return;
            }

            var trimmedColsString = Utils.TrimEnd(Utils.TrimStart(splitInput[2], '('), ')');
            var selectedCols = Utils.Split(trimmedColsString, ',');

            var trimmedValues = Utils.TrimEnd(Utils.TrimStart(splitInput[4], '('), ')');
            var values = Utils.Split(trimmedValues, ',');

            if (selectedCols.Length != values.Length)
            {
                Console.WriteLine("Incorrect amount of inputs");
                return;
            }

            //TODO: FILEMANAGER INSERT INTO TABLE
        }
        static public void Delete(string input)
        {
            //FROM SampleTable WHERE condition
            //becomes [FROM , SampleTable , WHERE , condition]
            var splitInput = Utils.Split(input, ' ', 4);

            if (Utils.ToUpper(splitInput[0]) != "FROM")
            {
                Console.WriteLine("FROM not found");
                return;
            }

            if (Utils.ToUpper(splitInput[0]) != "WHERE")
            {
                Console.WriteLine("WHERE not found");
                return;
            }

            //TODO: FILEMANAGER DELETE IN TABLE 
        }
        static public void Select(string input)
        {
            // Name, BirthDate FROM Sample WHERE Id<> 5 AND BirthDate > "01.01.2000" ORDER BY Id DESC
            //becomes [ Name, BirthDate , FROM , Sample , WHERE , Id<> , 5 , AND , BirthDate , > , "01.01.2000" , ORDER , BY , Id , DESC ]

            int index = 0; // Index to track the position of the "FROM" keyword
            var splitInput = Utils.Split(input, ' '); // Split the input string by spaces
            int flag = 1; // Flag to track the parsing state

            for (int i = 0; i < splitInput.Length; i++)
            {
                // If we're looking for the "FROM" keyword
                if (flag == 1)
                {
                    if (Utils.ToUpper(splitInput[i]) == "FROM")
                    {
                        index = i; // Record the index of "FROM"
                        flag++; // Move to the next state
                        continue; // Continue to the next iteration
                    }
                    // Trim trailing commas from the current column name
                    splitInput[i] = Utils.TrimEnd(splitInput[i], ',');
                }
                else // We're past the "FROM" keyword
                {
                    // Check for "WHERE" keyword
                    if (Utils.ToUpper(splitInput[i]) == "WHERE")
                    {
                        flag++; // Move to the condition parsing state
                        break; // Exit the loop as we don't need to check further
                    }

                    // Check for "ORDER" keyword
                    if (Utils.ToUpper(splitInput[i]) == "ORDER")
                    {
                        flag = 4; // Set flag to indicate we're parsing for ordering
                        break; // Exit the loop as we don't need to check further
                    }
                }
            }

            // Extract columns specified in the SELECT statement
            //[  Name, BirthDate ]
            var inputCols = Utils.Slice(splitInput, 0, index);

            string[]? conditions = null; // Initialize conditions variable to store filtering conditions

            // If we're in the condition parsing state
            if (flag == 3)
            {
                //if no ORDER

                //becomes conditions = [ Id<> , 5 , AND , BirthDate , > , "01.01.2000"  ]
                // Slice the input for conditions after "WHERE"
                conditions = Utils.Slice(splitInput, index + 3);
            }

            // If we're in the ordering state
            if (flag == 4)
            {
                //if ORDER

                //becomes conditions = [ WHERE , Id<> , 5 , AND , BirthDate , > , "01.01.2000" , ORDER , BY , Id , DESC ]
                // Slice the input for conditions after "ORDER"
                conditions = Utils.Slice(splitInput, index + 2);
            }

            //TODO: SELECT FROM TABLE WITH FILE MANAGER
        }

    }
}
