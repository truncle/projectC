using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DairyPanel : MonoBehaviour
{
    GameManager gameManager;
    GameObject maingameManagers;
    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;
    private ExploreManager exploreManager;

    private int currPage = 0;
    private List<GameObject> dairyPages;


    private void Start()
    {
        gameManager = GameManager.Instance;
        maingameManagers = GameObject.Find("MaingameManagers");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        processManager = maingameManagers.GetComponent<ProcessManager>();
        contentManager = maingameManagers.GetComponent<ContentManager>();
        exploreManager = maingameManagers.GetComponent<ExploreManager>();
    }


    // Update is called once per frame
    void Update()
    {

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

    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
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

    //事件选择
    public void SelectStory(int option)
    {

    }
}
