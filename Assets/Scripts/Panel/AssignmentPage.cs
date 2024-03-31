using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignmentPage : MonoBehaviour
{
    GameManager gameManager;
    GameObject maingameManagers;
    private List<GameObject> assignCharacters = new();

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        maingameManagers = GameObject.Find("MaingameManagers");
        Transform charactersTransform = transform.Find("AssignCharacters");
        int characterCount = charactersTransform.childCount;
        for (int i = 0; i < characterCount; i++)
            assignCharacters.Add(charactersTransform.GetChild(i).gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectWater()
    {
        foreach (var character in assignCharacters)
        {
            SelectWater(character);
        }
    }

    public void SelectWater(GameObject character)
    {
        Toggle toggle = character.transform.Find("Water").GetComponent<Toggle>();
        toggle.isOn = !toggle.isOn;
    }

    public void SelectFood()
    {
        foreach (var character in assignCharacters)
            SelectFood(character);
    }

    public void SelectFood(GameObject character)
    {
        Toggle toggle = character.transform.Find("Food").GetComponent<Toggle>();
        toggle.isOn = !toggle.isOn;
    }

    public void SelectCharacter(GameObject character)
    {
        SelectWater(character);
        SelectFood(character);
    }
}
