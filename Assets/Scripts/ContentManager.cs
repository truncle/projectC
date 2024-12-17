using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using Table;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Util;

//将每轮处理结果汇总成文本展示
public class ContentManager : MonoBehaviour
{
    private GameObject MaingameUI;
    public DiaryPanel DiaryPanel;

    public JournalPage JournalPage;
    public AssignmentPage AssignmentPage;
    public ExploryPage ExploryPage;
    public StorylinePage StorylinePage;

    public ResourceManager resourceManager;
    public ProcessManager processManager;
    public ExploreManager exploreManager;
    public StorylineManager storylineManager;

    private JournalPageContent journalContent = new();
    private AssignmentPageContent assignmentContent = new();
    private ExploryPageContent exploryContent = new();
    private StorylinePageContent storylineContent = new();

    private void Start()
    {
        MaingameUI = GameObject.Find("MaingameUI");

        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
        exploreManager = GetComponent<ExploreManager>();
        storylineManager = GetComponent<StorylineManager>();

        DiaryPanel = MaingameUI.transform.Find("DiaryPanel").GetComponent<DiaryPanel>();
        JournalPage = DiaryPanel.transform.Find("Journal").GetComponent<JournalPage>();
        AssignmentPage = DiaryPanel.transform.Find("Assignment").GetComponent<AssignmentPage>();
        ExploryPage = DiaryPanel.transform.Find("Explory").GetComponent<ExploryPage>();
        StorylinePage = DiaryPanel.transform.Find("Storyline").GetComponent<StorylinePage>();

    }

    public void Clear()
    {
        journalContent = new();
        assignmentContent = new();
        exploryContent = new();
        storylineContent = new();
    }

    public void Init()
    {
        AssignmentPage.Init();
        ExploryPage.Init();
        StorylinePage.Init();
    }

    //把内容同步到表现上
    public void Sync()
    {
        DiaryPanel.InitRoundPages();

        TextMeshProUGUI dayTitle = DiaryPanel.transform.Find("DayTitle").GetComponent<TextMeshProUGUI>();
        dayTitle.text = $"第{processManager.CurrentDay}天";

        var JournalTextUI = JournalPage.transform.Find("TextContent").GetComponent<TextMeshProUGUI>();
        JournalTextUI.text = BuildJournalPageContent();

        //更新分配页面角色状态
        AssignmentPage.Sync();

        //更新探索页面角色状态和描述
        ExploryPage.Sync();

        //更新探索页面角色状态和描述
        StorylinePage.Sync();

        var StorylineContentUI = StorylinePage.transform.Find("TextContent").GetComponent<TextMeshProUGUI>();
        StorylineContentUI.text = TextTable.GetText(storylineManager.CurrentData.textContent);

        CheckStorylineSelectGroup(storylineManager.CurrentData);
    }

    #region 用来收集其它模块产生的结果
    public void PushStorylineEnd(EventStoryData storyData, int end)
    {
        journalContent.LastDayStoryData = storyData;
        journalContent.StoryEnd = end;
    }

    public void PushExploreStart(ExploreData exploreData)
    {
        journalContent.ExploreStartData = exploreData;
    }

    public void PushExploreEnd(ExploreData exploreData, int end)
    {
        journalContent.ExploreFinishData = exploreData;
        journalContent.ExploreEnd = end;
    }
    #endregion

    #region 用来构建每个页面需要的参数

    //todo 暂时使用, 后续页面需要进行结构调整
    public string BuildJournalPageContent()
    {
        StringBuilder sb = new();
        if (journalContent.LastDayStoryData.HasValue)
            sb.AppendLine('\n' + journalContent.GetLastStoryEndText());
        if (journalContent.ExploreStartData.HasValue)
            sb.AppendLine('\n' + journalContent.GetExploreStartText());
        if (journalContent.ExploreFinishData.HasValue)
            sb.AppendLine('\n' + journalContent.GetExploreFinishText());
        return sb.ToString();
    }

