using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;

namespace Table
{
    public struct TextData
    {
        public int id;
        public Dictionary<string, string> content;

        public string GetText(string language)
        {
            if (content.ContainsKey(language))
                return content[language];
            return content["cnContent"];
        }
    }

    public static class TextTable
    {
        public static List<TextData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("text.csv");
            foreach (var row in rawTable.data)
            {
                TextData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.content = new();
                data.content["cn"] = rawTable.Get("cnContent", row);
                datas.Add(data);
            }
        }

        public static TextData Get(int id)
        {
            Init();
            return datas.Where(e => e.id == id).First();
        }

        public static string GetText(int id, string language = "cn")
        {
            return Get(id).GetText(language);
        }
    }
}
