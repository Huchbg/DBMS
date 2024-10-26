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
                Console.WriteLine("Enter a command: ");

                string userInput=Console.ReadLine();    
                var splitInput=Utils.Split(userInput, ' ',2);

                switch (Utils.ToUpper(splitInput[0]))
                {
                    case "CREATETABLE":
                        FrontendCommands.CreateTable(splitInput[1]);
                        break;
                    case "DROPTABLE":
                        Console.WriteLine("Deletes table");
                        break;
                    case "LISTTABLES":
                        Console.WriteLine("Shows tables");
                        break;
                    case "TABLEINFO":
                        Console.WriteLine("Shows table info");
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
                        Console.WriteLine("Creates index");
                        break;
                    case "DROPINDEX":
                        Console.WriteLine("drops index");
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
