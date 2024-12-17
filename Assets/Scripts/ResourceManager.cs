using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table;
using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType
{
    Water = 1, Food = 2,
}

public enum StatusType
{
    Hungry = 1, Thirsty = 2,
}

public enum LiveStatus
{
    Normal, Dead, Crazy
}

public class CharacterStatus
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

    public CharacterStatus Clone()
    {
        CharacterStatus data = new()
        {
            characterId = characterId,
            liveStatus = liveStatus,
            statusValues = new(statusValues)
        };
        return data;
    }

    public override string ToString()
    {
        return $"Id:{characterId} H:{GetValue(StatusType.Hungry)} T:{GetValue(StatusType.Thirsty)}";
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
    public Dictionary<int, Dictionary<ResourceType, bool>> resourceAlloc;

    public void Start()
    {
    }

    //初始化基本资源值
    public void Init()
    {
        CharacterConstTable.Init();
        characters = new()
        {
            //new() { characterId = 1, statusValues = new(){
            //    {StatusType.Hungry, 12},
            //    {StatusType.Thirsty, 12}
            //}},
            //new() { characterId = 2, statusValues = new(){
            //    {StatusType.Hungry, 12},
            //    {StatusType.Thirsty, 12}
            //}},
            //new() { characterId = 3, statusValues = new(){
            //    {StatusType.Hungry, 12},
            //    {StatusType.Thirsty, 12}
            //}},
            //new() { characterId = 4, statusValues = new(){
            //    {StatusType.Hungry, 12},
            //    {StatusType.Thirsty, 12}
            //}},
        };
        charactersTemp = new();
        foreach (var characterConst in CharacterConstTable.datas)
        {
            CharacterStatus cs = new()
            {
                characterId = characterConst.id,
                statusValues = new(),
            };
            CharacterStatus csTemp = new()
            {
                characterId = characterConst.id,
                statusValues = new(),
            };
            foreach (var data in characterConst.statesInitial)
            {
                cs.statusValues[(StatusType)data[0]] = data[1];
                csTemp.statusValues[(StatusType)data[0]] = data[1];
            }
            characters.Add(cs);
            charactersTemp.Add(csTemp);
        }

        resourceValues = new()
        {
            { ResourceType.Water, 5 },
            { ResourceType.Food, 5 },
        };
        resourceValuesTemp = new(resourceValues);

        items = new()
        {
            1,3,5
        };
        itemsTemp = new(items);

        resourceAlloc = new();
    }

    //分配资源
    public bool AllocResource(int characterId, ResourceType type)
    {
        if (!HasResource(type, 1))
            return false;
        if (!resourceAlloc.ContainsKey(characterId))
            resourceAlloc[characterId] = new();
        Dictionary<ResourceType, bool> characterAlloc = resourceAlloc[characterId];
        characterAlloc[type] = true;
        DeductResource(type, 1);
        return true;
    }
    public bool UnallocResource(int characterId, ResourceType type)
    {
        if (!resourceAlloc.ContainsKey(characterId))
            resourceAlloc[characterId] = new();
        Dictionary<ResourceType, bool> characterAlloc = resourceAlloc[characterId];
        if (characterAlloc.GetValueOrDefault(type) == false)
            return false;
        characterAlloc[type] = false;
        AddResource(type, 1);
        return true;
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

    public List<CharacterStatus> GetCharacters()
    {
        List<CharacterStatus> datas = new();
        foreach (var character in characters)
        {
            datas.Add(character.Clone());
        }
        return datas;
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

    public bool UpdateCharacter(int characterId, StatusType statusType, int value)
    {
        return UpdateCharacter(new()
        {
            characterId = characterId,
            statusValues = new() { { statusType, value } },
        });
    }

    public bool UpdateCharacter(int characterId, LiveStatus status)
    {
        CharacterStatus characterStatus = charactersTemp.Where(e => e.characterId == characterId).First();
        if (characterStatus.liveStatus == LiveStatus.Dead)
            return false;
        characterStatus.liveStatus = status;
        return true;
    }



    public int GetResourceNum(ResourceType type)
    {
        return resourceValues.GetValueOrDefault(type);
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
            if (res.Count == 2)
                DeductResource(res[0], res[1]);
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
            if (res.Count == 2)
                AddResource(res[0], res[1]);
        }
    }

    //每天结束时结算资源消耗
    public void SettleDayResource()
    {
        MiscData resConsume = MiscTable.Get("res_consume");
        for (int i = 0; i < charactersTemp.Count; i++)
        {
            var character = charactersTemp[i];
            var characterConst = CharacterConstTable.Get(character.characterId);
            int hungryLimit = characterConst.statesLimit[0][1];
            int thirstyLimit = characterConst.statesLimit[1][1];
            if (character.liveStatus == LiveStatus.Dead)
                continue;
            var allocateResource = resourceAlloc.GetValueOrDefault(character.characterId);
            if (allocateResource != null)
            {
                if (allocateResource.GetValueOrDefault(ResourceType.Water))
                    character.statusValues[StatusType.Thirsty] = thirstyLimit;
                if (allocateResource.GetValueOrDefault(ResourceType.Food))
                    character.statusValues[StatusType.Hungry] = hungryLimit;
            }
            character.statusValues[StatusType.Thirsty] += int.Parse(resConsume.para1);
            character.statusValues[StatusType.Hungry] += int.Parse(resConsume.para2);
            if (character.statusValues[StatusType.Thirsty] <= 0)
            {
                character.liveStatus = LiveStatus.Dead;
            }
            if (character.statusValues[StatusType.Hungry] <= 0)
            {
                character.liveStatus = LiveStatus.Dead;
            }
        }
        SyncResource();
    }

    //将资源变更同步到当前资源
    public void SyncResource()
    {
        resourceAlloc.Clear();
        characters.Clear();
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
