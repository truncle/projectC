using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public struct CharacterStatus
{
    public int characterId;
    public int hungry;
    public int mind;
    public int thirsty;
    public bool isAlive;//�Ƿ����
}

//��������ÿ�ֵ���Դ���
public class ResourceManager : MonoBehaviour
{
    public List<CharacterStatus> characters;
    public Dictionary<int, int> items;

    //�غ����ݴ�, �غϽ���ͬ������Դֵ��
    public List<CharacterStatus> charactersTemp;
    public Dictionary<int, int> itemsTemp;

    //������Դ����
    public Dictionary<int, Dictionary<int, int>> resourceAlloc;

    public void Start()
    {
    }

    //��ʼ��������Դֵ
    public void Init()
    {

    }

    //������Դ
    public bool AllocResource(int characterId, int itemId, int itemNum)
    {
        if (!DeductItem(itemId, itemNum))
            return false;
        Dictionary<int, int> itemTable = resourceAlloc.GetValueOrDefault(characterId, new Dictionary<int, int>());
        itemTable.Add(itemId, itemTable.GetValueOrDefault(itemId) + itemNum);
        return true;
    }
    public bool UnallocResource(int characterId, int itemId, int itemNum)
    {
        Dictionary<int, int> itemTable = resourceAlloc.GetValueOrDefault(characterId, new Dictionary<int, int>());
        if (itemTable.GetValueOrDefault(itemId) < itemNum)
            return false;
        itemTable.Add(itemId, itemTable.GetValueOrDefault(itemId) - itemNum);
        AddItem(itemId, itemNum);
        return true;
    }

    //������Դ������
    public void SettleCurrentDay()
    {
        //todo ������Դ��������ı��ɫ״̬
        resourceAlloc.Clear();
    }


    //==================== ���÷��� =======================

    //�����Ƿ��㹻
    public bool HasEnoughItem(int itemId, int num)
    {
        return itemsTemp.GetValueOrDefault(itemId) >= num;
    }

    //�۳�����
    public bool DeductItem(Dictionary<int, int> updateList)
    {
        foreach (var itemChange in updateList)
        {
            if (!itemsTemp.ContainsKey(itemChange.Key) || itemsTemp[itemChange.Key] - itemChange.Value < 0)
                return false;
        }
        foreach (var itemChange in updateList)
        {
            itemsTemp.Add(itemChange.Key, itemsTemp[itemChange.Key] - itemChange.Value);
        }
        return true;
    }

    //���ӵ���
    public bool AddItem(Dictionary<int, int> updateList)
    {
        foreach (var itemChange in updateList)
        {
            itemsTemp.Add(itemChange.Key, itemsTemp.GetValueOrDefault(itemChange.Key) + itemChange.Value);
        }
        return true;
    }

    public bool DeductItem(int itemId, int num)
    {
        if (itemsTemp.GetValueOrDefault(itemId) >= num)
        {
            itemsTemp.Add(itemId, itemsTemp[itemId] - num);
            return true;
        }
        else return false;
    }

    public bool AddItem(int itemId, int num)
    {
        itemsTemp.Add(itemId, itemsTemp.GetValueOrDefault(itemId) + num);
        return true;
    }

    //�޸Ľ�ɫ״̬
    public bool UpdateCharacter(int characterId, CharacterStatus changeStatus)
    {
        CharacterStatus characterStatus = charactersTemp.Where(e => e.characterId == characterId).First();
        if (changeStatus.hungry + characterStatus.hungry < 0)
            return false;
        if (changeStatus.mind + characterStatus.mind < 0)
            return false;
        if (changeStatus.thirsty + characterStatus.thirsty < 0)
            return false;

        changeStatus.hungry += characterStatus.hungry;
        changeStatus.mind += characterStatus.mind;
        changeStatus.thirsty += characterStatus.thirsty;

        return true;
    }

    //����Դ���ͬ������ǰ��Դ
    public void SyncResource()
    {
        characters = new(charactersTemp);
        items = new(itemsTemp);
    }

    //��ѯ�ӿ�
    public CharacterStatus GetCharacterStatus(int characterId)
    {
        return charactersTemp.Where(e => e.characterId == characterId).First();
    }

    public int GetItemNum(int itemId)
    {
        return itemsTemp.GetValueOrDefault(itemId);
    }
}