    public void BuildAssignmentPageContent()
    {
        AssignmentPageContent content = new();
        content.WaterNum = resourceManager.GetResourceNum(ResourceType.Water);
        content.FoodNum = resourceManager.GetResourceNum(ResourceType.Food);
        content.characters = resourceManager.GetCharacters();
        assignmentContent = content;
    }

    public void BuildExplorePageContent()
    {
        ExploryPageContent content = new();
        content.WaterNum = resourceManager.GetResourceNum(ResourceType.Water);
        content.FoodNum = resourceManager.GetResourceNum(ResourceType.Food);
        content.characters = resourceManager.GetCharacters();
        exploryContent = content;
    }
    #endregion

    public int GetStorylineOption()
    {
        return StorylinePage.GetStorylineOption();
    }

    //开始探索的选择参数
    public ExploreOption GetExploreOption()
    {
        return ExploryPage.GetExploreOption();
    }

    //检查需要设置的选择组
    public void CheckStorylineSelectGroup(EventStoryData storyData)
    {
        if (StorylinePage.SelectGroup != null)
        {
            StorylinePage.SelectGroup.gameObject.SetActive(false);
        }

        if (storyData.eventType == (int)EventStoryType.Select)
        {
            StorylinePage.SelectGroup = StorylinePage.BinarySelectGroup;
        }
        else if (storyData.eventType == (int)EventStoryType.ItemSelect)
        {
            StorylinePage.SelectGroup = StorylinePage.ItemGroup;
            //for (int i = storyData.provideItem.Count; i < StorylinePage.SelectGroup.transform.childCount; i++)
            //    StorylinePage.SelectGroup.transform.GetChild(i).gameObject.SetActive(false);
            for (int i = 0; i < storyData.provideItem.Count; i++)
            {
                GameObject option = StorylinePage.SelectGroup.transform.GetChild(i).gameObject;
                option.SetActive(true);
                //todo 更换道具图片
                option.GetComponentInChildren<Image>().sprite = SpriteLoader.GetItemSprite(storyData.provideItem[i]);
            }
        }
        else if (storyData.eventType == (int)EventStoryType.GroupSelect)
        {
            StorylinePage.SelectGroup = StorylinePage.MultiSelectGroup;
            List<int> groupList = exploreManager.checkedGroupSet.ToList();
            groupList.Sort();
            for (int i = 0; i < groupList.Count; i++)
            {
                StorylinePage.SelectGroup.transform.GetChild(i).gameObject.SetActive(true);
                //todo 更换组id对应显示内容
            }
        }
        else
        {
            StorylinePage.SelectGroup = null;
        }

        if (StorylinePage.SelectGroup != null)
            StorylinePage.SelectGroup.gameObject.SetActive(true);
    }
}

#region 把单一页面上需要的数据都存在一个结构里, 方便管理以及存档。Manager内已有的数据不包括在内
public class JournalPageContent
{
    public EventStoryData? LastDayStoryData;
    public ExploreData? ExploreStartData;
    public ExploreData? ExploreFinishData;
    public int StoryEnd;
    public int ExploreEnd;

    public string GetLastStoryEndText()
    {
        string endText = string.Empty;
        if (LastDayStoryData.HasValue)
            endText = TextTable.GetText(LastDayStoryData.Value.endTextContent[StoryEnd]);
        return endText;
    }

    public string GetExploreStartText()
    {
        string startText = string.Empty;
        if (ExploreStartData.HasValue)
            startText = TextTable.GetText(ExploreStartData.Value.textContent);
        return startText;
    }

    public string GetExploreFinishText()
    {
        string endText = string.Empty;
        if (ExploreFinishData.HasValue)
            endText = TextTable.GetText(ExploreFinishData.Value.endTextContent[ExploreEnd]);
        return endText;
    }
}

public class AssignmentPageContent
{
    public List<CharacterStatus> characters = new();
    public int WaterNum;
    public int FoodNum;
}

public class ExploryPageContent
{
    public List<CharacterStatus> characters = new();
    public int WaterNum;
    public int FoodNum;
}

public class StorylinePageContent
{

}
#endregion
