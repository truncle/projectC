using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using System.Linq;
using Util;

//��������غ��л��߼�, ͳ�ƴ�����ܸ���ģ��Ľ���������¼
public class ProcessManager : MonoBehaviour
{
    public int CurrentDay { get; private set; }
    public int CurrentCharacter { get; private set; }

    private StorylineManager storylineManager;
    private ExploreManager exploreManager;
    private ResourceManager resourceManager;
    private ContentManager contentManager;

    //��Ϸ�еĸ��ּ�¼
    public List<DayRecord> dayRecord = new();
    public Dictionary<int, int> storylineRecord = new();
    public Dictionary<int, int> exploreRecord = new();

    public void Start()
    {
        CurrentDay = 1;
        storylineManager = GetComponent<StorylineManager>();
        resourceManager = GetComponent<ResourceManager>();
        exploreManager = GetComponent<ExploreManager>();
        contentManager = GetComponent<ContentManager>();
        resourceManager.Init();
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
            storylineManager.Select(1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            exploreManager.SelectCharacter(1);
            exploreManager.CheckStartExplore();
        }
    }

    public void InitCurrentDay()
    {
        Debug.Log("Init day" + CurrentDay);
        storylineManager.InitDayStoryline(CurrentDay);
        contentManager.Sync();
    }

    //��Ҫ�������ǿ��ѡ���Ƿ�ѡ�����
    public bool EndCurrentDay()
    {
        if (!storylineManager.IsChecked)
            return false;
        Debug.Log("End day " + CurrentDay);
        storylineManager.SettleCurrentDay();
        resourceManager.SyncResource();
        CurrentDay += 1;
        contentManager.Sync();
        return true;
    }

    //�ж�ĳ���������Ƿ�����, ��һ��û��conditionʱ��Ĭ��ֵ
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
                    case "MIND":
                        value = resourceManager.GetCharacterStatus(Convert.ToInt32(condition.param[0])).GetValue(StatusType.Mind);
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "EVT":
                        value = storylineRecord.GetValueOrDefault(Convert.ToInt32(condition.param[0]));
                        conditionValue = Convert.ToInt32(condition.param[1]);
                        break;
                    case "ITEM":
                        value = resourceManager.HasItem(Convert.ToInt32(condition.param[0])) ? 1 : 0;
                        conditionValue = 1;
                        break;
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
        return result;
    }

    //�����¼����
    public void SaveStorylineResult(int id, int result)
    {
        storylineRecord.Add(id, result);
    }

    //����̽�����
    public void SaveExploreResult(int id, int result)
    {
        exploreRecord.Add(id, result);
    }

}
