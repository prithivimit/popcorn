using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataScience_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader(@"F:\Data Science Project\yelp_phoenix_academic_dataset");
            Dictionary<string, User> users = new Dictionary<string, User>();
            string line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("\"type\": \"review\""))
                {
                    Console.WriteLine(line);
                }
                line = reader.ReadLine();
            }
        }
    }
}
