using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScienceAssignmentItemItem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Import data
            string userItemData = "../../../userItem.data";
            string movieLensData = "../../../u.data";
            FileStream fileStream = new FileStream(userItemData, FileMode.Open);
            CsvReader csvReader = new CsvReader();
            List<string> csvContent = csvReader.retrieveCsvContent(fileStream);
            fileStream.Close();

            // Parse data
            Dictionary<string, Dictionary<string, decimal>> itemRatings = retrieveItemRatingsItemData(csvContent);
            Dictionary<string, Dictionary<string, decimal>> userRatings = retrieveUserRatingsItemData(csvContent);


            RecommendSystem recommendSystem = new RecommendSystem(itemRatings, userRatings);
            recommendSystem.computeDeviations();

            Console.WriteLine("Predicted ratings of user 7");
            Console.WriteLine("Predicted rating for item: 101 with rating of: " + recommendSystem.predictRating("7", "101"));
            Console.WriteLine("Predicted rating for item: 103 with rating of: " + recommendSystem.predictRating("7", "103"));
            Console.WriteLine("Predicted rating for item: 106 with rating of: " + recommendSystem.predictRating("7", "106"));
            Console.WriteLine("Predicted ratings of user 3");
            Console.WriteLine("Predicted rating for item: 103 with rating of: " + recommendSystem.predictRating("3", "103"));
            Console.WriteLine("Predicted rating for item: 105 with rating of: " + recommendSystem.predictRating("3", "105"));

            recommendSystem.updateRating("3", "105", 4);
            Console.WriteLine("Predicted ratings of user 7 after rating update");
            Console.WriteLine("Predicted rating for item: 101 with rating of: " + recommendSystem.predictRating("7", "101"));
            Console.WriteLine("Predicted rating for item: 103 with rating of: " + recommendSystem.predictRating("7", "103"));
            Console.WriteLine("Predicted rating for item: 106 with rating of: " + recommendSystem.predictRating("7", "106"));

            fileStream = new FileStream(movieLensData, FileMode.Open);
            csvReader = new CsvReader();
            csvContent = csvReader.retrieveCsvContent(fileStream);
            fileStream.Close();

            // Parse data
            itemRatings = retrieveItemRatingsMovieLensData(csvContent);
            userRatings = retrieveUserRatingsMovieLensData(csvContent);


            recommendSystem = new RecommendSystem(itemRatings, userRatings);
            recommendSystem.computeDeviations();

            Dictionary<string, decimal> topRatedItems = recommendSystem.retrieveTopRecommendations("186", 5);
            foreach (KeyValuePair<string, decimal> item in topRatedItems)
            {
                Console.WriteLine("Recommended item: " + item.Key + " with predicted rating of: " + item.Value);
            }
        }

        private static Dictionary<string, Dictionary<string, decimal>> retrieveItemRatingsItemData(List<string> csvContent)
        {
            Dictionary<string, Dictionary<string, decimal>> itemRatings =
                new Dictionary<string, Dictionary<string, decimal>>();

            for (int i = 0; i < csvContent.Count; i++)
            {
                if(!itemRatings.ContainsKey(csvContent[i + 1]))
                    itemRatings.Add(csvContent[i + 1], new Dictionary<string, decimal>());
                itemRatings[csvContent[i + 1]].Add(csvContent[i], decimal.Parse(csvContent[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                i += 2;
            }
            return itemRatings;
        }

        private static Dictionary<string, Dictionary<string, decimal>> retrieveItemRatingsMovieLensData(List<string> csvContent)
        {
            Dictionary<string, Dictionary<string, decimal>> itemRatings =
                new Dictionary<string, Dictionary<string, decimal>>();

            for (int i = 0; i < csvContent.Count; i++)
            {
                if (!itemRatings.ContainsKey(csvContent[i + 1]))
                    itemRatings.Add(csvContent[i + 1], new Dictionary<string, decimal>());
                itemRatings[csvContent[i + 1]].Add(csvContent[i], decimal.Parse(csvContent[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                i += 3;  
            }
            return itemRatings;
        }

        private static Dictionary<string, Dictionary<string, decimal>> retrieveUserRatingsItemData(List<string> csvContent)
        {
            Dictionary<string, Dictionary<string, decimal>> userRatings =
                new Dictionary<string, Dictionary<string, decimal>>();

            for (int i = 0; i < csvContent.Count; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        if (!userRatings.ContainsKey(csvContent[i]))
                            userRatings.Add(csvContent[i], new Dictionary<string, decimal>());
                        break;
                    case 1:
                        if (!userRatings[csvContent[i - 1]].ContainsKey(csvContent[i]))
                            userRatings[csvContent[i - 1]].Add(csvContent[i], 0);
                        break;
                    case 2:
                        userRatings[csvContent[i - 2]][csvContent[i - 1]] = decimal.Parse(csvContent[i], CultureInfo.InvariantCulture.NumberFormat);
                        break;
                }
            }
            return userRatings;
        }

        private static Dictionary<string, Dictionary<string, decimal>> retrieveUserRatingsMovieLensData(List<string> csvContent)
        {
            Dictionary<string, Dictionary<string, decimal>> userRatings =
                new Dictionary<string, Dictionary<string, decimal>>();

            for (int i = 0; i < csvContent.Count; i++)
            {
                switch (i % 4)
                {
                    case 0:
                        if (!userRatings.ContainsKey(csvContent[i]))
                            userRatings.Add(csvContent[i], new Dictionary<string, decimal>());
                        break;
                    case 1:
                        if (!userRatings[csvContent[i - 1]].ContainsKey(csvContent[i]))
                            userRatings[csvContent[i - 1]].Add(csvContent[i], 0);
                        break;
                    case 2:
                        userRatings[csvContent[i - 2]][csvContent[i - 1]] = decimal.Parse(csvContent[i], CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case 3:
                        break;
                }
            }
            return userRatings;
        }
    }
}
