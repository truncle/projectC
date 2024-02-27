using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table;
using UnityEngine;

public enum ResourceType
{
    Food, Water,
}

public enum StatusType
{
    Hungry, Thirsty, Mind
}

public enum LiveStatus
{
    Normal, Dead, Crazy
}

public struct CharacterStatus
{
    public int characterId;
    public Dictionary<StatusType, int> statusValues;
    public LiveStatus liveStatus;//存活状态

    public int GetValue(StatusType type)
    {
        return statusValues.GetValueOrDefault(type, 0);
    }

    public void ChangeValue(StatusType type, int num)
    {
        statusValues[type] = statusValues.GetValueOrDefault(type) + num;
    }

    public void ChangeValue(KeyValuePair<StatusType, int> pair)
    {
        ChangeValue(pair.Key, pair.Value);
    }
}

//用来管理每轮的资源变更
public class ResourceManager : MonoBehaviour
{
    public List<CharacterStatus> characters;
    public Dictionary<ResourceType, int> resourceValues;
    public HashSet<int> items;

    //回合中暂存, 回合结束同步到资源值中
    public List<CharacterStatus> charactersTemp;
    public Dictionary<ResourceType, int> resourceValuesTemp;
    public HashSet<int> itemsTemp;

    //本日资源分配
    public Dictionary<int, Dictionary<ResourceType, int>> resourceAlloc;

    public void Start()
    {
    }

    //初始化基本资源值
    public void Init()
    {
        characters = new()
        {
            new() { characterId = 1, statusValues = new() },
            new() { characterId = 2, statusValues = new() },
            new() { characterId = 3, statusValues = new() },
            new() { characterId = 4, statusValues = new() },
        };
        resourceValues = new();
        items = new();

        charactersTemp = new()
        {
            new() { characterId = 1, statusValues = new() },
            new() { characterId = 2, statusValues = new() },
            new() { characterId = 3, statusValues = new() },
            new() { characterId = 4, statusValues = new() },
        };
        resourceValuesTemp = new();
        itemsTemp = new();
    }

    //分配资源
    public bool AllocResource(int characterId, ResourceType type, int num)
    {
        if (!HasResource(type, num))
            return false;
        if (resourceAlloc.ContainsKey(characterId))
            resourceAlloc[characterId] = new();
        Dictionary<ResourceType, int> characterAlloc = resourceAlloc[characterId];
        characterAlloc[type] = characterAlloc.GetValueOrDefault(type) + num;
        DeductResource(type, num);
        return true;
    }
    public bool UnallocResource(int characterId, ResourceType type, int num)
    {
        if (resourceAlloc.ContainsKey(characterId))
            resourceAlloc[characterId] = new();
        Dictionary<ResourceType, int> characterAlloc = resourceAlloc[characterId];
        if (characterAlloc.GetValueOrDefault(type) < num)
            return false;
        characterAlloc[type] = characterAlloc.GetValueOrDefault(type) - num;
        AddResource(type, num);
        return true;
    }

    //结算资源分配结果
    public void SettleCurrentDay()
    {
        //根据资源分配情况改变角色状态
        foreach (var characterRes in resourceAlloc)
        {
            Dictionary<StatusType, int> changeStatus = new();
            foreach (var res in characterRes.Value)
            {
                changeStatus.Add((StatusType)res.Key, res.Value);
            }
            UpdateCharacter(characterRes.Key, changeStatus);
        }
        resourceAlloc.Clear();
    }


    //==================== 常用方法 =======================

    //道具是否足够
    public bool HasItem(int itemId)
    {
        return itemsTemp.Contains(itemId);
    }

    public bool HasItem(HashSet<int> items)
    {
        foreach (int itemId in items)
        {
            if (!itemsTemp.Contains(itemId))
                return false;
        }
        return true;
    }

    //扣除道具
    public bool DeductItem(int itemId)
    {
        return itemsTemp.Remove(itemId);
    }

    public bool DeductItem(HashSet<int> deductItems)
    {
        if (!HasItem(deductItems))
            return false;
        foreach (var itemId in deductItems)
            itemsTemp.Remove(itemId);
        return true;
    }

    //增加道具
    public bool AddItem(int itemId)
    {
        if (itemId <= 0)
            return false;
        return itemsTemp.Add(itemId);
    }

    public void AddItem(List<int> addItems)
    {
        foreach (var item in addItems)
        {
            AddItem(item);
        }
    }

    //修改角色状态, 还需要处理各种状态 < 0时的情况
    public bool UpdateCharacter(CharacterStatus changeStatus)
    {
        CharacterStatus characterStatus = charactersTemp.Where(e => e.characterId == changeStatus.characterId).First();
        if (characterStatus.liveStatus != LiveStatus.Normal)
            return false;
        if (changeStatus.liveStatus != LiveStatus.Normal)
        {
            characterStatus.liveStatus = changeStatus.liveStatus;
            return true;
        }
        foreach (StatusType statusType in Enum.GetValues(typeof(StatusType)))
        {
            if (changeStatus.GetValue(statusType) + characterStatus.GetValue(statusType) < 0)
                return false;
        }

        foreach (var changeValue in changeStatus.statusValues)
        {
            characterStatus.ChangeValue(changeValue);
        }

        return true;
    }

    public bool UpdateCharacter(int characterId, Dictionary<StatusType, int> changeValues)
    {
        return UpdateCharacter(new()
        {
            characterId = characterId,
            statusValues = changeValues,
        });
    }

    //资源是否足够
    public bool HasResource(ResourceType type, int num)
    {
        return resourceValuesTemp.GetValueOrDefault(type) >= num;
    }

    public void DeductResource(ResourceType type, int num)
    {
        resourceValuesTemp[type] = Math.Max(resourceValuesTemp.GetValueOrDefault(type) - num, 0);
    }
    public void DeductResource(int type, int num)
    {
        if (type <= 0)
            return;
        DeductResource((ResourceType)type, num);
    }
    public void DeductResource(List<List<int>> resList)
    {
        foreach (var res in resList)
        {
            DeductResource(res[1], res[2]);
        }
    }

    public void AddResource(ResourceType type, int num)
    {
        resourceValuesTemp[type] = resourceValuesTemp.GetValueOrDefault(type) + num;
    }
    public void AddResource(int type, int num)
    {
        if (type <= 0)
            return;
        AddResource((ResourceType)type, num);
    }
    public void AddResource(List<List<int>> resList)
    {
        foreach (var res in resList)
        {
            AddResource(res[1], res[2]);
        }
    }

    //将资源变更同步到当前资源
    public void SyncResource()
    {
        characters = new();
        foreach (var characterInfo in charactersTemp)
        {
            characters.Add(
                new()
                {
                    characterId = characterInfo.characterId,
                    statusValues = new(characterInfo.statusValues),
                    liveStatus = characterInfo.liveStatus,
                });
        }
        resourceValues = new(resourceValuesTemp);
        items = new(itemsTemp);
    }

    //查询接口
    public CharacterStatus GetCharacterStatus(int characterId)
    {
        return charactersTemp.Where(e => e.characterId == characterId).First();
    }

    ////获取道具箱中的道具
    //public List<int> GetItembox(int itemboxId)
    //{
    //    List<int> items = ItemboxTable.Get(itemboxId).GetItems();
    //    AddItem(new HashSet<int>(items));
    //    return items;
    //}

}
