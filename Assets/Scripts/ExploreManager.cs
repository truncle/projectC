using System.Collections;
using System.Collections.Generic;
using Table;
using Util;
using UnityEngine;
using System.Linq;

//�����жϺͿ��ƹ���̽������

public class ExploreManager : MonoBehaviour
{
    public int exploreCharacter = 0;
    public List<int> itemSet = new();

    public Dictionary<int, int> lastExploreTime = new();

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
            //ûѡ�˵Ļ�����һ�µ���
            foreach (var item in itemSet)
            {
                resourceManager.AddItem(item, 1);
            }
            return;
        }

        //ɸ����������������̽����
        int groupId = Config.GroupBase + Random.Range(1, Config.GroupNum);
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //����include��excludeɸѡ
            return e.groupId == groupId
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        var data = pool[Random.Range(0, pool.Count)];

    }

    //ѡ��̽����ɫ
    public bool SelectCharacter(int characterId)
    {
        //��ɫ״̬
        if (!resourceManager.GetCharacterStatus(characterId).isAlive)
            return false;
        //̽����ȴ
        if ((processManager.CurrentDay - lastExploreTime.GetValueOrDefault(characterId)) < Config.ExploreCoolDown)
            return false;
        exploreCharacter = characterId;
        DoExplore = true;
        return true;
    }

    //ȡ��ѡ��
    public void UnselectCharacter()
    {
        exploreCharacter = 0;
        DoExplore = false;
    }

    public bool SelectItem(int itemId)
    {
        bool result = resourceManager.DeductItem(itemId, 1);
        if (result)
            itemSet.Add(itemId);
        return result;
    }

    public void UnselectItem(int itemId)
    {
        if (itemSet.Remove(itemId))
            resourceManager.AddItem(itemId, 1);
    }

}
