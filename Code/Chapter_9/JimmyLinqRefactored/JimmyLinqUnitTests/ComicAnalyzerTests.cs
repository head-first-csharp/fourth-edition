using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimmyLinqUnitTests
{
    using JimmyLinq;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ComicAnalyzerTests
    {
        IEnumerable<Comic> testComics = new[]
        {
                new Comic() { Issue = 1, Name = "Issue 1"},
                new Comic() { Issue = 2, Name = "Issue 2"},
                new Comic() { Issue = 3, Name = "Issue 3"},
        };

        [TestMethod]
        public void ComicAnalyzer_Should_Group_Comics()
        {
            var prices = new Dictionary<int, decimal>()
         {
            { 1, 20M },
            { 2, 10M },
            { 3, 1000M },
         };

            var groups = ComicAnalyzer.GroupComicsByPrice(testComics, prices);

            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual(PriceRange.Cheap, groups.First().Key);
            Assert.AreEqual(2, groups.First().First().Issue);
            Assert.AreEqual("Issue 2", groups.First().First().Name);
        }

        [TestMethod]
        public void ComicAnalyzer_Should_Generate_A_List_Of_Reviews()
        {
            var testReviews = new[]
            {
                new Review() { Issue = 1, Critic = Critics.MuddyCritic, Score = 14.5},
                new Review() { Issue = 1, Critic = Critics.RottenTornadoes, Score = 59.93},
                new Review() { Issue = 2, Critic = Critics.MuddyCritic, Score = 40.3},
                new Review() { Issue = 2, Critic = Critics.RottenTornadoes, Score = 95.11},
            };

            var expectedResults = new[]
            {
                "MuddyCritic rated #1 'Issue 1' 14.50",
                "RottenTornadoes rated #1 'Issue 1' 59.93",
                "MuddyCritic rated #2 'Issue 2' 40.30",
                "RottenTornadoes rated #2 'Issue 2' 95.11",
            };

            var actualResults = ComicAnalyzer.GetReviews(testComics, testReviews).ToList();
            CollectionAssert.AreEqual(expectedResults, actualResults);
        }

        [TestMethod]
        public void ComicAnalyzer_Should_Handle_Weird_Review_Scores()
        {
            var testReviews = new[]
            {
                new Review() { Issue = 1, Critic = Critics.MuddyCritic, Score = -12.1212},
                new Review() { Issue = 1, Critic = Critics.RottenTornadoes, Score = 391691234.48931},
                new Review() { Issue = 2, Critic = Critics.RottenTornadoes, Score = 0},
                new Review() { Issue = 2, Critic = Critics.MuddyCritic, Score = 40.3},
                new Review() { Issue = 2, Critic = Critics.MuddyCritic, Score = 40.3},
                new Review() { Issue = 2, Critic = Critics.MuddyCritic, Score = 40.3},
                new Review() { Issue = 2, Critic = Critics.MuddyCritic, Score = 40.3},
              };



            var expectedResults = new[]
            {
                "MuddyCritic rated #1 'Issue 1' -12.12",
                "RottenTornadoes rated #1 'Issue 1' 391691234.49",
                "RottenTornadoes rated #2 'Issue 2' 0.00",
                "MuddyCritic rated #2 'Issue 2' 40.30",
                "MuddyCritic rated #2 'Issue 2' 40.30",
                "MuddyCritic rated #2 'Issue 2' 40.30",
                "MuddyCritic rated #2 'Issue 2' 40.30",
              };

            var actualResults = ComicAnalyzer.GetReviews(testComics, testReviews).ToList();
            CollectionAssert.AreEqual(expectedResults, actualResults);
        }
    }
}
