using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// SUMMARY : This script runs when the current panel is the main menu
/// The user can click to play, option or quit button
public class SC_menu_click_handler: MonoBehaviour
{
		[SerializeField]
		private GameObject
				_GO_current_panel;

		/// SUMMARY : The user click on the play button. We display the play panel.
		/// PARAMETERS : The next panel.
		/// RETURN : Void.
		public void ClickPlayButton (GameObject panelToShow)
		{
				_GO_current_panel.SetActive (false);
				RetrieveHostList ();
				
				panelToShow.SetActive (true);
		}

		/// SUMMARY : The user click on the Replay button. We display the replay panel.
		/// PARAMETERS : The next panel.
		/// RETURN : Void.
		public void ClickReplayButton (GameObject panelToShow)
		{
			_GO_current_panel.SetActive (false);
			panelToShow.SetActive (true);
		}

		/// SUMMARY : The user click on the play button. We display the option panel.
		/// PARAMETERS : The next panel.
		/// RETURN : Void.
		public void ClickOptionsButton (GameObject panelToShow)
		{
				_GO_current_panel.SetActive (false);
				panelToShow.SetActive (true);
		}

		/// SUMMARY : The user click on the play button. We quit the game;
		/// PARAMETERS : None.
		/// RETURN : Void.
		public void ClickQuitButton ()
		{
				Application.Quit ();
		}

		/// SUMMARY : Retrieve from MasterServer, servers waiting for players
		/// PARAMETERS : None.
		/// RETURN : Return the servers waiting for players
		public void RetrieveHostList ()
		{
				MasterServer.ipAddress = "127.0.0.1";
				MasterServer.port = 23466;
				MasterServer.RequestHostList ("1V1");
		}

		void Start ()
		{
				Screen.autorotateToPortrait = true;
				Screen.autorotateToLandscapeRight = false;
				Screen.autorotateToLandscapeLeft = false;
				Screen.autorotateToPortraitUpsideDown = false;
				Screen.orientation = ScreenOrientation.Portrait;
		}
}
