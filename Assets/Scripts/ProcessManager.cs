using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using System.Linq;
using Util;

//用来管理回合切换逻辑, 统计处理汇总各个模块的结果并保存记录
public class ProcessManager : MonoBehaviour
{
    public int CurrentDay { get; private set; }
    public int CurrentCharacter { get; private set; }

    private StorylineManager storylineManager;
    private ExploreManager exploreManager;
    private ResourceManager resourceManager;
    private ContentManager contentManager;

    //游戏中的各种记录
    public List<DayRecord> dayRecord = new();
    public Dictionary<int, int> storylineRecord = new();
    public Dictionary<int, int> exploreRecord = new() { [0] = 1 };

    public void Start()
    {
        CurrentDay = 1;
        storylineManager = GetComponent<StorylineManager>();
        resourceManager = GetComponent<ResourceManager>();
        exploreManager = GetComponent<ExploreManager>();
        contentManager = GetComponent<ContentManager>();
        resourceManager.Init();
        contentManager.Init();

        InitCurrentDay();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            InitCurrentDay();
        else if (Input.GetKeyDown(KeyCode.W))
        {
            EndCurrentDay();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            //storylineManager.Select(1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            exploreManager.CheckStartExplore();
        }
    }

    //todo 改为事件处理, 不过现在这样更好展示流程
    //初始化当天的数据, 之后同步显示内容
    public void InitCurrentDay()
    {
        Debug.Log("Init day" + CurrentDay);
        storylineManager.InitStoryline(CurrentDay);
        contentManager.Sync();
    }

    //结算当天的各种选择和事件
    public bool EndCurrentDay()
    {
        Debug.Log("End day " + CurrentDay);
        contentManager.Clear();
        storylineManager.SettleStoryline();
        exploreManager.SettleDayExplore();
        resourceManager.SettleDayResource();
        CurrentDay += 1;
        //contentManager.Sync();
        return true;
    }

    //判断某个条件集是否满足, 传一个没有condition时的默认值
    public bool CanMeetCondition(List<List<Condition>> conditionSet, bool result = true)
    {
        foreach (var andList in conditionSet)
        {
            foreach (var condition in andList)
            {
                int value = 0;
                int conditionValue = -1;
                result = true;
                switch (condition.name)
                {
                    case "HUNGRY":
                        value = resourceManager.GetCharacterStatus(Convert.ToInt32(condition.param[0])).GetValue(StatusType.Hungry);
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "THIRSTY":
                        value = resourceManager.GetCharacterStatus(Convert.ToInt32(condition.param[0])).GetValue(StatusType.Thirsty);
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "FOOD":
                        value = resourceManager.GetResourceNum(ResourceType.Food);
                        conditionValue = Convert.ToInt32(condition.param[0]);
                        break;
                    case "WATER":
                        value = resourceManager.GetResourceNum(ResourceType.Water);
                        conditionValue = Convert.ToInt32(condition.param[0]);
                        break;
                    case "ITEM":
                        value = resourceManager.HasItem(Convert.ToInt32(condition.param[0])) ? 1 : 0;
                        conditionValue = 1;
                        break;
                    case "CHARACTER":
                        value = CurrentCharacter;
                        conditionValue = Convert.ToInt32(condition.param[0]);
                        break;
                    case "EXPLORE":
                        value = exploreRecord.GetValueOrDefault(Convert.ToInt32(condition.param[0]));
                        //conditionValue = Convert.ToInt32(condition.param[1]);
                        conditionValue = 1;
                        break;
                    case "EVT":
                        value = storylineRecord.GetValueOrDefault(Convert.ToInt32(condition.param[0]));
                        //conditionValue = Convert.ToInt32(condition.param[1]);
                        conditionValue = 1;
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
        return result;
    }

    //保存事件结果
    public void SaveStorylineResult(int id, int result)
    {
        //storylineRecord.Add(id, result);
        //storylineRecord[id] = result;
        storylineRecord[id] = 1;
    }

    //保存探索结果
    public void SaveExploreResult(int id, int result)
    {
        //exploreRecord.Add(id, result);
        exploreRecord[id] = 1;
        exploreRecord[0] = 0;
    }

}
