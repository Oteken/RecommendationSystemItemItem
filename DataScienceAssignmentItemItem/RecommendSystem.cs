using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScienceAssignmentItemItem
{
    class RecommendSystem
    {
        public Dictionary<string, Dictionary<string, decimal>> userRatings { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> itemRatings { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> itemDeviations { get; set; }
        public Dictionary<string, Dictionary<string, int>> itemDeviationsCount{ get; set; }
        private decimal UNKNOWNPREDICTIONVALUE = -100;

        public RecommendSystem(Dictionary<string, Dictionary<string, decimal>> itemRatings, Dictionary<string, Dictionary<string, decimal>> userRatings)
        {
            this.itemRatings = itemRatings;
            this.userRatings = userRatings;
            itemDeviations = new Dictionary<string, Dictionary<string, decimal>>();
            itemDeviationsCount = new Dictionary<string, Dictionary<string, int>>();
        }

        public void computeDeviations()
        {
            resetItemDeviations();
            foreach (KeyValuePair<string, Dictionary<string, decimal>> item in itemRatings)
            {
                updateDeviation(item.Key);
            }
        }

        private decimal computeDeviation(List<decimal> ratingsA, List<decimal> ratingsB)
        {
            decimal difference = 0;
            for (int i = 0; i < ratingsA.Count; i++)
            {
                difference += ratingsA[i] - ratingsB[i];
            }
            return difference / ratingsA.Count;
        }

        private void updateDeviation(string item)
        {
            List<decimal> sameRatingsItem;
            List<decimal> sameRatingsOtherItem;
            foreach (KeyValuePair<string, Dictionary<string, decimal>> otherItem in itemRatings)
            {
                sameRatingsItem = new List<decimal>();
                sameRatingsOtherItem = new List<decimal>();
                if (item != otherItem.Key)
                {
                    foreach (KeyValuePair<string, decimal> userRating in otherItem.Value)
                    {
                        if (itemRatings[item].ContainsKey(userRating.Key))
                        {
                            sameRatingsItem.Add(itemRatings[item][userRating.Key]);
                            sameRatingsOtherItem.Add(userRating.Value);
                        }
                    }
                }
                if (sameRatingsItem.Count > 0)
                {
                    decimal deviation = computeDeviation(sameRatingsItem, sameRatingsOtherItem);

                    itemDeviations[item][otherItem.Key] = deviation;
                    itemDeviations[otherItem.Key][item] = -deviation;
                    itemDeviationsCount[item][otherItem.Key] = sameRatingsItem.Count;
                    itemDeviationsCount[otherItem.Key][item] = sameRatingsItem.Count;
                }
            }
        }

        private void resetItemDeviations()
        {
            foreach (KeyValuePair<string, Dictionary<string, decimal>> itemRating in itemRatings)
            {
                if (!itemDeviations.ContainsKey(itemRating.Key))
                    itemDeviations.Add(itemRating.Key, new Dictionary<string, decimal>());
                if (!itemDeviationsCount.ContainsKey(itemRating.Key))
                    itemDeviationsCount.Add(itemRating.Key, new Dictionary<string, int>());
            }
        }

        public decimal predictRating(string user, string item)
        {
            decimal numerator = 0;
            int denominator = 0;
            foreach (KeyValuePair<string, decimal> userRating in userRatings[user])
            {
                if (itemDeviations[item].ContainsKey(userRating.Key))
                {
                    decimal deviation = itemDeviations[item][userRating.Key];
                    int ratingsCount = itemDeviationsCount[item][userRating.Key];
                    numerator += (userRating.Value + deviation) * ratingsCount;
                    denominator += ratingsCount;
                }
            }
            if (denominator == 0)
                return UNKNOWNPREDICTIONVALUE;
            return numerator / denominator;
        }

        public void updateRating(string user, string item, decimal rating)
        {
            if (userRatings[user].ContainsKey(item))
                userRatings[user][item] = rating;
            else
                userRatings[user].Add(item, rating);
            if (itemRatings[item].ContainsKey(user))
                itemRatings[item][user] = rating;
            else
                itemRatings[item].Add(user, rating);
            updateDeviation(item);
        }

        public Dictionary<string, decimal> retrieveTopRecommendations(string user, int amount)
        {
            List<string> unratedItemsByUser = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, decimal>> itemRating in itemRatings)
            {
                if (!userRatings[user].ContainsKey(itemRating.Key))
                    unratedItemsByUser.Add(itemRating.Key);
            }
            Dictionary<string, decimal> predictedItemsForUser = new Dictionary<string, decimal>();
            for (int i = 0; i < unratedItemsByUser.Count; i++)
            {
                decimal predictedValue = predictRating(user, unratedItemsByUser[i]);
                if (predictedValue != UNKNOWNPREDICTIONVALUE)
                    predictedItemsForUser.Add(unratedItemsByUser[i], predictedValue);
            }
            return retrieveTopItems(predictedItemsForUser, amount);
        }

        private Dictionary<string, decimal> retrieveTopItems(Dictionary<string, decimal> items, int amount)
        {
            Dictionary<string, decimal> topItems = new Dictionary<string, decimal>();
            for (int i = 0; i < amount; i++)
            {
                KeyValuePair<string, decimal> highestRatedItem = retrieveHighestFromDictionary(items);
                topItems.Add(highestRatedItem.Key, highestRatedItem.Value);
                items.Remove(highestRatedItem.Key);
            }
            return topItems;
        }

        private KeyValuePair<string, decimal> retrieveHighestFromDictionary(Dictionary<string, decimal> dictionary)
        {
            KeyValuePair<string, decimal> highestRated = dictionary.First();
            foreach(KeyValuePair<string, decimal> entry in dictionary)
            {
                if (entry.Value > highestRated.Value)
                    highestRated = entry;
            }
            return highestRated;
        }
    }
}
