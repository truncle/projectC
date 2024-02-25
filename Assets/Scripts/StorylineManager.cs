using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;
using Util;


//用于判断和控制故事主线内容
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    //这个事件的决策数据
    public int option = 0;
    public List<int> itemSet = new();
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
            e.days.Contains(day)
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
        else CurrentData = pool1[Random.Range(0, pool1.Count)];
        IsChecked = CurrentData.endTextContent.Count <= 1;

        //将初始化好的数据填充到ContentManager中等待显示
        ShowStoryData(CurrentData);
    }


    //清除当前事件所有信息
    private void ClearData()
    {
        option = 0;
        itemSet.Clear();
        IsChecked = false;
    }

    //结算当前事件结果
    public void SettleSotryline()
    {
        int resultIndex = option;
        for (int i = 0; i < CurrentData.itemSets.Count; i++)
        {
            if (GameUtil.ListContains(itemSet, CurrentData.itemSets[i]))
            {
                resultIndex = i + 1;
            }
        }
        //根据结果提供奖励, 道具和资源变化
        foreach (var statusChange in CurrentData.statusChange[resultIndex])
        {
            resourceManager.UpdateCharacter(statusChange);
        }
        //resourceManager.GetItembox(CurrentData.branchItem[resultIndex]);
        resourceManager.GetItembox(100002);

        processManager.SaveStorylineResult(CurrentData.id, resultIndex);
        //将初始化好的数据填充到ContentManager中等待显示
        ShowStoryData(CurrentData, resultIndex);
    }

    public void Select(int option)
    {
        this.option = option;
        IsChecked = true;
    }

    public bool SelectItem(int itemId)
    {
        bool result = resourceManager.DeductItem(itemId);
        if (result)
            itemSet.Add(itemId);
        return result;
    }

    public void UnselectItem(int itemId)
    {
        if (itemSet.Remove(itemId))
            resourceManager.AddItem(itemId);
    }

    //展示
    private void ShowStoryData(EventStoryData storyData, int end = -1)
    {
        Debug.Log("Show story data id: " + storyData.id);
        string pushText;
        if (end >= 0)
        {
            pushText = TextTable.GetText(storyData.endTextContent[end]);
            Debug.Log(string.Format("end{0}, endText:{1}", end, pushText));
        }
        else
        {
            pushText = TextTable.GetText(storyData.textContent);
            Debug.Log(string.Format("init story text:{0}", pushText));
            contentManager.PushContent("story text: " + TextTable.GetText(storyData.textContent));
        }
        contentManager.PushContent("story text: " + pushText);
    }

}
