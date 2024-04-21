using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//将每轮处理结果汇总成文本展示
public class ContentManager : MonoBehaviour
{
    private TextMeshProUGUI JournalTextUI;
    private TextMeshProUGUI ExploreTextUI;
    private TextMeshProUGUI StorylineContentUI;

    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ExploreManager exploreManager;

    //这里包含界面显示需要的所有信息, 需要打包进存档
    public string JournalText { get; set; } = "JournalText";
    public string ExploreText { get; set; } = "ExploreText";
    public string StorylineContent { get; set; } = "StroylineText";

    private void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        processManager = GetComponent<ProcessManager>();
        exploreManager = GetComponent<ExploreManager>();
        JournalTextUI = GameObject.Find("MaingameUI").transform.Find("DiaryPanel/Journal/TextContent").GetComponent<TextMeshProUGUI>();
        ExploreTextUI = GameObject.Find("MaingameUI").transform.Find("DiaryPanel/Explory/TextContent").GetComponent<TextMeshProUGUI>();
        StorylineContentUI = GameObject.Find("MaingameUI").transform.Find("DiaryPanel/Storyline/Display").GetComponent<TextMeshProUGUI>();
    }

    public void Sync()
    {
        //StringBuilder sb = new();
        //sb.AppendLine("Current Day: " + processManager.CurrentDay);
        //sb.AppendLine(journalText);
        //sb.AppendLine($"water:{resourceManager.GetResourceNum(ResourceType.Water)} food:{resourceManager.GetResourceNum(ResourceType.Food)}");
        //sb.AppendLine("Explore");
        //sb.AppendLine($"id:{exploreManager.exploreData.id} chara:{exploreManager.exploreCharacter} day:{exploreManager.exploreDay}");
        //sb.AppendLine("Characters");
        //foreach (var character in resourceManager.characters)
        //{
        //    sb.AppendLine(character.ToString());
        //}
        //JournalTextUI.text = sb.ToString();
        JournalTextUI.text = JournalText;
        ExploreTextUI.text = ExploreText;
        StorylineContentUI.text = StorylineContent;

    }

    public void PushContent(string content)
    {
        JournalText = content;
    }
    public void AddContent(string content)
    {
        JournalText += "\n";
        JournalText += content;
    }

    private void DisplayContent()
    {

    }

    private void GetContent()
    {

    }
}
