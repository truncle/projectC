using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiaryPanel : MonoBehaviour
{
    GameManager gameManager;
    GameObject maingameManagers;
    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;
    private ExploreManager exploreManager;

    private int currPageIndex = 0;
    private List<GameObject> diaryPages = new();
    private List<GameObject> enablePages;

    private GameObject prevPageBtn;
    private GameObject closePageBtn;
    private GameObject openPageBtn;

    private void Start()
    {
        gameManager = GameManager.Instance;
        maingameManagers = GameObject.Find("MaingameManagers");
        prevPageBtn = GameObject.Find("PrevPageBtn");
        closePageBtn = gameObject.transform.Find("ClosePanelBtn").gameObject;
        openPageBtn = transform.parent.Find("OpenPanelBtn").gameObject;
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        processManager = maingameManagers.GetComponent<ProcessManager>();
        contentManager = maingameManagers.GetComponent<ContentManager>();
        exploreManager = maingameManagers.GetComponent<ExploreManager>();

        prevPageBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InitRoundPages()
    {
        MiscData exploreLimit1 = MiscTable.Get("explore_limit_1");

        if (diaryPages.Count <= 0)
        {
            for (int i = 0; i < 4; i++)
            {
                diaryPages.Add(transform.GetChild(i).gameObject);
            }
        }
        if (processManager != null && exploreManager != null
            && (processManager.CurrentDay == Convert.ToInt32(exploreLimit1.para4) || exploreManager.exploreState == ExploreState.Exploring))
        {
            enablePages = diaryPages.Where(p => p.name != "Explory").ToList();
        }
        else
        {
            enablePages = new(diaryPages);
        }
    }

    public void NextPage()
    {
        if (currPageIndex < enablePages.Count - 1)
        {
            GameObject page = enablePages[currPageIndex];
            currPageIndex = currPageIndex + 1;
            GameObject nextPage = enablePages[currPageIndex];
            page.SetActive(false);
            nextPage.SetActive(true);
            if (nextPage.name == "Explory")
            {
                if (exploreManager.exploreState == ExploreState.Idle)
                {
                    nextPage.transform.Find("PreparePage").gameObject.SetActive(true);
                    nextPage.transform.Find("StartingPage").gameObject.SetActive(false);
                }
                else if (exploreManager.exploreState == ExploreState.Start)
                {
                    nextPage.transform.Find("PreparePage").gameObject.SetActive(false);
                    nextPage.transform.Find("StartingPage").gameObject.SetActive(true);
                }
            }
        }
        else
        {
            GameObject page = enablePages[currPageIndex];
            page.SetActive(false);
            processManager.EndCurrentDay();
            processManager.InitCurrentDay();
            currPageIndex = 0;
            GameObject nextPage = enablePages[currPageIndex];
            nextPage.SetActive(true);
            ClosePanel(openPageBtn);
        }

        if (currPageIndex == 0)
            prevPageBtn.SetActive(false);
        else
            prevPageBtn.SetActive(true);
    }

    public void PrevPage()
    {
        if (currPageIndex <= 0)
            return;
        GameObject page = enablePages[currPageIndex];
        currPageIndex = currPageIndex - 1;
        GameObject prevPage = enablePages[currPageIndex];
        page.SetActive(false);
        prevPage.SetActive(true);
        if (prevPage.name == "Explory")
        {
            if (exploreManager.exploreState == ExploreState.Idle)
            {
                prevPage.transform.Find("PreparePage").gameObject.SetActive(true);
                prevPage.transform.Find("StartingPage").gameObject.SetActive(false);
            }
            else if (exploreManager.exploreState == ExploreState.Start)
            {
                prevPage.transform.Find("PreparePage").gameObject.SetActive(false);
                prevPage.transform.Find("StartingPage").gameObject.SetActive(true);
            }
        }

        if (currPageIndex == 0)
            prevPageBtn.SetActive(false);
        else
            prevPageBtn.SetActive(true);
    }

    public void OpenPanel(GameObject go)
    {
        gameObject.SetActive(true);
        go.SetActive(false);
    }

    public void ClosePanel(GameObject go)
    {
        go.SetActive(true);
        gameObject.SetActive(false);
    }

    //资源分配逻辑还需要确认一下
    public void DoAssignResource()
    {

    }

    public void AssignResource(int characterId, int resourceType)
    {

    }

    public void AssignAllResource()
    {

    }

    public void PrepareExplore(Toggle toggle)
    {
        //exploreManager.PrepareExplore = !exploreManager.PrepareExplore;
        exploreManager.PrepareExplore = toggle.isOn;
        if (exploreManager.PrepareExplore)
        {
            toggle.GetComponentInChildren<Text>().text = "Prepared";
        }
        else
        {
            toggle.GetComponentInChildren<Text>().text = "Prepare";
        }
    }

    //事件选择
    public void SelectStory(int option)
    {

    }
}
