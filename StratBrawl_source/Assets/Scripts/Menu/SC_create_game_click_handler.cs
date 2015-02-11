using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// SUMMARY : This script runs when the current panel is the create game panel
/// The user can create a game by entering a game name and by pressing the button
public class SC_create_game_click_handler : MonoBehaviour
{
		[SerializeField]
		private GameObject
				_GO_current_panel;
		[SerializeField]
		private GameObject
				_GO_next_panel;
		[SerializeField]
		private Text
				_TE_lobby_title;

		/// SUMMARY : The user click on the create game button. We must create a new server by calling the RegisterAGame method
		/// PARAMETERS : The input field containing the game name
		/// RETURN : Void.
		public void ClickCreateButton (InputField gameName)
		{
				if(!gameName.text.Equals("")){
					_GO_current_panel.SetActive (false);
					_GO_next_panel.SetActive (true);
					_TE_lobby_title.text = gameName.text;
					RegisterAGame (gameName.text);
				}
		}
		
		/// SUMMARY : Initialize a new server using the game name and register it to the master server
		/// PARAMETERS : The name of the new server
		/// RETURN : Void.
		public void RegisterAGame (string gameName)
		{
	
				Network.InitializeServer (32, 1119, true);// !Network.HavePublicAddress ());
				MasterServer.ipAddress = "127.0.0.1";
				MasterServer.port = 23466;
				MasterServer.RegisterHost ("1V1", gameName, "Test Comment");
		}

		/// SUMMARY : The user click on the back button. We go back.
		/// RETURN : Void.
		public void ClickBackButton (GameObject panelToShow)
		{
				_GO_current_panel.SetActive (false);
				panelToShow.SetActive (true);
		}
}
