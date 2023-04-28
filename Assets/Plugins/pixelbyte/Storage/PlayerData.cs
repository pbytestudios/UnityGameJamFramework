using UnityEngine;
using System;
using Pixelbyte;

/// <summary>
/// This class encapsulates the PlayerPrefs class to provide and easier to 
/// work with variation
/// </summary>
static class PlayerData
{
	//If we ARE using the WebPlayer, or we don't want to use the file, then we'll wrap the PlayerPrefs Class
	//
	public static T Get<T>(string key)
	{
		string typeName = typeof(T).Name;
		if (typeName == "String")
			return (T)(object)PlayerPrefs.GetString(key);
		else if (typeName == "Int32")
			return (T)(object)PlayerPrefs.GetInt(key);
        else if (typeName == "Boolean")
            return (T)(object)bool.Parse(PlayerPrefs.GetString(key));
        else if (typeName == "Single")// -> single = float
            return (T)(object)PlayerPrefs.GetFloat(key);
        else
            Dbg.Err("Unsupported type: " + typeName);
		return default(T);
	}
	
	public static T Get<T>(string key, T defaultValue)
	{
		if(!PlayerPrefs.HasKey(key))
			return defaultValue;
		else
			return Get<T>(key);
	}

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
	
	public static void Set<T>(string key, T value) where T : IComparable
	{
		string typeName = typeof(T).Name;
		if (typeName == "String")
			PlayerPrefs.SetString(key, value.ToString());
		else if (typeName == "Int32")
			PlayerPrefs.SetInt(key, (int)(object)value);
		else if (typeName == "Boolean")
			PlayerPrefs.SetString(key, value.ToString());
		else if (typeName == "Single")// -> single = float
			PlayerPrefs.SetFloat(key, (float)(object)value);
		else
			Dbg.Err("Unsupported type: " + typeName);		
	}
	
	public static void DeleteKey (string key)
	{
        PlayerPrefs.DeleteKey(key);
	}
		
	public static void DeleteAll ()
	{
		PlayerPrefs.DeleteAll();
	}	
		
	//This method does nothing for the WebPlayer
	public static void Save ()
	{
        PlayerPrefs.Save();
	}
}