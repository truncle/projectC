using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Table
{
    public struct EventStoryData
    {
        public int id;
        public int textContent;
        public int eventType;
        public List<int> endTextContent;
        public List<int> days;
        public List<int> hungerChange;
        public List<int> thirstyChange;
        public List<int> mindChange;
        public List<List<int>> itemSets;
        public int item;
        public string include;
        public string exclude;
        public string priorityBranch;
    }

    public static class EventStoryTable
    {
        public static List<EventStoryData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("EventStory");
            foreach(var row in rawTable.data)
            {
                EventStoryData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.textContent = Convert.ToInt32(rawTable.Get("textContent", row));
                data.eventType = Convert.ToInt32(rawTable.Get("eventType", row));
                data.endTextContent = rawTable.GetList<int>("endTextContent", row, "|");
                data.days = rawTable.GetList<int>("days", row, "|");
                data.hungerChange = rawTable.GetList<int>("hungerChange", row, "|");
                data.thirstyChange = rawTable.GetList<int>("thirstyChange", row, "|");
                data.mindChange = rawTable.GetList<int>("mindChange", row, "|");
            }
        }
    }
}
