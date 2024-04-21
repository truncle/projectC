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

//�����жϺͿ��ƹ�����������
public class StorylineManager : MonoBehaviour
{
    public EventStoryData CurrentData;

    //����¼��ľ�������
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
        //�״�ɸѡ, ɸ�����������������¼�
        List<EventStoryData> pool1 = EventStoryTable.datas.Where(e =>
            //����include��excludeɸѡ
            e.day.Contains(day)
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false)
        ).ToList();

        //����ɸѡ, ����������������ɸѡ
        List<EventStoryData> pool2 = pool1.Where((EventStoryData e) =>
        {
            //����priorityBranch����ɸѡ
            return processManager.CanMeetCondition(e.priorityBranch);
        }).ToList();

        pool1.Add(EventStoryTable.datas.First());

        //���ӽ���б������ѡ��һ����Ϊ����Ĺ��½ڵ�
        if (pool2.Any())
            CurrentData = pool2[Random.Range(0, pool2.Count)];
        else if (pool1.Any()) CurrentData = pool1[Random.Range(0, pool1.Count)];
        else return;
        IsChecked = CurrentData.endTextContent.Count <= 1;

        //����ʼ���õ�������䵽ContentManager�еȴ���ʾ
        DisplayContent(CurrentData);
    }


    //�����ǰ�¼�������Ϣ
    private void ClearData()
    {
        option = 0;
        provideItemId = 0;
        IsChecked = false;
    }

    //���㵱ǰ�¼����
    public void SettleSotryline()
    {
        int resultIndex = option;
        if (CurrentData.eventType == (int)EventStoryType.ItemSelect)
            resultIndex = CurrentData.provideItem.IndexOf(provideItemId) + 1;

        //���ݽ���ṩ����, ���ߺ���Դ�仯
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
        //����ʼ���õ�������䵽ContentManager�еȴ���ʾ
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

    //todo �������ݶ�̬��ʾ��ť
    private void DisplayContent(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
        string storyContent = TextTable.GetText(storyData.textContent);
        Debug.Log(string.Format("init story text:{0}", storyContent));
        contentManager.StorylineContent = storyContent;
    }

    //�޸ĵ���JournalText
    private void DisplayEnding(EventStoryData storyData, int end)
    {
        Debug.Log("Show story ending id: " + storyData.id);
        string storyEnding = TextTable.GetText(storyData.endTextContent[end]);
        Debug.Log(string.Format("end{0}, endText:{1}", end, storyEnding));
        contentManager.JournalText += "\n" + storyEnding;
    }

}
