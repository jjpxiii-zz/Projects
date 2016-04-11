using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Tests
{
    [TestClass()]
    public class SwissFormatInfoTests
    {
        [TestMethod()]
        public void FormatTest()
        {
            Assert.AreEqual(string.Format(new PriceFormatter(), "{0:SwissDefault}", 133.00), "133.-");
            Assert.AreEqual(string.Format(new PriceFormatter(), "{0:SwissDefault}", 133.66), "133.66");
            Assert.AreEqual(string.Format(new PriceFormatter(), "{0:SwissDefault}", 133.6959), "133.70");
            Assert.AreEqual(string.Format(new PriceFormatter(), "{0:SwissWithCurrency}", 133.00), "CHF 133.-");
            Assert.AreEqual(string.Format(new PriceFormatter(), "{0:SwissWithCurrency}", 133.66), "CHF 133.66");
            Assert.AreEqual(string.Format(new PriceFormatter(), "{0:Default}", 133.00), "133,00");
        }

        [TestMethod]
        public void PriceFormatTest()
        {
            Assert.AreEqual("199.<sup>00</sup>", Programc.PriceFormat("199.00", "<sup>", "</sup>", "", "", "", "CHF", ".", false, "CH", true, false));
            Assert.AreEqual("199<sup>&euro;00</sup>", Programc.PriceFormat("199,00&euro;", "<sup>", "</sup>", "", "", "&euro;", "€", ",", false, "FR", false));
            Assert.AreEqual("199,00&euro;<em class=\"tax\">HT</em>", Programc.PriceFormat("199,00&euro;HT", "", "", "<em class=\"tax\">", "</em>", "&euro;", "€", ",", true, "FR", false));
            Assert.AreEqual("199<sup>&euro;00</sup><em class=\"tax\">HT</em>", Programc.PriceFormat("199,00&euro;HT", "<sup>", "</sup>", "<em class=\"tax\">", "</em>", "&euro;", "€", ",", true, "FR", false));
        }

        [TestMethod]
        public void DisplayCostTest()
        {

            Assert.AreEqual("199.-", Programc.DisplayCost(199.00m));
            Assert.AreEqual("199.55", Programc.DisplayCost(199.55m));
        }

        [TestMethod]
        public void TestTB()
        {
            var team1 = new List<string> { "a", "b", "c", "d", "e" };
            var team2 = new List<string> { "g", "h", "f", "i", "j" };
            var enemyList = new List<Tuple<string, string>> { new Tuple<string, string>("a", "b"), new Tuple<string, string>("f", "d"), new Tuple<string, string>("b", "f"), new Tuple<string, string>("e", "g"), new Tuple<string, string>("i", "j"), new Tuple<string, string>("b", "h") };

            var teamsOk = Program.SeparateEnemies(team1, team2, enemyList.Select(o => o).ToList());

            foreach (var i in teamsOk.Item3)
            {
                Assert.IsTrue(teamsOk.Item1.Contains(i.Item1) && teamsOk.Item1.Contains(i.Item2) || teamsOk.Item2.Contains(i.Item1) && teamsOk.Item2.Contains(i.Item2));
                enemyList.Remove(i);
            }

            foreach (var item in enemyList)
            {
                Assert.IsFalse(teamsOk.Item1.Contains(item.Item1) && teamsOk.Item1.Contains(item.Item2));
                Assert.IsFalse(teamsOk.Item2.Contains(item.Item1) && teamsOk.Item2.Contains(item.Item2));
            }
        }
    }
}