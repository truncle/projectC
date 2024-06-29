using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table;
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
    public LiveStatus liveStatus;//���״̬

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

    public override string ToString()
    {
        return $"Id:{characterId} H:{GetValue(StatusType.Hungry)} T:{GetValue(StatusType.Thirsty)}";
    }
}

//��������ÿ�ֵ���Դ���
public class ResourceManager : MonoBehaviour
{
    public List<CharacterStatus> characters;
    public Dictionary<ResourceType, int> resourceValues;
    public HashSet<int> items;

    //�غ����ݴ�, �غϽ���ͬ������Դֵ��
    public List<CharacterStatus> charactersTemp;
    public Dictionary<ResourceType, int> resourceValuesTemp;
    public HashSet<int> itemsTemp;

    //������Դ����
    public Dictionary<int, Dictionary<ResourceType, bool>> resourceAlloc;

    public void Start()
    {
    }

    //��ʼ��������Դֵ
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

    //������Դ
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

    //==================== ���÷��� =======================

    //�����Ƿ��㹻
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

    //�۳�����
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

    //���ӵ���
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

    //�޸Ľ�ɫ״̬, ����Ҫ�������״̬ < 0ʱ�����
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

    //��Դ�Ƿ��㹻
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

    //ÿ�����ʱ������Դ����
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

    //����Դ���ͬ������ǰ��Դ
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

    //��ѯ�ӿ�
    public CharacterStatus GetCharacterStatus(int characterId)
    {
        return charactersTemp.Where(e => e.characterId == characterId).First();
    }

    ////��ȡ�������еĵ���
    //public List<int> GetItembox(int itemboxId)
    //{
    //    List<int> items = ItemboxTable.Get(itemboxId).GetItems();
    //    AddItem(new HashSet<int>(items));
    //    return items;
    //}

}
