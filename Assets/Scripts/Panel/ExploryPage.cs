using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Util;

public class ExploryPage : MonoBehaviour
{
    GameObject maingameManagers;
    private ResourceManager resourceManager;
    private ExploreManager exploreManager;

    private Transform PreparePage;

    private Transform StartingPage;

    private Transform CharactersStatus;

    public Transform ExploreCharacters;

    public List<int> itemList;

    private Sprite defaultItemSprite;

    public int currItemIndex = -1;

    private Image imageItem;

    // Start is called before the first frame update
    public void Start()
    {
    }

    public void Init()
    {
        maingameManagers = GameObject.Find("MaingameManagers");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        exploreManager = maingameManagers.GetComponent<ExploreManager>();
        itemList = resourceManager.itemsTemp.ToList();

        PreparePage = transform.Find("PreparePage");
        StartingPage = transform.Find("StartingPage");
        CharactersStatus = PreparePage.transform.Find("CharactersStatus");
        ExploreCharacters = StartingPage.transform.Find("ExploreCharacters");

        imageItem = StartingPage.transform.Find("SelectItem/Item").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Sync()
    {
        // todo 更新角色状态展示文本
        for (int i = 0; i < CharactersStatus.childCount; i++)
        {
            var characterId = i + 1;
            Transform characterStatus = CharactersStatus.transform.GetChild(i);
            TextMeshProUGUI describe = characterStatus.Find("Describe").GetComponent<TextMeshProUGUI>();
            CharacterStatus character = resourceManager.GetCharacterStatus(characterId);
            describe.text = character.ToString();
            var waterSign = characterStatus.transform.Find("Avatar/WaterSign").gameObject;
            var foodSign = characterStatus.transform.Find("Avatar/FoodSign").gameObject;
            var stateSign = characterStatus.transform.Find("Avatar/StateSign").gameObject;
            if (character.GetValue(StatusType.Thirsty) <= 2)
                waterSign.SetActive(true);
            else waterSign.SetActive(false);
            if (character.GetValue(StatusType.Hungry) <= 2)
                foodSign.SetActive(true);
            else foodSign.SetActive(false);
            if (character.liveStatus != LiveStatus.Normal)
                stateSign.SetActive(true);
            else stateSign.SetActive(false);
        }

        itemList = resourceManager.itemsTemp.ToList();
        Toggle prepareToggle = PreparePage.Find("PrepareToggle").GetComponent<Toggle>();
        prepareToggle.isOn = exploreManager.PrepareExplore;
        foreach (var characterToggle in ExploreCharacters.GetComponentsInChildren<Toggle>())
        {
            characterToggle.isOn = false;
        }
        currItemIndex = -1;
        imageItem.sprite = defaultItemSprite;
    }

    public void NextItem()
    {
        currItemIndex++;
        if (currItemIndex == itemList.Count)
        {
            currItemIndex = -1;
            imageItem.sprite = defaultItemSprite;
            return;
        }
        currItemIndex %= itemList.Count;
        imageItem.sprite = SpriteLoader.GetItemSprite(itemList[currItemIndex]);
    }

    public void PrevItem()
    {
        currItemIndex -= 1;
        if (currItemIndex == -1)
        {
            imageItem.sprite = defaultItemSprite;
            return;
        }
        if (currItemIndex < 0)
            currItemIndex = itemList.Count - 1;
        imageItem.sprite = SpriteLoader.GetItemSprite(itemList[currItemIndex]);
    }

    public ExploreOption GetExploreOption()
    {
        int itemId = currItemIndex > 0 ? itemList[currItemIndex] : 0;
        int characterId = 0;
        for (int i = 0; i < ExploreCharacters.transform.childCount; i++)
        {
            Toggle toggle = ExploreCharacters.transform.GetChild(i).GetComponent<Toggle>();
            if (toggle.isOn)
                characterId = i + 1;
        }
        return new()
        {
            characterId = characterId,
            carryItem = itemId,
        };
    }
}
