using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class GameInterface : MonoBehaviour
{
    GameManager gameManager;
    GameObject maingameManagers;
    private ResourceManager resourceManager;
    private ProcessManager processManager;
    private ContentManager contentManager;
    private ExploreManager exploreManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        maingameManagers = GameObject.Find("MaingameManagers");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        processManager = maingameManagers.GetComponent<ProcessManager>();
        contentManager = maingameManagers.GetComponent<ContentManager>();
        exploreManager = maingameManagers.GetComponent<ExploreManager>();
    }

    public void InitDay()
    {
        processManager.InitCurrentDay();
    }


    public void EndDay()
    {
        processManager.EndCurrentDay();
    }

    public void StartExplore()
    {
        exploreManager.SelectCharacter(1);
        exploreManager.CheckStartExplore();
    }
}
