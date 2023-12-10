using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Util
{
    public static class GameUtil
    {
        static string tableFolderPath = Path.Combine(Application.streamingAssetsPath, "CsvTable");

        public static void Print()
        {
            Debug.Log("Hello");
        }

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
            List<string> fields = new(reader.ReadLine().Split(","));
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                List<string> lineData = new(line.Split(","));
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
    }

}
