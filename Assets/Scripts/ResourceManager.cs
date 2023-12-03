using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
}

//��������ÿ�ֵ���Դ���
public class ResourceManager : MonoBehaviour
{
    public List<CharacterStatus> characters;
    public Dictionary<StatusType, int> resourceValues;
    public HashSet<int> items;

    //�غ����ݴ�, �غϽ���ͬ������Դֵ��
    public List<CharacterStatus> charactersTemp;
    public Dictionary<StatusType, int> resourceValuesTemp;
    public HashSet<int> itemsTemp;

    //������Դ����
    public Dictionary<int, Dictionary<StatusType, int>> resourceAlloc;

    public void Start()
    {
    }

    //��ʼ��������Դֵ
    public void Init()
    {

    }

    //������Դ
    public bool AllocResource(int characterId, StatusType type, int num)
    {
        if (!HasResource(type, num))
            return false;
        if (resourceAlloc.ContainsKey(characterId))
            resourceAlloc[characterId] = new();
        Dictionary<StatusType, int> characterAlloc = resourceAlloc[characterId];
        characterAlloc[type] = characterAlloc.GetValueOrDefault(type) + num;
        DeductResource(type, num);
        return true;
    }
    public bool UnallocResource(int characterId, StatusType type, int num)
    {
        if (resourceAlloc.ContainsKey(characterId))
            resourceAlloc[characterId] = new();
        Dictionary<StatusType, int> characterAlloc = resourceAlloc[characterId];
        if (characterAlloc.GetValueOrDefault(type) < num)
            return false;
        characterAlloc[type] = characterAlloc.GetValueOrDefault(type) - num;
        AddResource(type, num);
        return true;
    }

    //������Դ������
    public void SettleCurrentDay()
    {
        //������Դ��������ı��ɫ״̬
        foreach (var changeStatus in resourceAlloc)
        {
            UpdateCharacter(changeStatus.Key, changeStatus.Value);
        }
        resourceAlloc.Clear();
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
        return itemsTemp.Add(itemId);
    }

    public HashSet<int> AddItem(HashSet<int> addItems)
    {
        HashSet<int> result = new();
        foreach (var itemId in addItems)
        {
            if (itemsTemp.Add(itemId))
                result.Add(itemId);
        }
        return result;
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

    //��Դ�Ƿ��㹻
    public bool HasResource(StatusType type, int num)
    {
        return resourceValuesTemp.GetValueOrDefault(type) >= num;
    }

    public bool DeductResource(StatusType type, int num)
    {
        if (!HasResource(type, num))
            return false;
        resourceValuesTemp[type] = resourceValuesTemp.GetValueOrDefault(type) - num;
        return true;
    }

    public void AddResource(StatusType type, int num)
    {
        resourceValuesTemp[type] = resourceValuesTemp.GetValueOrDefault(type) + num;
    }

    //����Դ���ͬ������ǰ��Դ
    public void SyncResource()
    {
        characters = new(charactersTemp);
        resourceValues = new(resourceValuesTemp);
        items = new(itemsTemp);
    }

    //��ѯ�ӿ�
    public CharacterStatus GetCharacterStatus(int characterId)
    {
        return charactersTemp.Where(e => e.characterId == characterId).First();
    }
}
