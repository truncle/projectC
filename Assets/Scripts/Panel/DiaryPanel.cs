using System.Collections;
using System.Collections.Generic;
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

    private int currPage = 0;
    private List<GameObject> dairyPages;

    private GameObject prevPageBtn;



    private void Start()
    {
        gameManager = GameManager.Instance;
        maingameManagers = GameObject.Find("MaingameManagers");
        prevPageBtn = GameObject.Find("PrevPageBtn");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        processManager = maingameManagers.GetComponent<ProcessManager>();
        contentManager = maingameManagers.GetComponent<ContentManager>();
        exploreManager = maingameManagers.GetComponent<ExploreManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (currPage == 0)
            prevPageBtn.SetActive(false);
        else
            prevPageBtn.SetActive(true);

    }

    public void NextPage()
    {
        if (currPage <= 2)
        {
            GameObject page = transform.GetChild(currPage).gameObject;
            currPage = currPage + 1;
            GameObject nextPage = transform.GetChild(currPage).gameObject;
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
                else
                {
                    NextPage();
                }
            }
        }
        else
        {
            processManager.EndCurrentDay();
            processManager.InitCurrentDay();
            GameObject page = transform.GetChild(currPage).gameObject;
            currPage = 0;
            GameObject nextPage = transform.GetChild(currPage).gameObject;
            page.SetActive(false);
            nextPage.SetActive(true);
        }
    }

    public void PrevPage()
    {
        if (currPage <= 0)
            return;
        GameObject page = transform.GetChild(currPage).gameObject;
        currPage = currPage - 1;
        GameObject prevPage = transform.GetChild(currPage).gameObject;
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
            else
            {
                PrevPage();
            }
        }
    }

    public void OpenPanel(GameObject go)
    {
        go.SetActive(false);
        gameObject.SetActive(true);
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
