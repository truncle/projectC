using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;

//用来管理回合切换逻辑, 统计处理汇总各个模块的结果并保存记录
public class ProcessManager : MonoBehaviour
{
    public int CurrentDay { get; private set; }
    public int CurrentCharacter { get; private set; }

    private StorylineManager storylineManager;
    private ExploreManager exploreManager;
    private ResourceManager resourceManager;

    //游戏中的各种记录
    Dictionary<int, int> storylineRecord = new();

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

    public bool CanMeetCondition(List<List<Condition>> conditionSet)
    {

        foreach (var andList in conditionSet)
        {
            foreach (var condition in andList)
            {
                int value = 0;
                int conditionValue = -1;
                bool result = true;
                switch (condition.name)
                {
                    case "HUNGRY":
                        value = resourceManager.GetCharacterStatus(Convert.ToInt32(condition.param[0])).hungry;
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "THIRSTY":
                        value = resourceManager.GetCharacterStatus(Convert.ToInt32(condition.param[0])).thirsty;
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "MIND":
                        value = resourceManager.GetCharacterStatus(Convert.ToInt32(condition.param[0])).mind;
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "EVT":
                        value = storylineRecord.GetValueOrDefault(Convert.ToInt32(condition.param[0]));
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "ITEM":
                        break;
                        value = resourceManager.GetItemNum(Convert.ToInt32(condition.param[0]));
                        conditionValue = 1;
                    case "CHARACTER":
                        value = CurrentCharacter;
                        conditionValue = Convert.ToInt32(condition.param[0]);
                        break;
                }
                switch (condition.func)
                {
                    case "MORE":
                        result = value > conditionValue;
                        break;
                    case "LESS":
                        result = value < conditionValue;
                        break;
                    case "EQUAL":
                        result = value == conditionValue;
                        break;
                }
                if (!result)
                    return false;
            }
        }
        return true;
    }
}
