using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploryPage : MonoBehaviour
{
    public GameObject StartingPage;

    // Start is called before the first frame update
    void Start()
    {
        StartingPage = transform.Find("StartingPage").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
