using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
