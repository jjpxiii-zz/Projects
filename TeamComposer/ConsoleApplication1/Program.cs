using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    public class Program
    {
        private static List<Tuple<string, string>> _enemyList;

        public static void Main()
        {
            /*      int numberOfPlayers = 10;
                  string[] list1 = new[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j"};
                  _enemyList = new HashSet<Tuple<string, string>>
                  {
                    new Tuple<string, string>("a", "b"),
                    new Tuple<string, string>("a", "d"),
                    new Tuple<string, string>("f", "j")
                  };
                  IOrderedEnumerable<string> sortedList = list1.OrderBy(g => Guid.NewGuid());
                  List<string> team1 = sortedList.Take(numberOfPlayers/2).ToList();
                  List<string> team2 = sortedList.Skip(numberOfPlayers/2).ToList();
            */

            var numberOfPlayers = 6;
            var team1 = new List<string> { "a", "b", "e" };
            var team2 = new List<string> { "c", "d", "f" };

            // try to team up friends
            var teams = SeparateEnemies(team1, team2, new List<Tuple<string, string>> { new Tuple<string, string>("a", "b"), new Tuple<string, string>("a", "d"), new Tuple<string, string>("b", "f") });

            var teamsOk = JoinFriends(teams.Item1, teams.Item2, new List<Tuple<string, string>> { new Tuple<string, string>("e", "b"), new Tuple<string, string>("a", "f") });

        }

        public static Tuple<List<string>, List<string>, List<Tuple<string, string>>> JoinFriends(List<string> team1, List<string> team2, List<Tuple<string, string>> enemyList)
        {
            _enemyList = enemyList;
            var rulesDropped = new List<Tuple<string, string>>();
            // check for enemies
            for (int i = 0; i < _enemyList.Count; i++)
            {
                var ex = _enemyList[i];
                if (team1.Contains(ex.Item1) && team1.Contains(ex.Item2))
                {
                    var res = FindPermutation(ex, team1, team2);
                    if (team1 == res.Item1 && team2 == res.Item2 || team2 == res.Item1 && team1 == res.Item2)
                    {
                        rulesDropped.Add(_enemyList[i]);
                        _enemyList.Remove(ex);
                        i--;
                    }
                    else
                    {
                        team1 = res.Item1;
                        team2 = res.Item2;
                    }
                }
                if (team2.Contains(ex.Item1) && team2.Contains(ex.Item2))
                {
                    var res = FindPermutation(ex, team2, team1);
                    if (team1 == res.Item1 && team2 == res.Item2 || team2 == res.Item1 && team1 == res.Item2)
                    {
                        rulesDropped.Add(_enemyList[i]);
                        _enemyList.Remove(ex);
                        i--;
                    }
                    else
                    {
                        team1 = res.Item1;
                        team2 = res.Item2;
                    }
                }
            }
            return new Tuple<List<string>, List<string>, List<Tuple<string, string>>>(team1, team2, rulesDropped);
        }

        public static Tuple<List<string>, List<string>, List<Tuple<string, string>>> SeparateEnemies(List<string> team1, List<string> team2, List<Tuple<string, string>> enemyList)
        {
            _enemyList = enemyList;
            var rulesDropped = new List<Tuple<string, string>>();
            // check for enemies
            if (!AreThereEnemiesInTeam(team1) && !AreThereEnemiesInTeam(team2)) goto end;
            for (int i = 0; i < _enemyList.Count; i++)
            {
                var ex = _enemyList[i];
                if (team1.Contains(ex.Item1) && team1.Contains(ex.Item2))
                {
                    var res = FindPermutation(ex, team1, team2);
                    if (team1 == res.Item1 && team2 == res.Item2 || team2 == res.Item1 && team1 == res.Item2)
                    {
                        rulesDropped.Add(_enemyList[i]);
                        _enemyList.Remove(ex);
                        i--;
                    }
                    else
                    {
                        team1 = res.Item1;
                        team2 = res.Item2;
                    }
                }
                if (team2.Contains(ex.Item1) && team2.Contains(ex.Item2))
                {
                    var res = FindPermutation(ex, team2, team1);
                    if (team1 == res.Item1 && team2 == res.Item2 || team2 == res.Item1 && team1 == res.Item2)
                    {
                        rulesDropped.Add(_enemyList[i]);
                        _enemyList.Remove(ex);
                        i--;
                    }
                    else
                    {
                        team1 = res.Item1;
                        team2 = res.Item2;
                    }
                }
            }

            end:
            return new Tuple<List<string>, List<string>, List<Tuple<string, string>>>(team1, team2, rulesDropped);
        }

        private static Tuple<List<string>, List<string>> FindPermutation(Tuple<string, string> tuple, List<string> teamWithEnemies, List<string> secondTeam)
        {
            for (int i = 0; i <= 1; i++)
            {
                var enemy1 = teamWithEnemies.Find(t => t == tuple.Item1);
                var tempList = teamWithEnemies.Where(t => t != tuple.Item1).ToList();
                foreach (var p in secondTeam)
                {
                    var tempList2 = teamWithEnemies.Where(t => t != tuple.Item1).ToList(); ;
                    tempList2.Add(p);
                    if (!AreThereEnemiesInTeam(tempList2))
                    {
                        var tempSecond = secondTeam.Where(t => t != p).ToList();
                        tempSecond.Add(enemy1);
                        if (!AreThereEnemiesInTeam(tempSecond))
                        {
                            teamWithEnemies = tempList2;
                            secondTeam = tempSecond;
                            break;
                        }
                    }
                }
                if (tempList.Count != 3)
                {
                    tuple = new Tuple<string, string>(tuple.Item2, tuple.Item1);
                    continue;
                }
                return new Tuple<List<string>, List<string>>(teamWithEnemies, secondTeam);
            }
            return new Tuple<List<string>, List<string>>(teamWithEnemies, secondTeam);
        }

        public static bool AreThereEnemiesInTeam(List<string> team)
        {
            return _enemyList.Any(t => team.Contains(t.Item1) && team.Contains(t.Item2));
        }
    }
}