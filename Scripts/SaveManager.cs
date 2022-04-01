using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
	private static string m_EncryptionKey = "SoSafe";
	private static bool m_ScrambleData = false;

	public static string TurnIntoJson<T>(T objectToJson)
	{
		return JsonUtility.ToJson(objectToJson);
	}

	public static T ConvertFromJson<T>(string objectToJson)
	{
		T instance = JsonUtility.FromJson<T>(objectToJson);
		return instance;
	}

	public static void SaveTheJson(string FileName, string JsonToSave)
	{
		string saveFile = Application.persistentDataPath + "/" + FileName + ".data";
		if (m_ScrambleData)
			JsonToSave = DataScrambler(JsonToSave);
		writeFile(saveFile, JsonToSave);
	}

	public static string LoadTheJson(string FileName)
	{
		string saveFile = Application.persistentDataPath + "/" + FileName + ".data";
		string jsonToLoad = "";
		if (File.Exists(saveFile))
		{
			jsonToLoad = File.ReadAllText(saveFile);
			if(m_ScrambleData)
				jsonToLoad = DataScrambler(jsonToLoad);
		}
		else
		{
			Debug.LogError("Trying to load file that does not exist");
		}
		return jsonToLoad;
	}

	private static void writeFile(string saveFile, string JsonToSave)
	{
		File.WriteAllText(saveFile, JsonToSave);
	}

	public static void CreateDirectory(string DirectoryName)
	{
		string saveFile = Application.persistentDataPath + "/" + DirectoryName;
		Directory.CreateDirectory(saveFile);
	}

	public static bool CheckJsonExistence(string FileName)
	{
		string saveFile = Application.persistentDataPath + "/" + FileName + ".data";
		if (File.Exists(saveFile))
		{
			return true;
		}
		return false;
	}
	static string DataScrambler(string data)
	{
		string res = "";
		for (int i = 0; i < data.Length; i++)
		{
			res += (char)(data[i] ^ m_EncryptionKey[i % m_EncryptionKey.Length]);
		}
		return res;
	}
}