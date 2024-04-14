using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
        Toggle toggle = character.transform.Find("Food").GetComponent<Toggle>();
        if (!toggle.isOn)
        {
            bool res = resourceManager.AllocResource(assignCharacters.IndexOf(character), ResourceType.Food, 1);
            if (!res) return;
        }
        else
        {
            bool res = resourceManager.UnallocResource(assignCharacters.IndexOf(character), ResourceType.Food, 1);
            if (!res) return;
        }
        toggle.isOn = !toggle.isOn;
    }

    public void SelectWater(GameObject character)
    {
        Toggle toggle = character.transform.Find("Water").GetComponent<Toggle>();
        if (!toggle.isOn)
        {
            bool res = resourceManager.AllocResource(assignCharacters.IndexOf(character), ResourceType.Water, 1);
            if (!res) return;
        }
        else
        {
            bool res = resourceManager.UnallocResource(assignCharacters.IndexOf(character), ResourceType.Water, 1);
            if (!res) return;
        }
        toggle.isOn = !toggle.isOn;
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
            SelectFood(character);
    }

    public void SelectCharacter(GameObject character)
    {
        SelectWater(character);
        SelectFood(character);
    }
}
