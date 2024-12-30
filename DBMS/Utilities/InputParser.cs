using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Utilities
{
    internal class InputParser
    {
        public void RUN()
        {
            Console.WriteLine("Welcomee to ToyDBMS/SQL Input Console\n");
            while (true)
            {
                // CreateTable Sample2(Id:int, Name:string, BirthDate:date default “01.01.2022”) // Works
                // Insert INTO Sample2 (Id,Name) VALUES (1,“Ivan”) // Works
                // Insert INTO Sample2 (Id,Name) VALUES (2,“Petar”) // Works
                // Insert INTO Sample2 (Id,Name,BirthDate) VALUES (3,“Georgi”,"02.02.2022") // Works
                // Insert INTO Sample2 (BirthDate,Id,Name) VALUES ("03.03.2022",4,"Meesho") // Works
                // Insert INTO Sample2 (BirthDate,Id,Name) VALUES ("04.04.2022",5,"Spas") // Works
                // Insert INTO Sample2 (BirthDate,Id,Name) VALUES ("06.09.1999",69,"Evala")// Works

                // CreateTable SampleJoin(Id:int, AvgGrade:int, ClassName:string) // Works
                // INSERT INTO SampleJoin (Id,AvgGrade,ClassName) VALUES (1,5.50,"Class A") // Works

                // Select Name, BirthDate FROM Sample WHERE Id <> 5 AND BirthDate > "01.01.2000" // Works
                // Select Name, BirthDate FROM Sample WHERE Id <> 5 AND ( BirthDate > "01.01.2000" OR Name = "Ivan" ) // Works
                // Select Name, BirthDate FROM Sample WHERE Id <> 3 AND BirthDate > "01.01.2002" // Works
                // Select Distinct * FROM Sample WHERE Id < 5 OR BirthDate > "01.01.2004" // Works
                // Select * FROM Sample WHERE Id < 5 OR BirthDate > "01.01.2004"
                // Select Name, Id FROM Sample // Works
                // Select Id, Name FROM Sample // Works
                // Select Name, Dupe FROM Sample // Works

                //Select Distinct Name, BirthDate FROM Sample WHERE Id <> 5 AND BirthDate > "01.01.2000" // Works
                //Select Distinct * FROM Sample WHERE Id <> 5 AND BirthDate > "01.01.2000" // Works
                //Select Distinct Name, BirthDate FROM Sample // Works
                //Select Distinct * FROM Sample // Works
                //Select Distinct * FROM Sample ORDER BY Name DESC // Works
                //Select Name, BirthDate FROM Sample ORDER BY Id DESC // Works
                //Select Distinct Name, BirthDate FROM Sample ORDER BY Id DESC // Works
                //Select Distinct * FROM Sample ORDER BY Id DESC // Works
                //Select Distinct * FROM Sample ORDER BY BirthDate DESC // Works

                //SELECT DISTINCT Name, BirthDate FROM Sample WHERE Id <> 5 AND BirthDate > "01.01.2000" ORDER BY Name // Works
                //SELECT DISTINCT Name, BirthDate FROM Sample WHERE Id <> 5 AND BirthDate > "01.01.2000" ORDER BY Name ASC // Works
                //SELECT DISTINCT Name, BirthDate FROM Sample WHERE Id <> 5 AND BirthDate > "01.01.2000" ORDER BY Name DESC // Works
                //SELECT DISTINCT * FROM Sample WHERE Id <> 5 AND BirthDate > "05.04.2003" ORDER BY Id DESC // Works

                //DELETE FROM Sample WHERE Id = 8 OR Name = "Petar" // Works
                //DELETE FROM SampleCopy WHERE Name = "Petar" OR Id > 8 // Works

                //CREATEINDEX bd_index ON Sample2 (BirthDate) // Works
                //DROPINDEX bd_index ON Sample (BirthDate) // Works
                //CREATEINDEX id_index ON Sample (Id) // Works

                // Insert INTO Sample (Id,Name) VALUES (70,"Test")

                Console.WriteLine("Enter a command: ");

                string userInput=Console.ReadLine();    
                var splitInput=Utils.Split(userInput, ' ',2);

                switch (Utils.ToUpper(splitInput[0]))
                {
                    case "CREATETABLE":
                        FrontendCommands.CreateTable(splitInput[1]);
                        break;
                    case "DROPTABLE":
                        FrontendCommands.DeleteTable(splitInput[1]);
                        break;
                    case "LISTTABLES":
                        FileManager.GetTableNames();
                        break;
                    case "TABLEINFO":
                        FileManager.GetTableInfo(splitInput[1]);
                        break;
                    case "SELECT":
                        FrontendCommands.Select(splitInput[1]);
                        break;
                    case "INSERT":
                        FrontendCommands.Insert(splitInput[1]);
                        break;
                    case "HELP":
                        Console.WriteLine("Available Commands: CREATETABLE, DROPTABLE, LISTTABLES, TABLEINFO, SELECT, INSERT, DELETE");
                        break;
                    case "DELETE":
                        FrontendCommands.Delete(splitInput[1]);
                        break;
                    case "CREATEINDEX":
                        FileManager.CreateIndex(splitInput[1]);
                        break;
                    case "DROPINDEX":
                        FileManager.DeleteIndex(splitInput[1]);
                        break;
                    case "STOP":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid Command. If you want to exit type STOP");
                        break;
                }
            
            }
        }
    }
}
