using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;

namespace Table
{
    public struct ItemData
    {
        public int id;
        public int name;
        public string path;
    }

    public static class ItemTable
    {
        public static List<ItemData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("item.csv");
            foreach (var row in rawTable.data)
            {
                ItemData data = new();
                data.id = Convert.ToInt32(rawTable.Get("Id", row));
                data.name = Convert.ToInt32(rawTable.Get("name", row));
                data.path = rawTable.Get("path", row);
                datas.Add(data);
            }
        }

        public static ItemData Get(int id)
        {
            Init();
            return datas.Where(e => e.id == id).First();
        }
    }
}
