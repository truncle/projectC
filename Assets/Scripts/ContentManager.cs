using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public void Sync()
    {
        var JournalTextUI = JournalPage.transform.Find("TextContent").GetComponent<TextMeshProUGUI>();
        JournalTextUI.text = JournalText.ToString();

        //todo 更新分配页面角色状态

        //todo 更新探索页面角色状态和描述


        var StorylineContentUI = StorylinePage.transform.Find("TextContent").GetComponent<TextMeshProUGUI>();
        StorylineContentUI.text = StorylineContent.ToString();
    }

    public ExploreOption GetExploreOption()
    {
        return ExploryPage.GetExploreOption();
    }

    public void AddSelectGroup()
    {

    }
}
