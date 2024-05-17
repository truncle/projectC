using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;
using Unity.VisualScripting;

namespace Table
{
    public struct MiscData
    {
        public string name;
        public string para1;
        public string para2;
        public string para3;
        public string para4;
        public string para5;
    }

    public static class MiscTable
    {
        public static List<MiscData> datas = new();
        public static RawTable rawDatas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("misc.csv");
            rawDatas = rawTable;
            foreach (var row in rawTable.data)
            {
                MiscData data = new();
                data.name = rawTable.Get("misc", row);
                data.para1 = rawTable.Get("para1", row);
                data.para2 = rawTable.Get("para2", row);
                data.para3 = rawTable.Get("para3", row);
                data.para4 = rawTable.Get("para4", row);
                data.para5 = rawTable.Get("para5", row);
                datas.Add(data);
            }
        }

        public static MiscData Get(string name)
        {
            Init();
            return datas.Where(e => e.name == name).First();
        }

        public static List<List<int>> GetRandomExploreResource()
        {
            MiscData data = Get("explore_res_get");
            var resList = GameUtil.GetList3(data.para1);
            List<int> rateList = data.para2.Split("|").Select(int.Parse).ToList();
            return resList[GameUtil.GetRandomIndices(rateList, resList.Count).First()];
        }
    }
}
