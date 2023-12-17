using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//将每轮处理结果汇总成文本展示
public class ContentManager : MonoBehaviour
{
    private TextMeshProUGUI JournalTextUI;

    private string journalText = "JournalText";

    private void Start()
    {
        JournalTextUI = GameObject.Find("MaingameUI").transform.Find("DiaryPanel/Journal/TextContent").GetComponent<TextMeshProUGUI>();
    }

    public void Sync()
    {
        JournalTextUI.text = journalText;
    }

    public void PushContent(string content)
    {
        journalText = content;
    }

    private void DisplayContent()
    {

    }

    private void GetContent()
    {

    }
}
