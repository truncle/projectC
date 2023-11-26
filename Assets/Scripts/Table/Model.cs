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

        public List<T> GetList<T>(string field, List<string> row, string sep)
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
    }
}
