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
        public int textContent; //事件出现时的文本
        public int eventType;
        public int eventTest;
        public List<int> day;
        public List<int> provideItem; //可以提供的道具
        public List<List<int>> provideRes; //可选提供的资源
        public List<int> endTextContent; //事件结束文本
        public List<List<int>> getItem;
        public List<List<List<int>>> getRes;
        public List<List<List<int>>> lostRes;
        public List<List<int>> statusChange; // 状态改变
        public List<List<Condition>> include;
        public List<List<Condition>> exclude;
        public List<List<Condition>> priorityBranch;
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
            int count = 0;
            foreach (var row in rawTable.data)
            {
                count++;
                if (count == 30)
                    break;
                int eventType = Convert.ToInt32(rawTable.Get("eventType", row));
                if (eventType == 0)
                    //剔除空事件
                    continue;
                EventStoryData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.textContent = Convert.ToInt32(rawTable.Get("textContent", row));
                data.eventType = Convert.ToInt32(rawTable.Get("eventType", row));
                data.eventTest = Convert.ToInt32(rawTable.Get("eventTest", row));
                data.day = new();
                List<int> daySpan = rawTable.GetList<int>("day", row, ":");
                if (daySpan.Count == 1)
                    data.day.Add(daySpan[0]);
                else
                    for (int day = daySpan[0]; day <= daySpan[1]; day++)
                        data.day.Add(day);
                data.provideItem = rawTable.GetList<int>("provideItem", row);
                data.provideRes = rawTable.GetList2<int>("provideRes", row);
                data.endTextContent = rawTable.GetList<int>("endTextContent", row, "|");
                data.getItem = rawTable.GetList2<int>("getItem", row);
                data.getRes = rawTable.GetList3<int>("getRes", row);
                data.lostRes = rawTable.GetList3<int>("lostRes", row);
                data.statusChange = rawTable.GetList2<int>("statusChange", row);
                data.include = GameUtil.GetConditionSet(rawTable.Get("include", row));
                data.exclude = GameUtil.GetConditionSet(rawTable.Get("exclude", row));
                data.priorityBranch = GameUtil.GetConditionSet(rawTable.Get("priorityBranch", row));
                datas.Add(data);
            }
        }
    }
}
