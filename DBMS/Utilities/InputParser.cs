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

                // CreateTable SampleJoin(Id:int, AvgGrade:int, ClassName:string) // Works
                // INSERT INTO SampleJoin (Id,AvgGrade,ClassName) VALUES (1,5.50,"Class A") // Works

                // CreateTable Sample2(Id:int, Name:string, BirthDate:date default “01.01.2022”) // Works
                // Insert INTO Sample2 (Id,Name) VALUES (1,“Ivan”) // Works
                // Insert INTO Sample2 (Id,Name) VALUES (2,“Petar”) // Works
                // Insert INTO Sample2 (Id,Name,BirthDate) VALUES (3,“Georgi”,"02.02.2022") // Works
                // Insert INTO Sample2 (BirthDate,Id,Name) VALUES ("03.03.2022",4,"Kris") // Works
                // Insert INTO Sample2 (BirthDate,Id,Name) VALUES ("04.04.2022",5,"Petkan") // Works
                // Insert INTO Sample2 (BirthDate,Id,Name) VALUES ("06.09.1999",69,"Nincho")// Works


                // Select Name, Id FROM Sample2 // Works
                // Select Id, Name FROM Sample2 // Works
                // Select Name, Something FROM Sample2 // Works

                //Select Distinct Name, BirthDate FROM Sample2 // Works
                //Select Distinct * FROM Sample2 // Works
                //Select Distinct * FROM Sample2 ORDER BY Name DESC // Works
                //Select Name, BirthDate FROM Sample2 ORDER BY Id DESC // Works
                //Select Distinct Name, BirthDate FROM Sample2 ORDER BY Id DESC // Works
                //Select Distinct * FROM Sample2 ORDER BY Id DESC // Works
                //Select Distinct * FROM Sample2 ORDER BY BirthDate DESC // Works



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
