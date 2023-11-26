using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;


//�����жϺͿ��ƹ�����������
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    //ÿ���ѡ������
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
        //�״�ɸѡ, ɸ�����������������¼�
        List<EventStoryData> pool1 = EventStoryTable.datas.Where((EventStoryData e) =>
        {
            //����include��excludeɸѡ
            return e.days.Contains(day)
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude);
        }).ToList();

        //����ɸѡ, ����������������ɸѡ
        List<EventStoryData> pool2 = pool1.Where((EventStoryData e) =>
        {
            //����priorityBranch����ɸѡ
            return processManager.CanMeetCondition(e.priorityBranch);
        }).ToList();

        //���ӽ���б������ѡ��һ����Ϊ����Ĺ��½ڵ�
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

    //չʾ
    private void ShowStoryData(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
    }

}
