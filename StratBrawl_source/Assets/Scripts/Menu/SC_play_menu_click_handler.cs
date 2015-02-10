using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// SUMMARY : This script runs when the current panel is the "play panel"
/// This panel contains thes list of the current server waiting for a player
/// The user can click on a server to join it.
/// The user can create a new server by clicking on the create game button.
public class SC_play_menu_click_handler : MonoBehaviour
{
		private List<GameObject> _games_button = new List<GameObject> ();
		private float _time = 4;
		private const int _REFRESH_RATE = 5;
		[SerializeField]
		private GameObject
				_GO_current_panel;
		[SerializeField]
		private GameObject
				_GO_servers_container;
		[SerializeField]
		private GameObject
				_GO_button_server;
		[SerializeField]
		private Text
				_TE_lobby_title;
		[SerializeField]
		private GameObject
				_GO_lobby_panel;

		
		/// SUMMARY : The user click on the create game button. The next panel is displayed.
		/// PARAMETERS : The next panel.
		/// RETURN : Void.
		public void ClickCreateGameButton (GameObject panel_to_show)
		{
				_GO_current_panel.SetActive (false);
				panel_to_show.SetActive (true);
		}


		/// SUMMARY : Retrieve master server hosts list
		/// PARAMETERS : None.
		/// RETURN : Void.
		void RefreshServersList ()
		{	
				foreach (GameObject btn_obj in _games_button) {
						Destroy (btn_obj);
				}
				_games_button.Clear ();
				if (MasterServer.PollHostList ().Length != 0) {
						HostData[] hostData = MasterServer.PollHostList ();
						int i = 0;
						while (i < hostData.Length) {
								HostData server = hostData [i];
								string server_name = server.gameName;

								GameObject button_obj = Instantiate (_GO_button_server) as GameObject;
								Button button = button_obj.GetComponentInChildren<Button> ();

								button.GetComponentInChildren<Text> ().text = server_name;
								button.transform.SetParent (_GO_servers_container.transform, false);

								button.onClick.AddListener (delegate {
										NextPanel (server);
								});

								_games_button.Add (button_obj);
								//button.transform.Translate (new Vector3 (0, 2, 0));

				
								i++;
						}
						MasterServer.ClearHostList ();
				}
		}

		//The script is awake, request the server list from the master server
		void Awake ()
		{
				MasterServer.ClearHostList ();
				MasterServer.RequestHostList ("1V1");

		}

		//We want to refresh the server list each _REFRESH_RATE time
		void Update ()
		{
				_time += Time.deltaTime;
				if (_time > _REFRESH_RATE) {
						_time = 0;
						RefreshServersList ();
				}
		}

		/// SUMMARY : The user want to join a server
		/// PARAMETERS : None.
		/// RETURN : Void.
		void NextPanel (HostData server)
		{
				_GO_current_panel.SetActive (false);
				_GO_lobby_panel.SetActive (true);
				_TE_lobby_title.text = server.gameName;
				Network.Connect (server);
		}
}
