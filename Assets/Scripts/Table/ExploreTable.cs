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
        public int itemBoxId; //获得道具
        public int returnDays;
        public List<int> failed;
        public List<int> endTextContent;
        public List<int> branchItem;
        public List<List<int>> itemSets;
        public List<List<Condition>> include;
        public List<List<Condition>> exclude;
        public List<List<CharacterStatus>> statusChange;
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
                data.returnDays = Convert.ToInt32(rawTable.Get("returnDays", row));
                data.failed = rawTable.GetList<int>("failed", row, "|");
                data.itemBoxId = Convert.ToInt32(rawTable.Get("getItem", row));
                data.branchItem = rawTable.GetList<int>("branchItem", row, "|");
                data.endTextContent = rawTable.GetList<int>("endTextContent", row, "|");
                data.itemSets = rawTable.GetList2<int>("itemSet", row);
                data.include = GameUtil.GetConditionSet(rawTable.Get("include", row));
                data.exclude = GameUtil.GetConditionSet(rawTable.Get("exclude", row));
                data.statusChange = new();
                List<List<List<int>>> statusChangeRaw = rawTable.GetList3<int>("statusChange", row, "|", "&", ":");
                statusChangeRaw = new();
                foreach (var endStatusChangeRaw in statusChangeRaw)
                {
                    List<CharacterStatus> endStatusChange = new();
                    foreach (var changeInfo in endStatusChangeRaw)
                    {
                        int characterId = changeInfo[0];
                        StatusType type = (StatusType)changeInfo[1];
                        int changeNum = changeInfo[3];
                        endStatusChange.Add(new()
                        {
                            characterId = characterId,
                            statusValues = new()
                            {
                                [type] = changeNum
                            },
                        });
                    }
                    data.statusChange.Add(endStatusChange);
                }
            }
        }
    }
}
