using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class GameInterface : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void ClickButton(string operation)
    {
        //GameEventSystem.Instance.SendUIEvent(operation);
    }
}
