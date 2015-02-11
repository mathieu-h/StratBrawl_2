using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SC_replay_menu : MonoBehaviour {

	[SerializeField]
	private GameObject _GO_current_panel;

	[SerializeField]
	private GameObject _GO_row_button;

	[SerializeField]
	private GameObject
		_GO_replay_container;

	// Use this for initialization
	void Start () {
	
		string[] names = SC_replay_files_manager.GetReplaysNames ();
		//string[] names = {"BLA1", "BLA2","BLA3"};
		for (int i = 0; i < names.Length; ++i)
		{
			GameObject button_obj = Instantiate (_GO_row_button) as GameObject;
			Button button = button_obj.GetComponentInChildren<Button> ();
			
			button.GetComponentInChildren<Text> ().text = names[i];
			button.transform.SetParent (_GO_replay_container.transform, false);

			button.transform.Translate(Vector3.down * 50 * i);
			//button.GetComponent<RectTransform>().position = Vector3.down * 20;
			
			button.onClick.AddListener (delegate {
				Text name = button.GetComponentsInChildren<Text>()[0];
				NextPanel (name.text);
			});
		}


	}

	/// SUMMARY : The user click on the back button. We go back.
	/// RETURN : Void.
	public void ClickBackButton(GameObject panelToShow)
	{
		_GO_current_panel.SetActive (false);
		panelToShow.SetActive (true);
	}


	/// SUMMARY : The user want to watch a replays
	/// PARAMETERS : The replay name
	/// RETURN : Void.
	void NextPanel (string name)
	{
		_GO_current_panel.SetActive (false);
		SC_replay_manager._replay = SC_replay_files_manager.LoadReplay (name);
		Application.LoadLevel ("Replay");
	}
}
