using System.Collections;
using System.Collections.Generic;
using Table;
using Util;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

//�����жϺͿ��ƹ���̽������

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
    public int characterId;
    public int carryItem;
}

public class ExploreManager : MonoBehaviour
{
    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;


    //̽��״̬
    public ExploreState exploreState = ExploreState.Idle;
    public bool PrepareExplore { get; set; } = false;

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
        contentManager = GetComponent<ContentManager>();
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
        else if (exploreState == ExploreState.Exploring)
            CheckEndExplore();
    }

    //���̽����ʼ
    public void CheckStartExplore()
    {
        ExploreOption option = contentManager.GetExploreOption();
        if (option.characterId <= 0)
        {
            PrepareExplore = false;
            return;
        }

        exploreCharacter = option.characterId;
        carryItem = option.carryItem;
        //ɸ����������������̽����
        int groupId = selectedGroup;
        if (groupId == 0)
        {
            do
            {
                groupId = Config.GroupBase + Random.Range(1, Config.GroupNum + 1);
            }
            while (groupSet.Contains(groupId));
        }
        int exploreReturnDay = Random.Range(6, 9);//todo ����
        int exploreNum = Mathf.Max(groupExploreNum.GetValueOrDefault(groupId), 1);

        Debug.Log($"groupId: {groupId}");
        Debug.Log($"exploreNum: {exploreNum}");
        List<ExploreData> pool = ExploreTable.datas.Where(e =>
        {
            //����include��excludeɸѡ
            return e.groupId == groupId
            && e.exploreNum == exploreNum
            && processManager.CanMeetCondition(e.include)
            && !processManager.CanMeetCondition(e.exclude, false);
        }).ToList();
        exploreData = pool[Random.Range(0, pool.Count)];
        exploreState = ExploreState.Exploring;
        exploreDay = 0;

        // �����¼��������ʾ
        DisplayExploreStart(exploreData);
        //ShowExploreData(data, resultIndex);
    }

    //���̽������
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
        //todo ����̽�����, ʧ��, һ��, �ɹ�, ��ɹ�
        int resultIndex = 0;

        //���ݽ���ṩ����, ���ߺ���Դ�仯, ��ȡ��ͨ��misc������

        //����Я�������ṩ���⽱��, ���ߺ���Դ�仯
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

        //�����¼��������ʾ
        DisplayExploreEnd(exploreData, resultIndex);
        //ShowExploreData(data, resultIndex);

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

    //--------------��Ҳ���--------------
    ////ѡ��̽����ɫ
    //public bool SelectCharacter(int characterId)
    //{
    //    ////��ɫ״̬
    //    //if (resourceManager.GetCharacterStatus(characterId).liveStatus != LiveStatus.Normal)
    //    //    return false;
    //    ////̽����ȴ
    //    //if ((processManager.CurrentDay - lastExploreTime.GetValueOrDefault(characterId)) < Config.ExploreCoolDown)
    //    //    return false;
    //    exploreCharacter = characterId;
    //    DoExplore = true;
    //    return true;
    //}

    ////ȡ��ѡ��
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
        Debug.Log("start explore id: " + exploreData.id);
        string startExploreText = TextTable.GetText(exploreData.textContent);
        Debug.Log(string.Format("start explore text:{0}", startExploreText));
        contentManager.JournalText.AppendLine();
        contentManager.JournalText.AppendLine(startExploreText);
    }

    private void DisplayExploreEnd(ExploreData exploreData, int end)
    {
        Debug.Log("end explore id: " + exploreData.id);
        string endExploreText = TextTable.GetText(exploreData.endTextContent[end]);
        Debug.Log(string.Format("end explore text:{0}", endExploreText));
        contentManager.JournalText.AppendLine();
        contentManager.JournalText.AppendLine(endExploreText);
    }

    //չʾ
    private void ShowExploreData(ExploreData storyData, int end = -1)
    {
        Debug.Log("Show explore data id: " + storyData.id);
        if (end >= 0)
            Debug.Log(string.Format("end{0}, endText:{1}", end, storyData.endTextContent[end]));
    }

}
