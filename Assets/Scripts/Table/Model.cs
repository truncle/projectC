using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<T> result = new();
            for (int i = 0; i < tmp.Length; i++)
            {
                result.Add((T)Convert.ChangeType(tmp[i], typeof(T)));
            }
            return result;
        }

        public List<List<T>> GetList2<T>(string field, List<string> row, string sep = "|", string sep2 = ":")
        {
            List<List<T>> result = new();
            int index = fields.IndexOf(field);
            string[] list1 = row[index].Split(sep);
            for (int i = 0; i < list1.Length; i++)
            {
                string[] list2 = row[index].Split(sep2);
                List<T> listT = new();
                for (int j = 0; j < list2.Length; i++)
                {
                    listT.Add((T)Convert.ChangeType(list2[j], typeof(T)));
                }
                result.Add(listT);
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
