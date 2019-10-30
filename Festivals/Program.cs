using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Festivals
{
    class Program
    {
        static void Main(string[] args)
        {
            int attempt = 1;

            MusicFestivals musicFestivals = new MusicFestivals();
            Console.WriteLine("Connecting to API server...Attempt: "+ attempt+ ". Please wait...\n");

            //Funtion below calls again if a false is returned and count < 6
            //This resolves the problems expeienced with the different types of 
            //JSON object returned from URL.
            while(!musicFestivals.GetApi() && attempt < 6){
                attempt++;
                Console.WriteLine("\nProblem in establishing contact. Trying again... Attempt: " + attempt + ". Please wait...\n");
                Thread.Sleep(300);
            }
            if(attempt > 4)
                Console.WriteLine("\nAPI server is down. Please try again later.");
            Console.Read();
        }
    }
}
