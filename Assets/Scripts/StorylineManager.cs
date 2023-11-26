using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;


//用于判断和控制故事主线内容
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    //每天的选择数据
    public int option = 0;
    public HashSet<int> itemSet = new();
    public bool IsChecked { get; private set; }

    private ResourceManager resourceManager;
    private ProcessManager processManager;

    void Start()
    {
        EventStoryTable.Init();
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
    }

    public void InitDayStoryline(int day)
    {
        ClearDayData();
        //首次筛选, 筛出所有满足条件的事件
        List<EventStoryData> pool1 = EventStoryTable.datas.Where((EventStoryData e) =>
        {
            //根据include和exclude筛选
            return e.days.Contains(day)
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude);
        }).ToList();

        //二次筛选, 根据优先条件进行筛选
        List<EventStoryData> pool2 = pool1.Where((EventStoryData e) =>
        {
            //根据priorityBranch进行筛选
            return processManager.CanMeetCondition(e.priorityBranch);
        }).ToList();

        //最后从结果列表中随机选择一个作为当天的故事节点
        if (pool2.Any())
            CurrentData = pool2[Random.Range(0, pool2.Count)];
        else CurrentData = pool1[Random.Range(0, pool1.Count)];
        IsChecked = CurrentData.endTextContent.Count <= 1;
        ShowStoryData(CurrentData);
    }

    private void ClearDayData()
    {
        option = 0;
        itemSet.Clear();
        IsChecked = false;
    }

    public void Select(int option)
    {
        this.option = option;
        IsChecked = true;
    }

    public bool UseItem(int itemId)
    {
        bool result = resourceManager.DeductItem(itemId, 1);
        if (result)
            itemSet.Add(itemId);
        return result;
    }

    public void CancelItem(int itemId)
    {
        itemSet.Remove(itemId);
    }

    //展示
    private void ShowStoryData(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
    }

}
