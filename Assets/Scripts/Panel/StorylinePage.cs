using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StorylinePage : MonoBehaviour
{
    public ToggleGroup SelectGroup;

    private GameObject nextPage;

    // Start is called before the first frame update
    void Start()
    {
        SelectGroup = GetComponentInChildren<ToggleGroup>();
        nextPage = GameObject.Find("NextPageBtn");
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectGroup.ActiveToggles().Any())
            nextPage.SetActive(true);
        else nextPage.SetActive(false);

    }
}
