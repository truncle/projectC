using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class AssignmentPage : MonoBehaviour
{
    GameManager gameManager;
    GameObject maingameManagers;
    ResourceManager resourceManager;
    private List<GameObject> assignCharacters = new();

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        maingameManagers = GameObject.Find("MaingameManagers");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        Transform charactersTransform = transform.Find("AssignCharacters");
        int characterCount = charactersTransform.childCount;
        for (int i = 0; i < characterCount; i++)
            assignCharacters.Add(charactersTransform.GetChild(i).gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectFood(GameObject character)
    {
        GameObject checkMark = character.transform.Find("Food/CheckMark").gameObject;
        if (checkMark.activeSelf == false)
        {
            bool res = resourceManager.AllocResource(assignCharacters.IndexOf(character) + 1, ResourceType.Food, 1);
            if (!res) return;
        }
        else
        {
            bool res = resourceManager.UnallocResource(assignCharacters.IndexOf(character) + 1, ResourceType.Food, 1);
            if (!res) return;
        }
        checkMark.SetActive(!checkMark.activeSelf);
    }

    public void SelectWater(GameObject character)
    {
        GameObject checkMark = character.transform.Find("Water/CheckMark").gameObject;
        if (checkMark.activeSelf == false)
        {
            bool res = resourceManager.AllocResource(assignCharacters.IndexOf(character) + 1, ResourceType.Water, 1);
            if (!res) return;
        }
        else
        {
            bool res = resourceManager.UnallocResource(assignCharacters.IndexOf(character) + 1, ResourceType.Water, 1);
            if (!res) return;
        }
        checkMark.SetActive(!checkMark.activeSelf);
    }

    public void SelectWater()
    {
        foreach (var character in assignCharacters)
        {
            SelectWater(character);
        }
    }

    public void SelectFood()
    {
        foreach (var character in assignCharacters)
        {
            SelectFood(character);
        }
    }

    public void SelectCharacter(GameObject character)
    {
        SelectWater(character);
        SelectFood(character);
    }
}
