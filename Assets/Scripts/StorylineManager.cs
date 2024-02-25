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

    //����¼��ľ�������
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

        //����ʼ���õ�������䵽ContentManager�еȴ���ʾ
        ShowStoryData(CurrentData);
    }


    //�����ǰ�¼�������Ϣ
    private void ClearData()
    {
        option = 0;
        itemSet.Clear();
        IsChecked = false;
    }

    //���㵱ǰ�¼����
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
        //���ݽ���ṩ����, ���ߺ���Դ�仯
        foreach (var statusChange in CurrentData.statusChange[resultIndex])
        {
            resourceManager.UpdateCharacter(statusChange);
        }
        //resourceManager.GetItembox(CurrentData.branchItem[resultIndex]);
        resourceManager.GetItembox(100002);

        processManager.SaveStorylineResult(CurrentData.id, resultIndex);
        //����ʼ���õ�������䵽ContentManager�еȴ���ʾ
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
