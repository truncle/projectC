//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Table;
using UnityEngine.UI;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using System.Resources;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting.FullSerializer;
//using Unity.Mathematics;

namespace Test
{


    public class CustomMenuItems
    {
        public static GameTest test;
        // 添加到 "Tools" 菜单
        [MenuItem("Tools/RunTest")]
        public static void RunTest()
        {
            test ??= new GameTest();
            test.LoadTest();
            test.contentManager.StartCoroutine(test.RunTest());
        }
    }

    public enum AssignStrategy
    {
        Auto, Never, ByConfig
    }

    public enum ExploreStrategy
    {
        Auto, Never, ByConfig
    }
    public enum StorylineStrategy
    {
        Auto, First, ByConfig
    }

    public class OperationData
    {
        public int day;
        public AssignStrategy assignStrategy;
        public ExploreStrategy exploreStrategy;
        public StorylineStrategy storylineStrategy;
        public bool prepareExplore;
        public List<(int characterId, int resourceType)> resourceAlloc = new();
        public List<ExploreOption> exploreOptions = new();
        public Dictionary<int, int> storylineOptions = new();
        public bool exit;
    }

    public class CheckData
    {
        public int day;
    }

    public class GameTest
    {
        public List<OperationData> opts = new();
        public List<CheckData> checks = new();
        public GameObject managerObject;
        public GameObject UIObject;
        public ContentManager contentManager;

        public Button OpenPanelButton;
        public Button NextPageButton;
        public Toggle PrepareExploreToggle;
        public IEnumerator RunTest()
        {
            int maxDay = EventStoryTable.datas.Max(e => e.day.Max());
            //maxDay = 17;
            for (int day = 1; day <= maxDay; day++)
            {
                //todo do check
                //--------------------------
                var opt = opts.Where(o => o.day == day).FirstOrDefault();
                if (opt != null)
                {
                    Debug.Log($"day {day} have operation");
                    AssignOpt(opt);
                    ExploreOpt(opt);
                    StorylineOpt(opt);
                }
                else
                {
                    Debug.Log("day " + day);
                    break;
                }

                Debug.Log("startViewing");
                int currDay = contentManager.processManager.CurrentDay;
                yield return new WaitForSeconds(0.1f);
                OpenPanelButton.onClick.Invoke();
                while (currDay == contentManager.processManager.CurrentDay)
                {
                    yield return new WaitForSeconds(0.1f);
                    Debug.Log("click next page");
                    NextPageButton.onClick.Invoke();
                }
            }
        }

        public void AssignOpt(OperationData opt)
        {
            if (opt.assignStrategy == AssignStrategy.Auto)
            {
                var assignCharacters = contentManager.AssignmentPage.assignCharacters;
                for (int i = 0; i < assignCharacters.Count; i++)
                {
                    var characterId = i + 1;
                    GameObject assignCharacter = assignCharacters[i];
                    CharacterStatus characterStatus = contentManager.resourceManager.GetCharacterStatus(characterId);
                    if (characterStatus.liveStatus != LiveStatus.Normal)
                        continue;
                    if (characterStatus.GetValue(StatusType.Thirsty) <= 1)
                        contentManager.AssignmentPage.SelectWater(assignCharacter);
                    if (characterStatus.GetValue(StatusType.Hungry) <= 1)
                        contentManager.AssignmentPage.SelectFood(assignCharacter);
                }
            }
            else
            {

            }
        }

        public void ExploreOpt(OperationData opt)
        {
            if (opt.exploreStrategy == ExploreStrategy.Auto)
            {
                var exploreManager = contentManager.exploreManager;
                var explorePage = contentManager.ExploryPage;
                if (exploreManager.exploreState == ExploreState.Idle)
                    PrepareExploreToggle.isOn = true;
                else if (exploreManager.exploreState == ExploreState.Start)
                {
                    foreach (var character in contentManager.resourceManager.characters)
                    {
                        if (character.liveStatus == LiveStatus.Normal)
                        {
                            var toggle = explorePage.ExploreCharacters.GetChild(character.characterId - 1).GetComponent<Toggle>();
                            toggle.isOn = true;
                            break;
                        }
                    }
                    if (explorePage.itemList.Count > 0)
                    {
                        explorePage.NextItem();
                    }
                }
            }
        }

