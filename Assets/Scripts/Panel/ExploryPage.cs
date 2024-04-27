using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ExploryPage : MonoBehaviour
{
    GameObject maingameManagers;
    public ResourceManager resourceManager;

    public GameObject PreparePage;

    public GameObject StartingPage;

    public GameObject ExploreCharacters;

    private List<int> itemList;

    private List<Sprite> itemSprites = new();

    [SerializeField]
    private Sprite noneItemSprite;

    private int currItemIndex = -1;

    private Image imageItem;

    private void OnEnable()
    {
        maingameManagers = GameObject.Find("MaingameManagers");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        itemList = resourceManager.itemsTemp.ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        PreparePage = transform.Find("PreparePage").gameObject;
        StartingPage = transform.Find("StartingPage").gameObject;
        ExploreCharacters = StartingPage.transform.Find("ExploreCharacters").gameObject;
        itemSprites = new(Resources.LoadAll<Sprite>("Pictures/items"));
        imageItem = StartingPage.transform.Find("SelectItem/Item").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextItem()
    {
        Debug.Log(currItemIndex);
        currItemIndex += 1;
        if (currItemIndex == itemList.Count)
        {
            currItemIndex = -1;
            imageItem.sprite = noneItemSprite;
            return;
        }
        currItemIndex %= itemList.Count;
        imageItem.sprite = itemSprites[currItemIndex];
    }

    public void PrevItem()
    {
        currItemIndex -= 1;
        if (currItemIndex == -1)
        {
            imageItem.sprite = noneItemSprite;
            return;
        }
        if (currItemIndex < 0)
            currItemIndex = itemList.Count - 1;
        imageItem.sprite = itemSprites[currItemIndex];
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
