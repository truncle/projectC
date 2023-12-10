using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;
using Util;


//�����жϺͿ��ƹ�����������
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    //ÿ��ľ�������
    public int option = 0;
    public List<int> itemSet = new();
    public bool IsChecked { get; private set; }

    private ResourceManager resourceManager;
    private ProcessManager processManager;

    private bool isSettled = false;

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
        List<EventStoryData> pool1 = EventStoryTable.datas.Where(e =>
            //����include��excludeɸѡ
            e.days.Contains(day)
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false)
        ).ToList();

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
        isSettled = false;
    }

    //���㵱ǰ�¼����
    public void SettleCurrentDay()
    {
        if (isSettled)
            return;
        int resultIndex = option;
        for (int i = 0; i < CurrentData.itemSets.Count; i++)
        {
            if (GameUtil.ListContains(itemSet, CurrentData.itemSets[i]))
            {
                resultIndex = i + 1;
            }
        }
        //���ݽ���ṩ����, ���ߺ���Դ�仯
        foreach (var statusChange in CurrentData.statusChange[resultIndex])
        {
            resourceManager.UpdateCharacter(statusChange);
        }
        //resourceManager.GetItembox(CurrentData.branchItem[resultIndex]);
        resourceManager.GetItembox(100002);

        isSettled = true;
        processManager.SaveStorylineResult(CurrentData.id, resultIndex);
        //todo �����¼��������ʾ
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

    //չʾ
    private void ShowStoryData(EventStoryData storyData, int end = -1)
    {
        Debug.Log("Show story data id: " + storyData.id);
        if (end >= 0)
            Debug.Log(string.Format("end{0}, endText:{1}", end, TextTable.GetText(storyData.endTextContent[end])));
        else
            Debug.Log(string.Format("init story text:{0}", TextTable.GetText(storyData.textContent)));
    }

}
