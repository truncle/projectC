using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;


//用于判断和控制故事主线内容
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;
    public bool IsChecked { get; private set; }

    void Start()
    {
        EventStoryTable.Init();
    }

    public void InitDayStoryline(int day)
    {
        //首次筛选, 筛出所有满足条件的事件
        List<EventStoryData> pool1 = EventStoryTable.datas.Where((EventStoryData e) =>
        {
            //todo 还要根据include和exclude筛选, 需要record的数据
            return e.days.Contains(day);
        }).ToList();

        //二次筛选, 根据优先条件进行筛选
        List<EventStoryData> pool2 = pool1.Where((EventStoryData e) =>
        {
            //todo 根据priorityBranch进行筛选
            return true;
        }).ToList();

        //最后从结果列表中随机选择一个作为当天的故事节点
        if (pool2.Any())
            CurrentData = pool2[Random.Range(0, pool2.Count)];
        else CurrentData = pool2[Random.Range(0, pool1.Count)];
        IsChecked = CurrentData.endTextContent.Count <= 1;
        ShowStoryData(CurrentData);
    }

    //展示
    private void ShowStoryData(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
    }

}
