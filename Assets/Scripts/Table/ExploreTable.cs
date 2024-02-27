using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;

namespace Table
{
    public struct ExploreData
    {
        public int id;
        public int textContent;
        public int exploreNum;
        public int groupId;
        public int returnDays;
        public List<int> endTextContent;
        public List<int> failRate; //对应角色的探索失败率
        public List<int> provideItem; //可以提供的道具
        public List<int> getItem;
        public List<List<List<int>>> getRes;
        public List<List<int>> statusChange;
        public List<List<Condition>> include;
        public List<List<Condition>> exclude;
    }

    public static class ExploreTable
    {
        public static List<ExploreData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("eventExplory.csv");
            foreach (var row in rawTable.data)
            {
                ExploreData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.textContent = Convert.ToInt32(rawTable.Get("textContent", row));
                data.exploreNum = Convert.ToInt32(rawTable.Get("exploreNum", row));
                data.groupId = Convert.ToInt32(rawTable.Get("groupId", row));
                data.returnDays = Convert.ToInt32(rawTable.Get("returnDays", row));
                data.endTextContent = rawTable.GetList<int>("endTextContent", row, "|");
                data.failRate = rawTable.GetList<int>("failRate", row, "|");
                data.provideItem = rawTable.GetList<int>("provideItem", row, "|");
                data.getItem = rawTable.GetList<int>("getItem", row, "|");
                data.getRes = rawTable.GetList3<int>("getRes", row, "|");
                data.include = GameUtil.GetConditionSet(rawTable.Get("include", row));
                data.exclude = GameUtil.GetConditionSet(rawTable.Get("exclude", row));
                data.statusChange = rawTable.GetList2<int>("statusChange", row, "|");
                datas.Add(data);
            }
        }
    }
}
