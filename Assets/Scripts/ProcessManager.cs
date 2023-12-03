using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using System.Linq;

//��������غ��л��߼�, ͳ�ƴ�����ܸ���ģ��Ľ���������¼
public class ProcessManager : MonoBehaviour
{
    public int CurrentDay { get; private set; }
    public int CurrentCharacter { get; private set; }

    private StorylineManager storylineManager;
    private ExploreManager exploreManager;
    private ResourceManager resourceManager;

    //��Ϸ�еĸ��ּ�¼
    public List<DayRecord> dayRecord = new();
    public Dictionary<int, int> storylineRecord = new();

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
        if (!storylineManager.IsChecked)
            return false;
        CurrentDay += 1;
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
                        value = resourceManager.GetItemNum(Convert.ToInt32(condition.param[0]));
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

}
