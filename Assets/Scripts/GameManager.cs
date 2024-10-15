using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;

	public static GameManager Instance
	{
		get { return instance; }
	}

	private void Awake()
	{
		LoadTables();
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		SceneManager.LoadSceneAsync("GuiScene", LoadSceneMode.Additive);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			List<List<Table.Condition>> result = Util.GameUtil.GetConditionSet("EVT:EQUAL[10001:1]");
			Table.Condition condition = result[0][0];
		}
	}

	//游戏启动时读取的表
	private void LoadTables()
	{
	}
}
