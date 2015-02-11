using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SC_manager_ui : MonoBehaviour {

	[SerializeField]
	private Text _Text_score_team_true;
	[SerializeField]
	private Text _Text_score_team_false;

	[SerializeField]
	private GameObject _GO_timer;
	[SerializeField]
	private Text _Text_timer;

	[SerializeField]
	private GameObject _GO_button_back_slots_brawler;
	[SerializeField]
	private GameObject _GO_button_back_types;

	[SerializeField]
	private GameObject _GO_panel_actions_slots_brawler;
	[SerializeField]
	private GameObject _GO_panel_actions_types;

	[SerializeField]
	private Text _t_button_slot_1;
	[SerializeField]
	private Text _t_button_slot_2;
	[SerializeField]
	private Text _t_button_slot_3;

	[SerializeField]
	private Button _BU_button_slot_1;
	[SerializeField]
	private Button _BU_button_slot_2;
	[SerializeField]
	private Button _BU_button_slot_3;


	[SerializeField]
	private GameObject _GO_button_end_turn;

	[SerializeField]
	private GameObject _GO_menu_end;
	[SerializeField]
	private Text _test_end;

	public void Awake()
	{
		//SetActiveButtonBackSlotsBrawler(false);
		SetActiveButtonBackTypes(false);
		SetActivePanelActionsSlotsBrawler(false);
		SetActivePanelActionsTypes(false);
	}

	public void HighlightSlot(int selected_slot)
	{	
		switch (selected_slot) {
			case 0:					
					_BU_button_slot_1.image.color = new Color (248f/255f, 234f/255f, 127f/255f,1);
					_BU_button_slot_2.image.color = new Color (1,1,1,1);
					_BU_button_slot_3.image.color = new Color (1,1,1,1);
					break;
			case 1:
					_BU_button_slot_2.image.color = new Color (248f/255f, 234f/255f, 127f/255f,1);
					_BU_button_slot_1.image.color = new Color (1,1,1,1);
					_BU_button_slot_3.image.color = new Color (1,1,1,1);
					break;
			case 2:			
					_BU_button_slot_3.image.color = new Color (248f/255f, 234f/255f, 127f/255f,1);
					_BU_button_slot_2.image.color = new Color (1,1,1,1);
					_BU_button_slot_1.image.color = new Color (1,1,1,1);
					break;
			default:
					break;
		}
	}


	public void SetActiveButtonBackSlotsBrawler(bool b_active)
	{
		_GO_panel_actions_slots_brawler.SetActive(b_active);
	}

	public void SetActiveButtonEndTurn(bool b_active)
	{
		_GO_button_end_turn.SetActive(b_active);
	}

	public void SetActiveButtonBackTypes(bool b_active)
	{
		_GO_button_back_types.SetActive(b_active);
	}

	public void SetActivePanelActionsSlotsBrawler(bool b_active)
	{
		_GO_panel_actions_slots_brawler.SetActive(b_active);
	}
	
	public void SetActivePanelActionsTypes(bool b_active)
	{
		_GO_panel_actions_types.SetActive(b_active);
	}

	public void SetActionSlotText(string text, int slotNumber){	
		switch (slotNumber+1) {
			case 1:
				_t_button_slot_1.text = text;
				break;
			case 2:
				_t_button_slot_2.text = text;
				break;
			case 3:
				_t_button_slot_3.text = text;
				break;
			default:
				break;
		}
	}

	public void UpdateActionsSlotForBrawler(SC_brawler brawler){
		/** ne marche pas il faudrait stocker les buttons dans un tableau pour les parcourir en meme temps que les actions
		int nb_actions = SC_manager_game._instance._game_settings._number_actions_per_turn;
		for (int i = 0; i < nb_actions-1; i++) {
			_t_button_slot_1.text = brawler._actions [i].ToString ();
		}
		**/
		_t_button_slot_1.text = brawler._actions [0].ToString ();
		_t_button_slot_2.text = brawler._actions [1].ToString ();
		_t_button_slot_3.text = brawler._actions [2].ToString ();

	}

	public void SetScore(bool b_team, int i_score)
	{
		if (b_team)
			_Text_score_team_true.text = i_score.ToString();
		else
			_Text_score_team_false.text = i_score.ToString();
	}

	public void StartTimer(int i_duration)
	{
		_GO_timer.SetActive(true);
		_Text_timer.text = i_duration.ToString();
		StartCoroutine("Timer_Coroutine", i_duration);
	}

	public IEnumerator Timer_Coroutine(int i_duration)
	{
		int i_timer = i_duration - 1;
		float f_timer = i_duration;
		_Text_timer.text = i_timer.ToString();

		while (i_timer > 0)
		{
			f_timer -= Time.deltaTime;
			if (f_timer < i_timer)
			{
				i_timer--;
				_Text_timer.text = i_timer.ToString();
			}
			yield return null;
		}
	}

	public void EndTimer()
	{
		StopCoroutine("Timer_Coroutine");
		_GO_timer.SetActive(false);
	}

	public void SetEnd(GameResult game_result)
	{
		_GO_menu_end.SetActive(true);
		_GO_timer.SetActive(false);
		switch(game_result)
		{
		case GameResult.Win:
			_test_end.text = "You Win !";
			break;
		case GameResult.Lose:
			_test_end.text = "You Lose !";
			break;
		case GameResult.Draw:
			_test_end.text = "Draw !";
			break;
		}
	}


}
