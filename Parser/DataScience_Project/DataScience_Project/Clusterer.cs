using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using weka.core;
using weka.clusterers;
using weka.filters.unsupervised.attribute;

namespace DataScience_Project
{
    class Clusterer
    {
        static void Main(string[] arg)
        {
            //Parse the businesses
            Dictionary<string, Business> businesses = new Dictionary<string, Business>();
            StreamReader reader = new StreamReader(@"F:\Data Science Project\businesses.txt");
            HashSet<string> set = new HashSet<string>();
            string line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("\"type\": \"business\""))
                {
                    JObject obj = JObject.Parse(line);
                    if (obj["categories"].ToString().Contains("Restaurants"))
                    {
                        JArray array = JArray.Parse(obj["categories"].ToString());
                        foreach (JValue val in array)
                        {
                            set.Add(val.ToString());
                        }
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
                    if (businesses.ContainsKey(obj["business_id"].ToString()) && businesses[obj["business_id"].ToString()].categories.Contains("Restaurants"))
                    {
                        Review review = new Review();
                        if (users.ContainsKey(obj["user_id"].ToString()))
                        {
                            review.text = RemoveSpecialCharacters(obj["text"].ToString());
                            review.business_id = obj["business_id"].ToString();
                            review.stars = float.Parse(obj["stars"].ToString());
                            review.user_id = obj["user_id"].ToString();
                            users[obj["user_id"].ToString()].reviews.Add(review);
                        }
                        else
                        {
                            review.text = RemoveSpecialCharacters(obj["text"].ToString());
                            review.business_id = obj["business_id"].ToString();
                            review.stars = float.Parse(obj["stars"].ToString());
                            review.user_id = obj["user_id"].ToString();
                            User user = new User();
                            user.reviews.Add(review);
                            users.Add(obj["user_id"].ToString(), user);
                        }
                        businesses[obj["business_id"].ToString()].reviews.Add(review);
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
                        users[obj["user_id"].ToString()].user_id = obj["user_id"].ToString();
                        users[obj["user_id"].ToString()].average_stars = float.Parse(obj["average_stars"].ToString());
                        users[obj["user_id"].ToString()].review_count = int.Parse(obj["review_count"].ToString());
                        users[obj["user_id"].ToString()].funny_votes = int.Parse(obj["votes"]["funny"].ToString());
                        users[obj["user_id"].ToString()].useful_votes = int.Parse(obj["votes"]["useful"].ToString());
                        users[obj["user_id"].ToString()].cool_votes = int.Parse(obj["votes"]["cool"].ToString());
                    }
                }
                line = reader.ReadLine();
            }

            
            int z = 0;
            foreach (KeyValuePair<string, Business> business in businesses)
            {
                z++;
                //Console.WriteLine(++z);
                //if (z == 10) break;
                //KeyValuePair<string, Business> business = new KeyValuePair<string, Business>("3vKhV2ELR2hmwlnoNqYWaA", businesses["3vKhV2ELR2hmwlnoNqYWaA"]);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("@relation reviews");
                sb.AppendLine("@attribute user_id string");
                sb.AppendLine("@attribute text string");
                sb.AppendLine("@attribute stars numeric");
                sb.AppendLine("@data");
                foreach (Review review in business.Value.reviews)
                {
                    sb.Append(review.user_id + ",\"" + review.text + "\"" + "," + review.stars);
                    sb.AppendLine("");
                }
                Instances instances = new Instances(new java.io.StringReader(sb.ToString()));
                //Create nominal filter for the user_id attribute
                StringToNominal nominalFilter = new StringToNominal();
                String[] options = weka.core.Utils.splitOptions("-R first");
                nominalFilter.setOptions(options);
                nominalFilter.setInputFormat(instances);
                //apply the filter
                instances = weka.filters.Filter.useFilter(instances, nominalFilter);

                //Create string to word vector filter for the text attribute
                StringToWordVector stwFilter = new StringToWordVector();
                options = weka.core.Utils.splitOptions("-R first-last -P att_ -W 1000 -prune-rate -1.0 -T -I -N 0 -L -stemmer weka.core.stemmers.NullStemmer -M 2 -tokenizer weka.core.tokenizers.WordTokenizer -delimiters \\r\\n\\t.,;:\\\'\\\"()?!\"\"");
                stwFilter.setOptions(options);
                stwFilter.setInputFormat(instances);
                //apply the filter
                instances = weka.filters.Filter.useFilter(instances, stwFilter);

                SimpleKMeans kmeansClusterer = new SimpleKMeans();
                options = weka.core.Utils.splitOptions("-N " + Math.Ceiling(business.Value.reviews.Count / (decimal)5).ToString() + " -A \"weka.core.EuclideanDistance -R first-last\" -I 5 -S 10");
                kmeansClusterer.setOptions(options);
                kmeansClusterer.setPreserveInstancesOrder(true);
                kmeansClusterer.buildClusterer(instances);
                int[] assignments = kmeansClusterer.getAssignments();

                for (int j = 0; j < assignments.Length; j++)
                {
                    for (int k = 0; k < assignments.Length; k++)
                    {
                        if (j != k && assignments[j] == assignments[k])
                        {
                            users[business.Value.reviews[j].user_id].friends.Add(business.Value.reviews[k].user_id);
                        }
                    }
                }

                /*int i = 0;
                foreach (int clusterNum in assignments)
                {
                    string str = "";
                    foreach (string u in users[business.Value.reviews[i].user_id].friends)
                    {
                        str += u + ",";
                    }
                    Console.WriteLine(clusterNum + "-->" + business.Value.reviews[i].user_id + "-->" + str);
                    i++;
                }*/

                

                /*EM emClusterer = new EM();
                options = weka.core.Utils.splitOptions("-I 100 -N -1 -M 1.0E-6 -S 100");
                emClusterer.setOptions(options);
                emClusterer.buildClusterer(instances);
                
                for (int i=0;i<business.Value.reviews.Count;i++)
                {
                    Console.WriteLine(emClusterer.clusterInstance(instances.instance(i)) + "-->" + business.Value.reviews[i].text);
                }*/

                //break;
            }

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement usersElt = xmlDoc.CreateElement("users");
            xmlDoc.AppendChild(usersElt);
            foreach (KeyValuePair<string, User> user in users)
            {
                if (user.Value.friends.Count == 0) continue;
                if (string.IsNullOrEmpty(user.Value.name))
                    user.Value.name = "No Name";
                XmlElement userElt = xmlDoc.CreateElement("user");

                //id attribute
                XmlAttribute idAtt = xmlDoc.CreateAttribute("id");
                idAtt.Value = user.Value.user_id;
                userElt.Attributes.Append(idAtt);

                //name
                XmlElement nameElt = xmlDoc.CreateElement("name");
                nameElt.InnerText = user.Value.name;
                userElt.AppendChild(nameElt);

                //friends
                XmlElement friendsElt = xmlDoc.CreateElement("friends");
                foreach (string user_id in user.Value.friends)
                {
                    XmlElement friendElt = xmlDoc.CreateElement("friend");
                    friendElt.InnerText = user_id;
                    friendsElt.AppendChild(friendElt);
                }
                userElt.AppendChild(friendsElt);

                //reviews
                XmlElement reviewsElt = xmlDoc.CreateElement("reviews");
                foreach (Review review in user.Value.reviews)
                {
                    XmlElement reviewElt = xmlDoc.CreateElement("review");
                    //text
                    XmlElement textElt = xmlDoc.CreateElement("text");
                    textElt.InnerText = review.text;
                    reviewElt.AppendChild(textElt);
                    
                    //lat
                    XmlElement latElt = xmlDoc.CreateElement("lat");
                    latElt.InnerText = businesses[review.business_id].latitude.ToString();
                    reviewElt.AppendChild(latElt);

                    //long
                    XmlElement longElt = xmlDoc.CreateElement("long");
                    longElt.InnerText = businesses[review.business_id].longitude.ToString();
                    reviewElt.AppendChild(longElt);

                    reviewsElt.AppendChild(reviewElt);

                    //break;
                }
                userElt.AppendChild(reviewsElt);
                usersElt.AppendChild(userElt);
            }

            StreamWriter userlistwriter = new StreamWriter(@"F:\Data Science Project\users\userlist.txt");
            userlistwriter.Write("<users>");
            XmlNodeList usersList = xmlDoc.DocumentElement.SelectNodes("//user");
            foreach (XmlNode user in usersList)
            {
                if (string.IsNullOrEmpty(user.Attributes["id"].InnerText)) continue;
                StreamWriter writer = new StreamWriter(@"F:\Data Science Project\users\" + user.Attributes["id"].InnerText + ".txt");
                writer.Write(user.OuterXml);
                writer.Close();

                userlistwriter.Write("<user id=\"" + user.Attributes["id"].InnerText + "\"><name>" + user.SelectSingleNode("name").InnerText + "</name></user>");
            }
            userlistwriter.Write("</users>");
            userlistwriter.Close();
        }
        public static string RemoveSpecialCharacters(string str)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\\')
                    sb.Append(' ');
                else if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'z' || (str[i] == '.' || str[i] == '_' || str[i] == ' ')))
                    sb.Append(str[i]);
                else if (str[i] == '\n')
                    sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}