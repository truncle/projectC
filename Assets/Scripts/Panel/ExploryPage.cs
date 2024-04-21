using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploryPage : MonoBehaviour
{
    public GameObject PreparePage;

    public GameObject StartingPage;

    private List<Sprite> itemSprites = new();

    private int currItem = 0;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        PreparePage = transform.Find("PreparePage").gameObject;
        StartingPage = transform.Find("StartingPage").gameObject;
        itemSprites = new(Resources.LoadAll<Sprite>("Pictures/items"));
        image = StartingPage.transform.Find("SelectItem/Item").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextItem()
    {
        currItem += 1;
        currItem %= itemSprites.Count;
        image.sprite = itemSprites[currItem];
    }

    public void PrevItem()
    {
        currItem -= 1;
        if (currItem < 0)
            currItem += itemSprites.Count;
        image.sprite = itemSprites[currItem];
    }
}
