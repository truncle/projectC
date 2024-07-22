using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;

namespace Table
{
    public struct TextboxData
    {
        public int id;
        public int type;
        public int textId;
        public List<List<int>> dropList;

        public int GetText()
        {
            if (type == 1)
            {
                int rand = UnityEngine.Random.Range(1, 10000);
                foreach (var dropItem in dropList)
                {
                    if (dropItem[1] >= rand)
                    {
                        return dropItem[0];
                    }
                    else
                    {
                        rand -= dropItem[1];
                    }
                }
            }
            else if (type == 2)
            {
                return textId;
            }
            return new();
        }
    }

    public static class TextboxTable
    {
        public static List<TextboxData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("textBox.csv");
            foreach (var row in rawTable.data)
            {
                TextboxData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.type = Convert.ToInt32(rawTable.Get("type", row));
                if (data.type == 1)
                {
                    data.dropList = rawTable.GetList2<int>("content", row, "|");
                }
                else if (data.type == 2)
                {
                    data.textId = Convert.ToInt32(rawTable.Get("content", row));
                }
                datas.Add(data);
            }
        }

        public static TextboxData Get(int id)
        {
            Init();
            return datas.Where(e => e.id == id).First();
        }
    }
}
