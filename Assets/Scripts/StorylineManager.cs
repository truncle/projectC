using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;
using Util;


public enum EventStoryType
{
    Normal, Select, ItemSelect
}

//用于判断和控制故事主线内容
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    //这个事件的决策数据
    public int option = 0;
    public int provideItemId = 0;
    public bool IsChecked { get; private set; }

    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;

    void Start()
    {
        EventStoryTable.Init();
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
        contentManager = GetComponent<ContentManager>();
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

        pool1.Add(EventStoryTable.datas.First());

        //最后从结果列表中随机选择一个作为当天的故事节点
        if (pool2.Any())
            CurrentData = pool2[Random.Range(0, pool2.Count)];
        else if (pool1.Any()) CurrentData = pool1[Random.Range(0, pool1.Count)];
        else return;
        IsChecked = CurrentData.endTextContent.Count <= 1;

        //将初始化好的数据填充到ContentManager中等待显示
        DisplayContent(CurrentData);
    }


    //清除当前事件所有信息
    private void ClearData()
    {
        option = 0;
        provideItemId = 0;
        IsChecked = false;
    }

    //结算当前事件结果
    public void SettleSotryline()
    {
        int resultIndex = option;
        if (CurrentData.eventType == (int)EventStoryType.ItemSelect)
            resultIndex = CurrentData.provideItem.IndexOf(provideItemId) + 1;

        //根据结果提供奖励, 道具和资源变化
        if (CurrentData.getItem.Any())
        {
            resourceManager.AddItem(CurrentData.getItem[resultIndex]);
        }

        if (CurrentData.getRes.Any())
        {
            resourceManager.AddResource(CurrentData.getRes[resultIndex]);
            //resourceManager.AddResource(ResourceType.Water, 20);
            //resourceManager.AddResource(ResourceType.Food, 10);
        }
        //if (CurrentData.lostRes.Any())
        //{
        //    resourceManager.DeductResource(CurrentData.getRes[resultIndex]);
        //}
        //todo statusChange

        processManager.SaveStorylineResult(CurrentData.id, resultIndex);
        //将初始化好的数据填充到ContentManager中等待显示
        DisplayEnding(CurrentData, resultIndex);
    }

    public void Select(int option)
    {
        this.option = option;
        IsChecked = true;
        if (CurrentData.provideRes.Any())
        {
            resourceManager.DeductResource(CurrentData.provideRes);
        }
    }

    public bool ProvideItem(int itemId)
    {
        bool result = resourceManager.DeductItem(itemId);
        if (result)
            provideItemId = itemId;
        return result;
    }

    //todo 根据数据动态显示按钮
    private void DisplayContent(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
        string storyContent = TextTable.GetText(storyData.textContent);
        Debug.Log(string.Format("init story text:{0}", storyContent));
        contentManager.StorylineContent = storyContent;
    }

    //修改的是JournalText
    private void DisplayEnding(EventStoryData storyData, int end)
    {
        Debug.Log("Show story ending id: " + storyData.id);
        string storyEnding = TextTable.GetText(storyData.endTextContent[end]);
        Debug.Log(string.Format("end{0}, endText:{1}", end, storyEnding));
        contentManager.JournalText += "\n" + storyEnding;
    }

}
