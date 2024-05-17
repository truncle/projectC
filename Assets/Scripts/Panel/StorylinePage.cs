using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StorylinePage : MonoBehaviour
{
    public ToggleGroup BinarySelectGroup;
    public ToggleGroup ItemGroup;
    public ToggleGroup MultiSelectGroup;

    public ToggleGroup SelectGroup;

    private GameObject nextPage;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init()
    {
        BinarySelectGroup = transform.Find("BinarySelectGroup").GetComponent<ToggleGroup>();
        ItemGroup = transform.Find("ItemGroup").GetComponent<ToggleGroup>();
        MultiSelectGroup = transform.Find("MultiSelectGroup").GetComponent<ToggleGroup>();
        nextPage = transform.parent.Find("NextPageBtn").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectGroup != null)
        {
            if (SelectGroup.ActiveToggles().Any())
                nextPage.SetActive(true);
            else nextPage.SetActive(false);
        }
    }

    private void OnDisable()
    {
        nextPage.SetActive(true);
    }
}
