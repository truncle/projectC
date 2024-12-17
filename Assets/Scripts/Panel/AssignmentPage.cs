using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class AssignmentPage : MonoBehaviour
{
    GameObject maingameManagers;
    ResourceManager resourceManager;
    public List<GameObject> assignCharacters = new();

    GameObject selectWaterBtn;
    GameObject selectFoodBtn;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnEnable()
    {
        Sync();
    }


    public void Init()
    {
        maingameManagers = GameObject.Find("MaingameManagers");
        resourceManager = maingameManagers.GetComponent<ResourceManager>();
        Transform charactersTransform = transform.Find("AssignCharacters");
        selectWaterBtn = transform.Find("SelectWaterBtn").gameObject;
        selectFoodBtn = transform.Find("SelectFoodBtn").gameObject;
        int characterCount = charactersTransform.childCount;
        for (int i = 0; i < characterCount; i++)
            assignCharacters.Add(charactersTransform.GetChild(i).gameObject);
    }

    public void Sync()
    {
        for (int i = 0; i < assignCharacters.Count; i++)
        {
            var characterId = i + 1;
            GameObject assignCharacter = assignCharacters[i];
            var waterCheckMark = assignCharacter.transform.Find("Water/CheckMark").gameObject;
            var foodCheckMark = assignCharacter.transform.Find("Food/CheckMark").gameObject;
            var waterSign = assignCharacter.transform.Find("Avatar/WaterSign").gameObject;
            var foodSign = assignCharacter.transform.Find("Avatar/FoodSign").gameObject;
            var stateSign = assignCharacter.transform.Find("Avatar/StateSign").gameObject;
            CharacterStatus characterStatus = resourceManager.GetCharacterStatus(characterId);
            if (characterStatus.GetValue(StatusType.Thirsty) <= 2)
                waterSign.SetActive(true);
            else waterSign.SetActive(false);
            if (characterStatus.GetValue(StatusType.Hungry) <= 4)
                foodSign.SetActive(true);
            else foodSign.SetActive(false);
            if (characterStatus.liveStatus != LiveStatus.Normal)
                stateSign.SetActive(true);
            else stateSign.SetActive(false);
            waterCheckMark.SetActive(false);
            foodCheckMark.SetActive(false);
        }
        selectWaterBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"WaterNum: {resourceManager.GetResourceNum(ResourceType.Water)}";
        selectFoodBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"FoodNum: {resourceManager.GetResourceNum(ResourceType.Food)}";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectFood(GameObject character)
    {
        GameObject checkMark = character.transform.Find("Food/CheckMark").gameObject;
        int characterId = assignCharacters.IndexOf(character) + 1;
        var characterStatus = resourceManager.GetCharacterStatus(characterId);
        if (characterStatus.liveStatus == LiveStatus.Dead)
            return;
        if (checkMark.activeSelf == false)
        {
            bool res = resourceManager.AllocResource(characterId, ResourceType.Food);
            if (!res) return;
        }
        else
        {
            bool res = resourceManager.UnallocResource(characterId, ResourceType.Food);
            if (!res) return;
        }
        checkMark.SetActive(!checkMark.activeSelf);
    }

    public void SelectWater(GameObject character)
    {
        GameObject checkMark = character.transform.Find("Water/CheckMark").gameObject;
        int characterId = assignCharacters.IndexOf(character) + 1;
        var characterStatus = resourceManager.GetCharacterStatus(characterId);
        if (characterStatus.liveStatus == LiveStatus.Dead)
            return;
        if (checkMark.activeSelf == false)
        {
            bool res = resourceManager.AllocResource(characterId, ResourceType.Water);
            if (!res) return;
        }
        else
        {
            bool res = resourceManager.UnallocResource(characterId, ResourceType.Water);
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
