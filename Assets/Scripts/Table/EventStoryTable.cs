using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util;

namespace Table
{
    public struct EventStoryData
    {
        public int id;
        public int textContent;
        public int eventType;
        public List<int> branchItem;
        public List<int> endTextContent;
        public List<int> days;
        public List<List<int>> itemSets;
        public List<List<Condition>> include;
        public List<List<Condition>> exclude;
        public List<List<Condition>> priorityBranch;
        public List<List<CharacterStatus>> statusChange;
        public List<List<int>> resourceChange; // todo
    }

    public static class EventStoryTable
    {
        public static List<EventStoryData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("eventStory.csv");
            foreach (var row in rawTable.data)
            {
                EventStoryData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.textContent = Convert.ToInt32(rawTable.Get("textContent", row));
                data.eventType = Convert.ToInt32(rawTable.Get("eventType", row));
                data.branchItem = rawTable.GetList<int>("branchItem", row);
                data.endTextContent = rawTable.GetList<int>("endTextContent", row, "|");
                data.days = new();
                List<List<int>> daySpans = rawTable.GetList2<int>("day", row, "|");
                foreach (var daySpan in daySpans)
                {
                    for (int day = daySpan[0]; day <= daySpan[1]; day++)
                        data.days.Add(day);
                }
                data.itemSets = rawTable.GetList2<int>("itemSet", row);
                data.include = GameUtil.GetConditionSet(rawTable.Get("include", row));
                data.exclude = GameUtil.GetConditionSet(rawTable.Get("exclude", row));
                data.priorityBranch = GameUtil.GetConditionSet(rawTable.Get("priorityBranch", row));
                data.resourceChange = rawTable.GetList2<int>("resChange", row);
                data.statusChange = new();
                List<List<List<int>>> statusChangeRaw = rawTable.GetList3<int>("statesChange", row, "|", "&", ":");
                foreach (var endStatusChangeRaw in statusChangeRaw)
                {
                    List<CharacterStatus> endStatusChange = new();
                    foreach (var changeInfo in endStatusChangeRaw)
                    {
                        int characterId = changeInfo[0];
                        StatusType type = (StatusType)changeInfo[1];
                        int changeNum = changeInfo[2];
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
                datas.Add(data);
            }
        }
    }
}
