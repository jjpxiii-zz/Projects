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
    public class TeamComposerTests
    {
        [TestMethod]
        public void TestSeparateEnemies()
        {
            var team1 = new List<string> { "a", "b", "c", "d", "e" };
            var team2 = new List<string> { "g", "h", "f", "i", "j" };
            var enemyList = new List<Tuple<string, string>> { new Tuple<string, string>("a", "b"), new Tuple<string, string>("f", "d"), new Tuple<string, string>("b", "f"), new Tuple<string, string>("e", "g"), new Tuple<string, string>("i", "j"), new Tuple<string, string>("b", "h") };

            var teamsOk = Program.SeparateEnemies(team1, team2, enemyList.Select(o => o).ToList());

            //Assert.IsTrue(teamsOk.Item3.Any());
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

        [TestMethod]
        public void TestJoinFriends()
        {
            var team1 = new List<string> { "a", "b", "c", "d", "e" };
            var team2 = new List<string> { "g", "h", "f", "i", "j" };
            var enemyList = new List<Tuple<string, string>> { new Tuple<string, string>("a", "b"), new Tuple<string, string>("f", "d"), new Tuple<string, string>("b", "h") };

            var teamsOk = Program.JoinFriends(team1, team2, enemyList.Select(o => o).ToList());

            //Assert.IsTrue(teamsOk.Item3.Any());
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