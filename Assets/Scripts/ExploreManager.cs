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

public struct ExploreOption
{
    public bool prepareExplore;
    public int characterId;
    public int carryItem;
}

public class ExploreManager : MonoBehaviour
{
    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;

    //探索状态
    public ExploreState exploreState = ExploreState.Idle;
    public bool PrepareExplore { get; set; } = false;

    //已选择的组别和选中的组别
    public HashSet<int> checkedGroupSet = new();
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
        contentManager = GetComponent<ContentManager>();
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
        else if (exploreState == ExploreState.Exploring)
            CheckEndExplore();
    }

    //检查探索开始
    public void CheckStartExplore()
    {
        ExploreOption option = contentManager.GetExploreOption();
        if (option.characterId <= 0)
        {
            exploreState = ExploreState.Idle;
            return;
        }

        exploreCharacter = option.characterId;
        carryItem = option.carryItem;
        //筛出所有满足条件的探索项
        int groupId = selectedGroup;
        if (groupId == 0)
        {
            do
            {
                groupId = Config.GroupBase + Random.Range(1, Config.GroupNum + 1);
            }
            while (checkedGroupSet.Contains(groupId));
            checkedGroupSet.Add(groupId);
        }

        //读取配置
        MiscData exploreLimit1 = MiscTable.Get("explore_limit_1");
        MiscData exploreLimit2 = MiscTable.Get("explore_limit_2");
        int exploreReturnDay = 1;
        if (processManager.CurrentDay <= System.Convert.ToInt32(exploreLimit1.para4))
        {
            exploreReturnDay = System.Convert.ToInt32(exploreLimit1.para3);
        }
        else
        {
            exploreReturnDay = System.Convert.ToInt32(exploreLimit2.para3);
        }
        int exploreNum = groupExploreNum.GetValueOrDefault(groupId) + 1;

        Debug.Log($"groupId: {groupId}");
        Debug.Log($"exploreNum: {exploreNum}");
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //根据include和exclude筛选
            return e.groupId == groupId
            && e.exploreNum == exploreNum
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        exploreData = pool[Random.Range(0, pool.Count)];
        exploreState = ExploreState.Exploring;
        exploreDay = 0;

        // 更新事件结果的显示
        DisplayExploreStart(exploreData);
    }

    //检查探索结束
    public void CheckEndExplore()
    {

        if (exploreState != ExploreState.Exploring)
            return;
        //if (processManager.CurrentDay - exploreDay < exploreData.returnDays)
        if (exploreDay < 1)
        {
            exploreDay++;
            return;
        }
        //计算探索结果, 失败, 一般, 成功, 大成功
        MiscData data = MiscTable.Get("explore_res_get");
        var resList = GameUtil.GetList3(data.para1);
        List<int> rateList = data.para2.Split("|").Select(int.Parse).ToList();
        int resultIndex = GameUtil.GetRandomIndices(rateList, resList.Count).First();
        List<List<int>> getResource = resList[resultIndex];

        //根据结果提供奖励, 道具和资源变化
        resourceManager.AddResource(getResource);

        //根据携带道具提供额外奖励, 道具和资源变化
        int extraIndex = exploreData.provideItem.IndexOf(carryItem) + 1;
        if (exploreData.getItem.Any())
        {
            resourceManager.AddItem(exploreData.getItem[extraIndex]);
        }
        if (exploreData.getRes.Any())
        {
            resourceManager.AddResource(exploreData.getRes[extraIndex]);
        }

        // 角色状态变化
        //if (exploreData.statusChange.Any())
        //{
        //    resourceManager.UpdateCharacter(exploreData.statusChange[resultIndex][0], (LiveStatus)exploreData.statusChange[resultIndex][1]); ;
        //}

        processManager.SaveExploreResult(exploreData.id, resultIndex);
        int exploreNum = groupExploreNum.GetValueOrDefault(exploreData.groupId);
        groupExploreNum[exploreData.groupId] = exploreNum + 1;

        //更新事件结果的显示
        DisplayExploreEnd(exploreData, 0);

        ClearData();
    }

    private void ClearData()
    {
        exploreCharacter = 0;
        carryItem = 0;
        exploreDay = 0;
        PrepareExplore = false;
        exploreState = ExploreState.Idle;
        exploreData = new();
    }

    //--------------玩家操作--------------
    ////选择探索角色
    //public bool SelectCharacter(int characterId)
    //{
    //    ////角色状态
    //    //if (resourceManager.GetCharacterStatus(characterId).liveStatus != LiveStatus.Normal)
    //    //    return false;
    //    ////探索冷却
    //    //if ((processManager.CurrentDay - lastExploreTime.GetValueOrDefault(characterId)) < Config.ExploreCoolDown)
    //    //    return false;
    //    exploreCharacter = characterId;
    //    DoExplore = true;
    //    return true;
    //}

    ////取消选择
    //public void UnselectCharacter()
    //{
    //    exploreCharacter = 0;
    //    DoExplore = false;
    //}

    //public bool SelectItem(int itemId)
    //{
    //    bool result = resourceManager.DeductItem(itemId);
    //    carryItem = itemId;
    //    return result;
    //}

    //public void UnselectItem(int itemId)
    //{
    //    carryItem = itemId;
    //    resourceManager.AddItem(itemId);
    //}

    public void SelectGroup(int groupId)
    {
        selectedGroup = groupId;
    }

    private void DisplayExploreStart(ExploreData exploreData)
    {
        contentManager.PushExploreStart(exploreData);
        Debug.Log("start explore id: " + exploreData.id);
        //string startExploreText = TextTable.GetText(exploreData.textContent);
        //Debug.Log(string.Format("start explore text:{0}", startExploreText));
        //contentManager.JournalText.AppendLine();
        //contentManager.JournalText.AppendLine(startExploreText);

    }

    private void DisplayExploreEnd(ExploreData exploreData, int end)
    {
        contentManager.PushExploreEnd(exploreData, end);
        Debug.Log("end explore id: " + exploreData.id);
        //string endExploreText = TextTable.GetText(exploreData.endTextContent[end]);
        //Debug.Log(string.Format("end explore text:{0}", endExploreText));
        //contentManager.JournalText.AppendLine();
        //contentManager.JournalText.AppendLine(endExploreText);
    }

}
