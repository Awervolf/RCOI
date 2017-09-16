using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "HTTP test server";
            http_server test_server = new http_server();
            commands com_pars = new commands();
            string input;
            int flag = 0;

            while (flag == 0)
            {
                com_pars.terminate();
                input = Console.ReadLine();
                Task<int> managetask = new Task<int>(() => commands.manager(input, test_server));
                managetask.Start();
                while ((managetask.Status != TaskStatus.RanToCompletion) && (managetask.Status != TaskStatus.Faulted) && (managetask.Status != TaskStatus.Canceled))
                {                    
                }
                flag = managetask.Result;
            } 

        }
    }
}
