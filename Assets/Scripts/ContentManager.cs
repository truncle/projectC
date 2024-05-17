using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Table;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//将每轮处理结果汇总成文本展示
public class ContentManager : MonoBehaviour
{
    private GameObject MaingameUI;
    private DiaryPanel DiaryPanel;

    private JournalPage JournalPage;
    private AssignmentPage AssignmentPage;
    private ExploryPage ExploryPage;
    private StorylinePage StorylinePage;

    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ExploreManager exploreManager;
    private StorylineManager storylineManager;

    //这里包含界面显示需要的所有信息, 需要打包进存档
    public StringBuilder JournalText { get; set; } = new();
    public StringBuilder ExploreText { get; set; } = new();
    public StringBuilder StorylineContent { get; set; } = new();

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
        JournalText.Clear();
        ExploreText.Clear();
        StorylineContent.Clear();
    }

    public void Init()
    {
        AssignmentPage.Init();
        ExploryPage.Init();
        StorylinePage.Init();
    }

    //把内容同步到表现上
    public void Sync(bool isInit = false)
    {
        if (isInit)
        {
            DiaryPanel.InitRoundPages();
        }

        TextMeshProUGUI dayTitle = DiaryPanel.transform.Find("DayTitle").GetComponent<TextMeshProUGUI>();
        dayTitle.text = $"第{processManager.CurrentDay}天";

        var JournalTextUI = JournalPage.transform.Find("TextContent").GetComponent<TextMeshProUGUI>();
        JournalTextUI.text = JournalText.ToString();

        //更新分配页面角色状态
        AssignmentPage.Sync();

        //更新探索页面角色状态和描述
        ExploryPage.Sync();

        var StorylineContentUI = StorylinePage.transform.Find("TextContent").GetComponent<TextMeshProUGUI>();
        StorylineContentUI.text = StorylineContent.ToString();

        if (isInit)
            CheckStorylineSelectGroup(storylineManager.CurrentData);
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
            StorylinePage.SelectGroup.gameObject.SetActive(false);

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
                StorylinePage.SelectGroup.transform.GetChild(i).gameObject.SetActive(true);
                //todo 更换道具图片
            }
        }
        else if (storyData.eventType == (int)EventStoryType.GroupSelect)
        {
            StorylinePage.SelectGroup = StorylinePage.MultiSelectGroup;
            List<int> groupList = exploreManager.groupSet.ToList();
            groupList.Sort();
            for (int i = 0; i < groupList.Count; i++)
            {
                StorylinePage.SelectGroup.transform.GetChild(i).gameObject.SetActive(true);
                //todo 更换道具图片, 组id对应显示内容
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
