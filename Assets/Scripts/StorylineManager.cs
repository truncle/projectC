using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Table;


//�����жϺͿ��ƹ�����������
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
        //�״�ɸѡ, ɸ�����������������¼�
        List<EventStoryData> pool1 = EventStoryTable.datas.Where((EventStoryData e) =>
        {
            //todo ��Ҫ����include��excludeɸѡ, ��Ҫrecord������
            return e.days.Contains(day);
        }).ToList();

        //����ɸѡ, ����������������ɸѡ
        List<EventStoryData> pool2 = pool1.Where((EventStoryData e) =>
        {
            //todo ����priorityBranch����ɸѡ
            return true;
        }).ToList();

        //���ӽ���б������ѡ��һ����Ϊ����Ĺ��½ڵ�
        if (pool2.Any())
            CurrentData = pool2[Random.Range(0, pool2.Count)];
        else CurrentData = pool2[Random.Range(0, pool1.Count)];
        IsChecked = CurrentData.endTextContent.Count <= 1;
        ShowStoryData(CurrentData);
    }

    //չʾ
    private void ShowStoryData(EventStoryData storyData)
    {
        Debug.Log("Show story data id: " + storyData.id);
    }

}
