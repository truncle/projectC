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
    public Dictionary<int, int> characterExploreNum = new();

    private ResourceManager resourceManager;
    private ProcessManager processManager;

    public bool DoExplore { get; private set; }

    void Start()
    {
        EventStoryTable.Init();
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
    }

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
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //根据include和exclude筛选
            return e.groupId == groupId
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        var data = pool[Random.Range(0, pool.Count)];
    }

    //选择探索角色
    public bool SelectCharacter(int characterId)
    {
        //角色状态
        if (resourceManager.GetCharacterStatus(characterId).liveStatus != LiveStatus.Normal)
            return false;
        //探索冷却
        if ((processManager.CurrentDay - lastExploreTime.GetValueOrDefault(characterId)) < Config.ExploreCoolDown)
            return false;
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

}
