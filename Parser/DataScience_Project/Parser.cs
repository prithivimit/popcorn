using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DataScience_Project
{
    class Parser
    {
        static void Main(string[] arg)
        {
            //Parse the businesses
            Dictionary<string, Business> businesses = new Dictionary<string, Business>();
            StreamReader reader = new StreamReader(@"F:\Data Science Project\businesses.txt");
            string line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("\"type\": \"business\""))
                {
                    JObject obj = JObject.Parse(line);
                    Business business = new Business();
                    business.name = obj["name"].ToString();
                    business.full_address = obj["full_address"].ToString();
                    business.city = obj["city"].ToString();
                    business.state = obj["state"].ToString();
                    business.latitude = double.Parse(obj["latitude"].ToString());
                    business.longitude = double.Parse(obj["longitude"].ToString());
                    business.stars = float.Parse(obj["stars"].ToString());
                    business.review_count = int.Parse(obj["review_count"].ToString());
                    business.categories = obj["categories"].ToString();
                    businesses.Add(obj["business_id"].ToString(), business);
                }
                line = reader.ReadLine();
            }

            //Parse the reviews
            reader = new StreamReader(@"F:\Data Science Project\reviews.txt");
            Dictionary<string, User> users = new Dictionary<string, User>();
            line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("\"type\": \"review\""))
                {
                    JObject obj = JObject.Parse(line);
                    if (businesses[obj["business_id"].ToString()].categories.Contains("Restaurants"))
                    {
                        if (users.ContainsKey(obj["user_id"].ToString()))
                        {
                            Review review = new Review();
                            review.text = RemoveSpecialCharacters(obj["text"].ToString());
                            review.business_id = obj["business_id"].ToString();
                            review.stars = float.Parse(obj["stars"].ToString());
                            users[obj["user_id"].ToString()].reviews.Add(review);
                        }
                        else
                        {
                            Review review = new Review();
                            review.text = RemoveSpecialCharacters(obj["text"].ToString());
                            review.business_id = obj["business_id"].ToString();
                            review.stars = float.Parse(obj["stars"].ToString());
                            User user = new User();
                            user.reviews.Add(review);
                            users.Add(obj["user_id"].ToString(), user);
                        }
                        //Console.WriteLine(line);
                    }
                }
                line = reader.ReadLine();
            }

            //Parse the users
            reader = new StreamReader(@"F:\Data Science Project\users.txt");
            line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("\"type\": \"user\""))
                {
                    JObject obj = JObject.Parse(line);
                    if (users.ContainsKey(obj["user_id"].ToString()))
                    {
                        users[obj["user_id"].ToString()].name = obj["name"].ToString();
                        users[obj["user_id"].ToString()].average_stars = float.Parse(obj["average_stars"].ToString());
                        users[obj["user_id"].ToString()].review_count = int.Parse(obj["review_count"].ToString());
                        users[obj["user_id"].ToString()].funny_votes = int.Parse(obj["votes"]["funny"].ToString());
                        users[obj["user_id"].ToString()].useful_votes = int.Parse(obj["votes"]["useful"].ToString());
                        users[obj["user_id"].ToString()].cool_votes = int.Parse(obj["votes"]["cool"].ToString());
                    }
                }
                line = reader.ReadLine();
            }

            foreach (User user in users.Values)
            {
                if (user.reviews.Count > 0)
                {
                    Console.Write("\"");
                    foreach (Review review in user.reviews)
                    {
                        Console.Write(review.text + " ");
                    }
                    Console.Write("\"");
                    Console.WriteLine();
                }
            }

            //User target = users["FevBcg69uao1b4CSW-PKBw"];
        }
        public static string RemoveSpecialCharacters(string str)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'z' || (str[i] == '.' || str[i] == '_' || str[i] == ' ')))
                    sb.Append(str[i]);
                else if (str[i] == '\n')
                    sb.Append(' ');
            }

            return sb.ToString();
        }
    }

    class User
    {
        public User()
        {
            reviews = new List<Review>();
        }
        public List<Review> reviews { get; set; }
        public int funny_votes { get; set; }
        public int useful_votes { get; set; }
        public int cool_votes { get; set; }
        public string name { get; set; }
        public float average_stars { get; set; }
        public int review_count { get; set; }
    }

    class Review
    {
        public string text { get; set; }
        public string business_id { get; set; }
        public float stars { get; set; }
    }

    class Business
    {
        public string name { get; set; }
        public string full_address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public float stars { get; set; }
        public int review_count { get; set; }
        public string categories { get; set; }
    }
}