using System.Collections;
using System.Collections.Generic;
using Table;
using Util;
using UnityEngine;
using System.Linq;

//用于判断和控制故事探索内容

public enum ExploreState
{
    Idle, Start, Exploring,
}

public enum ExploreResult
{
    Failed, Normal, Success, SuperSuccess
}


public class ExploreManager : MonoBehaviour
{
    private ResourceManager resourceManager;
    private ProcessManager processManager;

    //探索状态
    public ExploreState exploreState = ExploreState.Idle;
    public bool PrepareExplore { get; set; } = false;
    public bool DoExplore { get; private set; }
    //已选择的组别和选中的组别
    public HashSet<int> groupSet = new();
    public int selectedGroup = 0;

    //探索中信息
    public ExploreData exploreData;
    public int exploreCharacter = 0;
    public int carryItem = 0;
    public int exploreDay = 0;

    public Dictionary<int, int> lastExploreTime = new();
    public Dictionary<int, int> groupExploreNum = new();


    void Start()
    {
        ExploreTable.Init();
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
    }

    //结算当天的探索系统
    public void SettleDayExplore()
    {
        if (exploreState == ExploreState.Idle && PrepareExplore)
        {
            exploreState = ExploreState.Start;
            PrepareExplore = false;
        }
        else if (exploreState == ExploreState.Start)
            CheckStartExplore();
        else if (exploreState == ExploreState.Start)
            CheckEndExplore();
    }

    //检查探索开始
    public void CheckStartExplore()
    {
        if (!DoExplore)
        {
            //没选人的话返还一下道具
            if (carryItem > 0)
                resourceManager.AddItem(carryItem);
            exploreState = ExploreState.Idle;
            return;
        }

        //筛出所有满足条件的探索项
        int groupId = selectedGroup;
        if (groupId == 0)
        {
            while (groupSet.Contains(groupId))
            {
                groupId = Config.GroupBase + Random.Range(1, Config.GroupNum + 1);
            }
        }
        int exploreReturnDay = Random.Range(6, 9);//todo 配置
        int exploreNum = groupExploreNum.GetValueOrDefault(groupId);
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //根据include和exclude筛选
            return e.groupId == groupId
            && e.exploreNum == exploreNum
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        exploreData = pool[Random.Range(0, pool.Count)];

        DoExplore = false;

        //todo 更新事件结果的显示
        //ShowExploreData(data, resultIndex);
    }

    //检查探索结束
    public void CheckEndExplore()
    {
        if (exploreDay < exploreData.returnDays)
            return;
        //todo 计算探索结果, 失败, 一般, 成功, 大成功
        int resultIndex = 0;

        //根据结果提供奖励, 道具和资源变化

        //根据携带道具提供奖励, 道具和资源变化
        int extraIndex = exploreData.provideItem.IndexOf(carryItem) + 1;
        if (exploreData.getItem.Any())
        {
            resourceManager.AddItem(exploreData.getItem[extraIndex]);
        }
        if (exploreData.getRes.Any())
        {
            resourceManager.AddResource(exploreData.getRes[extraIndex]);
        }

        processManager.SaveExploreResult(exploreData.id, resultIndex);
        int exploreNum = groupExploreNum.GetValueOrDefault(exploreData.groupId);
        groupExploreNum[exploreData.groupId] = exploreNum + 1;
        ClearData();
        //todo 更新事件结果的显示
        //ShowExploreData(data, resultIndex);
    }

    private void ClearData()
    {
        exploreCharacter = 0;
        carryItem = 0;
        exploreDay = 0;
        DoExplore = false;
        PrepareExplore = false;
        exploreState = ExploreState.Idle;
    }

    //--------------玩家操作--------------
    //选择探索角色
    public bool SelectCharacter(int characterId)
    {
        ////角色状态
        //if (resourceManager.GetCharacterStatus(characterId).liveStatus != LiveStatus.Normal)
        //    return false;
        ////探索冷却
        //if ((processManager.CurrentDay - lastExploreTime.GetValueOrDefault(characterId)) < Config.ExploreCoolDown)
        //    return false;
        exploreCharacter = characterId;
        DoExplore = true;
        return true;
    }

    //取消选择
    public void UnselectCharacter()
    {
        exploreCharacter = 0;
        DoExplore = false;
    }

    public bool SelectItem(int itemId)
    {
        bool result = resourceManager.DeductItem(itemId);
        carryItem = itemId;
        return result;
    }

    public void UnselectItem(int itemId)
    {
        carryItem = itemId;
        resourceManager.AddItem(itemId);
    }

    public void SelectGroup(int groupId)
    {
        selectedGroup = groupId;
    }

    //展示
    private void ShowExploreData(ExploreData storyData, int end = -1)
    {
        Debug.Log("Show explore data id: " + storyData.id);
        if (end >= 0)
            Debug.Log(string.Format("end{0}, endText:{1}", end, storyData.endTextContent[end]));
    }

}
