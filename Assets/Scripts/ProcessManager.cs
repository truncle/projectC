using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Test
{
    public int num;
}

//用来管理回合切换逻辑, 统计处理汇总各个模块的结果并保存记录
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

    //需要检查所有强制选项是否选择完毕
    public bool EndCurrentDay()
    {
        CurrentDay += 1;
        return true;
    }
}
