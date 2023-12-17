using System.Collections;
using System.Collections.Generic;
using Table;
using Util;
using UnityEngine;
using System.Linq;

//用于判断和控制故事探索内容

public class ExploreManager : MonoBehaviour
{
    public int exploreCharacter = 0;
    public List<int> selectedItemSet = new();

    public Dictionary<int, int> lastExploreTime = new();
    public Dictionary<int, int> groupExploreNum = new();

    private ResourceManager resourceManager;
    private ProcessManager processManager;

    public bool DoExplore { get; private set; }

    void Start()
    {
        ExploreTable.Init();
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
    }


    //检查探索开始和结算
    public void CheckStartExplore()
    {
        if (!DoExplore)
        {
            //没选人的话返还一下道具
            foreach (var item in selectedItemSet)
            {
                resourceManager.AddItem(item);
            }
            return;
        }

        //筛出所有满足条件的探索项
        int groupId = Config.GroupBase + Random.Range(1, Config.GroupNum);
        int exploreNum = groupExploreNum.GetValueOrDefault(groupId);
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //根据include和exclude筛选
            return e.groupId == groupId
            && e.exploreNum == exploreNum
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        var data = pool[Random.Range(0, pool.Count)];

        int resultIndex = 0;
        for (int i = 0; i < data.itemSets.Count; i++)
        {
            if (GameUtil.ListContains(selectedItemSet, data.itemSets[i]))
            {
                resultIndex = i + 1;
            }
        }

        //根据结果提供奖励, 道具和资源变化
        //foreach (var statusChange in data.statusChange[resultIndex])
        //{
        //    resourceManager.UpdateCharacter(statusChange);
        //}
        //resourceManager.GetItembox(data.branchItem[resultIndex]);
        resourceManager.GetItembox(data.itemBoxId);

        processManager.SaveExploreResult(data.id, resultIndex);
        groupExploreNum[groupId] = exploreNum + 1;
        ClearData();
        //todo 更新事件结果的显示
        //ShowExploreData(data, resultIndex);
    }
    private void ClearData()
    {
        exploreCharacter = 0;
        selectedItemSet.Clear();
        DoExplore = false;
    }

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
        if (result)
            selectedItemSet.Add(itemId);
        return result;
    }

    public void UnselectItem(int itemId)
    {
        if (selectedItemSet.Remove(itemId))
            resourceManager.AddItem(itemId);
    }

    //展示
    private void ShowExploreData(ExploreData storyData, int end = -1)
    {
        Debug.Log("Show explore data id: " + storyData.id);
        if (end >= 0)
            Debug.Log(string.Format("end{0}, endText:{1}", end, storyData.endTextContent[end]));
    }

}
