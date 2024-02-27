using System.Collections;
using System.Collections.Generic;
using Table;
using Util;
using UnityEngine;
using System.Linq;

//�����жϺͿ��ƹ���̽������

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

    //̽��״̬
    public ExploreState exploreState = ExploreState.Idle;
    public bool PrepareExplore { get; set; } = false;
    public bool DoExplore { get; private set; }
    //��ѡ�������ѡ�е����
    public HashSet<int> groupSet = new();
    public int selectedGroup = 0;

    //̽������Ϣ
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

    //���㵱���̽��ϵͳ
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

    //���̽����ʼ
    public void CheckStartExplore()
    {
        if (!DoExplore)
        {
            //ûѡ�˵Ļ�����һ�µ���
            if (carryItem > 0)
                resourceManager.AddItem(carryItem);
            exploreState = ExploreState.Idle;
            return;
        }

        //ɸ����������������̽����
        int groupId = selectedGroup;
        if (groupId == 0)
        {
            while (groupSet.Contains(groupId))
            {
                groupId = Config.GroupBase + Random.Range(1, Config.GroupNum + 1);
            }
        }
        int exploreReturnDay = Random.Range(6, 9);//todo ����
        int exploreNum = groupExploreNum.GetValueOrDefault(groupId);
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //����include��excludeɸѡ
            return e.groupId == groupId
            && e.exploreNum == exploreNum
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        exploreData = pool[Random.Range(0, pool.Count)];

        DoExplore = false;

        //todo �����¼��������ʾ
        //ShowExploreData(data, resultIndex);
    }

    //���̽������
    public void CheckEndExplore()
    {
        if (exploreDay < exploreData.returnDays)
            return;
        //todo ����̽�����, ʧ��, һ��, �ɹ�, ��ɹ�
        int resultIndex = 0;

        //���ݽ���ṩ����, ���ߺ���Դ�仯

        //����Я�������ṩ����, ���ߺ���Դ�仯
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
        //todo �����¼��������ʾ
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

    //--------------��Ҳ���--------------
    //ѡ��̽����ɫ
    public bool SelectCharacter(int characterId)
    {
        ////��ɫ״̬
        //if (resourceManager.GetCharacterStatus(characterId).liveStatus != LiveStatus.Normal)
        //    return false;
        ////̽����ȴ
        //if ((processManager.CurrentDay - lastExploreTime.GetValueOrDefault(characterId)) < Config.ExploreCoolDown)
        //    return false;
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

    //չʾ
    private void ShowExploreData(ExploreData storyData, int end = -1)
    {
        Debug.Log("Show explore data id: " + storyData.id);
        if (end >= 0)
            Debug.Log(string.Format("end{0}, endText:{1}", end, storyData.endTextContent[end]));
    }

}
