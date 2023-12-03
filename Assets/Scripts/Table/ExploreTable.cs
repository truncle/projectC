using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Table
{
    public struct ExploreData
    {
        public int id;
        public int textContent;
        public int exploreNum;
        public int groupId;
        public int getItem; //获得道具
        public List<int> endTextContent;
        public List<int> hungerChange;
        public List<int> thirstyChange;
        public List<int> mindChange;
        public List<List<int>> itemSets;
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
            RawTable rawTable = GameUtil.ReadCsvTable("EventExplory.csv");
            foreach (var row in rawTable.data)
            {
                ExploreData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.textContent = Convert.ToInt32(rawTable.Get("textContent", row));
                data.exploreNum = Convert.ToInt32(rawTable.Get("exploreNum", row));
                data.groupId = Convert.ToInt32(rawTable.Get("groupId", row));
                data.getItem = Convert.ToInt32(rawTable.Get("getItem", row));
                data.endTextContent = rawTable.GetList<int>("endTextContent", row, "|");
                data.hungerChange = rawTable.GetList<int>("hungerChange", row);
                data.thirstyChange = rawTable.GetList<int>("thirstyChange", row);
                data.mindChange = rawTable.GetList<int>("mindChange", row);
                data.itemSets = rawTable.GetList2<int>("itemSet", row);
                data.include = GameUtil.GetConditionSet(rawTable.Get("include", row));
                data.exclude = GameUtil.GetConditionSet(rawTable.Get("exclude", row));
            }
        }
    }
}