        public void StorylineOpt(OperationData opt)
        {
            if (opt.storylineStrategy == StorylineStrategy.Auto)
            {
                var storylineManager = contentManager.storylineManager;
                var storylinePage = contentManager.StorylinePage;
                var resourceManager = contentManager.resourceManager;
                if (storylineManager.CurrentData.eventType == (int)EventStoryType.Select ||
                    storylineManager.CurrentData.eventType == (int)EventStoryType.ItemSelect)
                {
                    int subtype = storylineManager.CurrentData.eventTest;
                    Debug.Log($"subtype:{subtype}");
                    Toggle[] toggles = storylinePage.SelectGroup.GetComponentsInChildren<Toggle>().OrderBy(c => c.transform.GetSiblingIndex()).ToArray();
                    switch (subtype)
                    {
                        case 4:
                            toggles[0].isOn = false;
                            toggles[1].isOn = true;
                            break;
                        case 5:
                            toggles[0].isOn = true;
                            toggles[1].isOn = false;
                            break;
                        case 7:
                            if (toggles.Length > 1)
                            {
                                int select = Random.Range(1, toggles.Length);
                                for (int i = 1; i < toggles.Length; i++)
                                {
                                    if (i == select)
                                        toggles[select].isOn = true;
                                    else toggles[select].isOn = false;
                                }
                            }
                            break;
                    }
                }
                else if (storylineManager.CurrentData.eventType == (int)EventStoryType.GroupSelect)
                {
                    Toggle[] toggles = storylinePage.SelectGroup.GetComponentsInChildren<Toggle>().OrderBy(c => c.transform.GetSiblingIndex()).ToArray();
                    int select = Random.Range(1, toggles.Length);
                    for (int i = 0; i < toggles.Length; i++)
                    {
                        if (i == select)
                            toggles[select].isOn = true;
                        else toggles[select].isOn = false;
                    }
                }
            }
        }

        public void LoadTest()
        {
            EventStoryTable.Init();
            TestOperationTable.Init();
            foreach (var data in TestOperationTable.datas)
            {
                OperationData opt = new()
                {
                    day = data.day,
                    assignStrategy = (AssignStrategy)data.assignStrategy,
                    exploreStrategy = (ExploreStrategy)data.exploreStrategy,
                    storylineStrategy = (StorylineStrategy)data.storylineStrategy,
                    prepareExplore = data.prepareExplore == 1,
                };
                foreach (var singleAlloc in data.resourceAlloc)
                    opt.resourceAlloc.Add(new(singleAlloc[0], singleAlloc[1]));
                foreach (var singleOption in data.exploreOptions)
                    opt.exploreOptions.Add(new()
                    {
                        characterId = singleOption[0],
                        carryItem = singleOption[1],
                    });
                foreach (var singleOption in data.storylineOptions)
                    opt.storylineOptions.Add(singleOption[0], singleOption[1]);
                opts.Add(opt);
            }

            managerObject = GameObject.Find("MaingameManagers");
            UIObject = GameObject.Find("MaingameUI");
            contentManager = managerObject.GetComponent<ContentManager>();
            OpenPanelButton = UIObject.transform.Find("OpenPanelBtn").GetComponent<Button>();
            NextPageButton = UIObject.transform.Find("DiaryPanel/NextPageBtn").GetComponent<Button>();
            PrepareExploreToggle = UIObject.transform.Find("DiaryPanel/Explory/PreparePage/PrepareToggle").GetComponent<Toggle>();
            contentManager.DiaryPanel.Init();
        }
    }
}
