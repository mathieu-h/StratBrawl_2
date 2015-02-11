using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SC_replay_files_manager : MonoBehaviour {

	static private string _s_directory_name = Application.persistentDataPath + "/Replays";


	/// SUMMARY :
	/// PARAMETERS : 
	/// RETURN : Void.
	static private void InitDirectory()
	{
		if (!Directory.Exists(_s_directory_name))
			Directory.CreateDirectory(_s_directory_name);
	}


	/// SUMMARY :
	/// PARAMETERS : 
	/// RETURN : Void.
	static public void SaveReplay(byte[] data_replay)
	{
		InitDirectory();

		string s_path = _s_directory_name + "/replay_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".sbreplay";
		File.WriteAllBytes(s_path, data_replay);
	}


	/// SUMMARY :
	/// PARAMETERS :
	/// RETURN : Void.
	static public string[] GetReplaysNames()
	{
		InitDirectory();

		char[] char_split = new char[] {'/', '\\'};
		string[] s_files_name = Directory.GetFiles(_s_directory_name);
		for (int i = 0; i < s_files_name.Length; ++i)
		{
			string[] s_tmp = s_files_name[i].Split (char_split);
			s_files_name[i] = s_tmp[s_tmp.Length - 1];
		}

		return s_files_name;
	}


	/// SUMMARY :
	/// PARAMETERS :
	/// RETURN : Void.
	static public Replay LoadReplay(string s_file_name)
	{
		InitDirectory();
		Debug.Log(s_file_name);
		string s_path = _s_directory_name + "/" + s_file_name;
		byte[] data_replay = File.ReadAllBytes(s_path);
		BinaryFormatter _BF = new BinaryFormatter();
		MemoryStream _MS = new MemoryStream();
		_MS.Write(data_replay,0,data_replay.Length); 
		_MS.Seek(0, SeekOrigin.Begin); 
		Replay _replay = (Replay)_BF.Deserialize(_MS);
		
		return _replay;
	}
}
