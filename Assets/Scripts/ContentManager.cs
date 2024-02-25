using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//将每轮处理结果汇总成文本展示
public class ContentManager : MonoBehaviour
{
    private TextMeshProUGUI JournalTextUI;

    //这里包含界面显示需要的所有信息, 需要打包进存档
    private string journalText = "JournalText";

    private string storylineText = "stroylineText";

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
