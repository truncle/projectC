using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;

namespace Table
{
    public struct ItemboxData
    {
        public int id;
        public int type;
        public List<int> contentList;
        public List<List<int>> dropList;

        public List<int> GetItems()
        {
            if (type == 1)
            {
                List<int> result = new();
                int rand = UnityEngine.Random.Range(1, 10000);
                foreach (var dropItem in dropList)
                {
                    if (dropItem[1] >= rand)
                    {
                        result.Add(dropItem[0]);
                        return result;
                    }
                    else
                    {
                        rand -= dropItem[1];
                    }
                }
            }
            else if (type == 2)
            {
                return contentList;
            }
            return new();
        }
    }

    public static class ItemboxTable
    {
        public static List<ItemboxData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("itemBox.csv");
            foreach (var row in rawTable.data)
            {
                ItemboxData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.type = Convert.ToInt32(rawTable.Get("type", row));
                if (data.type == 1)
                {
                    data.dropList = rawTable.GetList2<int>("content", row, "|", ";");
                }
                else if (data.type == 2)
                {
                    data.contentList = rawTable.GetList<int>("content", row);
                }
                datas.Add(data);
            }
        }

        public static ItemboxData Get(int id)
        {
            Init();
            return datas.Where(e => e.id == id).First();
        }
    }
}
