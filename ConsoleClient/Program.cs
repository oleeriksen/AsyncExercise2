using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {

        static void Main(string[] args)
        {

            HttpClient _client = new HttpClient();
            List<Task> mTasks = new();
            int number = GetNumber("Enter a number (0 for stop): ");
            while (number != 0)
            {
                DateTime start = DateTime.Now;

                Task<HttpResponseMessage> response = _client.GetAsync("https://localhost:5003/numbers/" + number);

                Task t = new Task(() => {
                    String result = (response.Result.Content.ReadAsStringAsync().Result);
                    WriteToFile("Result: " + result + " between " + start.ToLongTimeString() + " - " + DateTime.Now.ToLongTimeString());
                });
                t.Start();
                mTasks.Add(t);
                
                number = GetNumber("Enter a number (0 for stop): ");
            }
            // vent på at alle er færdige
            Console.WriteLine("We are closing...");
            foreach (var t in mTasks) t.Wait();

            Console.WriteLine("Done...");
            Console.ReadKey();
        }


        private static int GetNumber(String text)
        {
            Console.WriteLine(text);
            return int.Parse(Console.ReadLine());
        }

        private static Object mLock = new object();


        private static void WriteToFile(string s)
        {
            string path = @"fromclient.txt";

            lock(mLock) {

                StreamWriter sw = File.AppendText(path);

                sw.WriteLine(s);

                sw.Flush();
            }
        }
    }
}
