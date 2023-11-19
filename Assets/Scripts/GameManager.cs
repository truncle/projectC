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
		}
	}

	//��Ϸ����ʱ��ȡ�ı�
	private void LoadTables()
	{
	}

	public void OnButtonClick()
	{
		Debug.Log("Click Button");
	}
}
