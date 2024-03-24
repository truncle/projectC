using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Table
{
    public struct RawTable
    {
        public List<string> fields;
        public List<List<string>> data;

        public string Get(string field, List<string> row)
        {
            int index = fields.IndexOf(field);
            return row[index];
        }

        public List<T> GetList<T>(string field, List<string> row, string sep = "|")
        {
            int index = fields.IndexOf(field);
            string[] tmp = row[index].Split(sep);
            if (string.IsNullOrEmpty(row[index]))
                return new();
            List<T> result = new();
            for (int i = 0; i < tmp.Length; i++)
            {
                if (string.IsNullOrEmpty(tmp[i]))
                    continue;
                result.Add((T)Convert.ChangeType(tmp[i], typeof(T)));
            }
            return result;
        }

        public List<List<T>> GetList2<T>(string field, List<string> row, string sep = "|", string sep2 = ":")
        {
            List<List<T>> result = new();
            int index = fields.IndexOf(field);
            if (string.IsNullOrEmpty(row[index]))
                return new();
            string[] list1 = row[index].Split(sep);
            for (int i = 0; i < list1.Length; i++)
            {
                if (string.IsNullOrEmpty(list1[i]))
                    continue;
                string[] list2 = list1[i].Split(sep2);
                List<T> listT = new();
                for (int j = 0; j < list2.Length; j++)
                {
                    if (string.IsNullOrEmpty(list2[j]))
                        continue;
                    listT.Add((T)Convert.ChangeType(list2[j], typeof(T)));
                }
                result.Add(listT);
            }
            return result;
        }

        public List<List<List<T>>> GetList3<T>(string field, List<string> row, string sep = "|", string sep2 = "&", string sep3 = ":")
        {
            List<List<List<T>>> result = new();
            int index = fields.IndexOf(field);
            if (string.IsNullOrEmpty(row[index]))
                return new();
            string[] list1 = row[index].Split(sep);
            for (int i = 0; i < list1.Length; i++)
            {
                List<List<T>> result2 = new();
                if (string.IsNullOrEmpty(list1[i]))
                    continue;
                string[] list2 = list1[i].Split(sep2);
                for (int j = 0; j < list2.Length; j++)
                {
                    if (string.IsNullOrEmpty(list2[j]))
                        continue;
                    string[] list3 = list2[j].Split(sep3);
                    List<T> listT = new();
                    for (int k = 0; k < list3.Length; k++)
                    {
                        if (string.IsNullOrEmpty(list3[k]))
                            continue;
                        listT.Add((T)Convert.ChangeType(list3[k], typeof(T)));
                    }
                    result2.Add(listT);
                }
                result.Add(result2);
            }
            return result;
        }
    }

    public struct Condition
    {
        public string name;
        public string func;
        public List<string> param;
    }

    public struct DayRecord
    {
        public int day;
        public int storylineId;
        public int exploreId;
        public List<CharacterStatus> characters;
        public Dictionary<int, int> items;
    }
}
