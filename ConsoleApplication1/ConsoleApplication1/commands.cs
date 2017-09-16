using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class commands
    {
        public void terminate()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            Console.Write("> ");
        }
        public static int manager(string comm, http_server server)
        {            
            int res = 0;
            switch (comm)
            {
                case "run":
                    try
                    {
                        Task sstarts = new Task(() => server.server_start());
                        sstarts.Start();
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                    Console.WriteLine(DateTime.Now.ToString() + " Stars listening.");                    
                    break;
                case "stop":
                    try
                    {
                        server.server_stop();
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;                        
                        Console.WriteLine(e.Message);
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(DateTime.Now.ToString() + " Stoped.");
                    break;
                case "exit":
                    try
                    {
                        server.server_close();
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;                        
                        Console.WriteLine(e.Message);
                        break;
                    }
                    res = -1;
                    break;
                case "status": 
                    Console.WriteLine(DateTime.Now.ToString() + " " + server.server_status());
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;  
                    Console.WriteLine("Unimplemented command. " + comm);
                    break;
            }
            Console.ResetColor();
            return res;
        }

    }
}
