using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Test
{
    public int num;
}

//��������غ��л��߼�, ͳ�ƴ�����ܸ���ģ��Ľ���������¼
public class ProcessManager : MonoBehaviour
{
    public int CurrentDay { get; private set; }

    private StorylineManager storylineManager;
    private ExploreManager exploreManager;
    private ResourceManager resourceManager;

    public void Start()
    {
        CurrentDay = 1;
        storylineManager = GetComponent<StorylineManager>();
        resourceManager = GetComponent<ResourceManager>();
        exploreManager = GetComponent<ExploreManager>();
    }

    public void InitCurrentDay()
    {
        resourceManager.Init();
        storylineManager.InitDayStoryline(CurrentDay);
    }

    //��Ҫ�������ǿ��ѡ���Ƿ�ѡ�����
    public bool EndCurrentDay()
    {
        CurrentDay += 1;
        return true;
    }
}
