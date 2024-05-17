using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Util
{
    public static class GameUtil
    {
        static string tableFolderPath = Path.Combine(Application.streamingAssetsPath, "CsvTable");

        public static List<List<Table.Condition>> GetConditionSet(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new();
            List<List<Table.Condition>> result = new();
            string[] orArr = str.Split("|");
            for (int i = 0; i < orArr.Length; i++)
            {
                if (string.IsNullOrEmpty(orArr[i]))
                {
                    Debug.Log("condition error");
                    continue;
                }
                List<Table.Condition> andList = new();
                string[] andArr = orArr[i].Split("&");
                for (int j = 0; j < andArr.Length; j++)
                {
                    if (string.IsNullOrEmpty(andArr[j]))
                    {
                        Debug.Log("condition error");
                        continue;
                    }
                    andList.Add(GetCondition(andArr[j]));
                }
                result.Add(andList);
            }

            return result;
        }

        public static Table.Condition GetCondition(string str)
        {
            string pattern = @"(?<name>[A-Z]*?):(?<func>[A-Z]*?)\[(?<param>[\s\S]*?)\]";
            Match match = new Regex(pattern).Match(str);
            if (match.Success)
            {
                string name = match.Groups["name"].Value;
                string func = match.Groups["func"].Value;
                List<string> param = new(match.Groups["param"].Value.Split(":"));
                return new Table.Condition()
                {
                    name = name,
                    func = func,
                    param = param,
                };
            }
            else
            {
                Debug.Log("Match failed: " + str);
                return new();
            }
        }

        public static Table.RawTable ReadCsvTable(string filePath)
        {
            string path = Path.Combine(tableFolderPath, filePath);
            using StreamReader reader = new(path, Encoding.UTF8);
            List<List<string>> data = new();
            List<string> fields = new(reader.ReadLine().Split(";"));
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                List<string> lineData = new(line.Split(";"));
                data.Add(lineData);
            }
            return new Table.RawTable
            {
                fields = fields,
                data = data,
            };
        }

        public static bool ListContains<T>(List<T> list1, List<T> list2)
        {
            return list2.All(item => list1.Contains(item));
        }

        public static List<int> GetRandomIndices(List<int> rateList, int numIndices)
        {
            HashSet<int> randomIndices = new HashSet<int>();

            while (randomIndices.Count < numIndices)
            {
                int randomIndex = Random.Range(0, rateList.Count);
                randomIndices.Add(randomIndex);
            }

            return new List<int>(randomIndices);
        }

        public static List<List<int>> GetList2(string input, string sep = "|", string sep2 = ":")
        {
            List<List<int>> groups = new List<List<int>>();
            string[] subgroups = input.Split(sep);
            foreach (string subgroupStr in subgroups)
            {
                List<int> subgroup = subgroupStr.Split(sep2).Select(int.Parse).ToList();
                groups.Add(subgroup);
            }
            return groups;
        }

        public static List<List<List<int>>> GetList3(string input, string sep = "|", string sep2 = "&", string sep3 = ":")
        {
            List<List<List<int>>> result = new();
            string[] groups = input.Split(sep);
            foreach (string group in groups)
            {
                List<List<int>> subgroup = new List<List<int>>();
                string[] subgroups = group.Split(sep2);
                foreach (string subgroupStr in subgroups)
                {
                    List<int> sublist = subgroupStr.Split(sep3).Select(int.Parse).ToList();
                    subgroup.Add(sublist);
                }
                result.Add(subgroup);
            }
            return result;
        }

        //public static List<List<int>> List2()
        //{

        //}

        //public static List<List<List<int>>> List3()
        //{

        //}
    }

}
