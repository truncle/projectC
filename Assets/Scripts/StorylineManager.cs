using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;
using Util;

public enum EventStoryType
{
    Normal = 1, Select, ItemSelect, GroupSelect, FirstDay
}

//用于判断和控制故事主线内容
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;
    private ExploreManager exploreManager;

    void Start()
    {
        EventStoryTable.Init();
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
        contentManager = GetComponent<ContentManager>();
        exploreManager = GetComponent<ExploreManager>();
    }

    public void InitStoryline(int day)
    {
        ClearData();
        //首次筛选, 筛出所有满足条件的事件
        List<EventStoryData> pool1 = EventStoryTable.datas.Where(e =>
            //根据include和exclude筛选
            e.day.Contains(day)
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false)
        ).ToList();

        //二次筛选, 根据优先条件进行筛选
        List<EventStoryData> pool2 = pool1.Where((EventStoryData e) =>
        {
            //根据priorityBranch进行筛选
            return processManager.CanMeetCondition(e.priorityBranch);
        }).ToList();

        //最后从结果列表中随机选择一个作为当天的故事节点
        if (pool2.Any())
            CurrentData = pool2[Random.Range(0, pool2.Count)];
        else if (pool1.Any()) CurrentData = pool1[Random.Range(0, pool1.Count)];
        else return;

        //将初始化好的数据填充到ContentManager中等待显示
        DisplayContent(CurrentData);

        //第一天特殊逻辑
        if (day == 1)
        {
            List<EventStoryData> pool3 = EventStoryTable.datas.Where(e => e.eventType == (int)EventStoryType.FirstDay).ToList();
            DisplayEnding(pool3[Random.Range(0, pool3.Count)], 0);
        }
    }

    //清除当前事件所有信息
    private void ClearData()
    {
    }

    //结算当前事件结果
    public void SettleStoryline()
    {
        int resultIndex = contentManager.GetStorylineOption();

        if (CurrentData.eventType == (int)EventStoryType.GroupSelect)
        {
            List<int> groupList = exploreManager.checkedGroupSet.ToList();
            groupList.Sort();
            exploreManager.selectedGroup = groupList[resultIndex];
        }

        //根据结果提供奖励, 道具和资源变化
        if (CurrentData.getItem.Any())
        {
            resourceManager.AddItem(CurrentData.getItem[resultIndex]);
        }

        if (CurrentData.getRes.Any())
        {
            resourceManager.AddResource(CurrentData.getRes[resultIndex]);
        }
        //if (CurrentData.lostRes.Any())
        //{
        //    resourceManager.DeductResource(CurrentData.lostRes[resultIndex]);
        //}

        // 角色状态变化
        if (CurrentData.statusChange.Any())
        {
            resourceManager.UpdateCharacter(CurrentData.statusChange[resultIndex][0], (LiveStatus)CurrentData.statusChange[resultIndex][1]);
        }

        processManager.SaveStorylineResult(CurrentData.id, resultIndex);
        //将初始化好的数据填充到ContentManager中等待显示
        DisplayEnding(CurrentData, resultIndex);
    }

    //改为通过ContentManager主动拉取
    private void DisplayContent(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
        //string storyContent = TextTable.GetText(storyData.textContent);
        //Debug.Log(string.Format("init story text:{0}", storyContent));
        //contentManager.StorylineContent.AppendLine(storyContent);
    }

    private void DisplayEnding(EventStoryData storyData, int end)
    {
        contentManager.PushStorylineEnd(storyData, end);
        Debug.Log("Show story ending id: " + storyData.id);
        //string storyEnding = TextTable.GetText(storyData.endTextContent[end]);
        //Debug.Log(string.Format("end:{0}, endText:{1}", end, storyEnding));
        //contentManager.JournalText.AppendLine(storyEnding);
    }

}
