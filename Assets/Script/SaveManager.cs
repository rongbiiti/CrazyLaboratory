using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
	
	[System.Serializable]
	public class SaveData
	{
		public float playerHP;
		public int stage;
	}
	
	protected static readonly string[] findTags =
	{
		"SaveManager",
	};

	private static SaveManager instance;

	public static SaveManager Instance
	{
		get
		{
			if (instance == null)
			{

				Type type = typeof(SaveManager);

				foreach (var tag in findTags)
				{
					GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

					for (int j = 0; j < objs.Length; j++)
					{
						instance = (SaveManager)objs[j].GetComponent(type);
						if (instance != null)
							return instance;
					}
				}

				Debug.LogWarning(string.Format("{0} is not found", type.Name));
			}

			return instance;
		}
	}
	
	string filePath;
	public SaveData save;
	public bool IsNewGame { get; set; }

	void Awake()
	{
		CheckInstance();
		filePath = Application.persistentDataPath + "/" + ".savedata.json";
		save = new SaveData();
		Debug.Log(filePath);
		Load();
	}

	private bool CheckInstance()
	{
		if (instance == null)
		{
			instance = (SaveManager)this;
			DontDestroyOnLoad(gameObject);
			return true;
		}
		else if (Instance == this)
		{
			return true;
		}

		Destroy(this);
		return false;
	}
	
	public void Save(float hp, int stage)
	{
		save.playerHP = hp;
		if(stage > save.stage)
			save.stage = stage;
		string json = JsonUtility.ToJson(save);
		StreamWriter streamWriter = new StreamWriter(filePath);
		streamWriter.Write(json);
		streamWriter.Flush();
		streamWriter.Close();
	}
  
	public void Load()
	{
		if (!File.Exists(filePath)) return;
		var streamReader = new StreamReader(filePath);
		string data = streamReader.ReadToEnd();
		streamReader.Close();
		save = JsonUtility.FromJson<SaveData>(data);
	}
}
