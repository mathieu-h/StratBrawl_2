using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public partial class SC_game_manager_client : MonoBehaviour {

	public SC_brawler _selected_brawler;
	public Action _selected_action;
	//public Action[] _actions_chosen;
	private int _number_of_chosen_actions;
	private int _selected_slot;
	private SC_cell _selected_cell;

	[SerializeField]
	private Sprite _Spr_ui_arrow;
	[SerializeField]
	private Sprite _Spr_ui_ball;

	private Image[,] _img_brawler_actions_cells;


	//Planification

	public void InitPlanification(){
		_board_game.SetActiveButtonsBrawlers(true, _b_player_team);
		_manager_ui.SetActiveButtonEndTurn (true);
		_manager_ui.StartTimer(_game_settings._i_planification_time);
		int i_width = _game_settings._i_nb_brawlers_per_team;
		int i_height = 3;		
		// TODO int i_height = _game_settings._i_nb_actions_per_turn;
		_img_brawler_actions_cells = new Image[i_width,i_height];
		for(int i = 0; i < i_width; i++)
		{
			for(int j = 0; j < i_height; j++)
			{
				_img_brawler_actions_cells[i,j] = null;
			}
		}
	}


	/// SUMMARY : Store the current brawler in the attribute, then open the panel containing the slots of action for this turn 
	/// aswell as the back button.Last thingswitch off the brawler buttons.
	/// PARAMETERS : The brawler that will execute the selected actions in the slots or null if it is called by CloseMenuActionsTypes.
	/// RETURN : Void.
	public void OpenMenuActionsSlots(SC_brawler _brawler=null)
	{
		_manager_ui.SetActivePanelActionsSlotsBrawler(true);
		_manager_ui.SetActiveButtonBackSlotsBrawler(true);
		_manager_ui.SetActiveButtonEndTurn (false);
		if (_selected_brawler != null) {
			_selected_brawler.ResetMat();
			SetActiveCellsForMoveAndTackle (false);
			SetActiveCellsForPass (false);
		}

		// Test to check if it is the first time the method is called or not this turn, since only CloseMenuActionsTypes calls it
		// without _brawler parameter
		//if (_brawler != null) {			
		_selected_brawler = _brawler;
		_manager_ui.UpdateActionsSlotForBrawler(_selected_brawler);
		_brawler.HighLightBrawler();
			//_board_game.SetActiveButtonsBrawlers (false, _brawler._b_team);
		//}
		// By default the first action is selected
		_selected_slot = 0;		
		_manager_ui.HighlightSlot (_selected_slot);
		OpenMenuActionsTypes ();
	}

	/// SUMMARY : Opens the panel containing the different types slots of action possible
	/// PARAMETERS : 
	/// RETURN : Void.
	public void OpenMenuActionsTypes()
	{
		//_manager_ui.SetActivePanelActionsSlotsBrawler(false);		
		_manager_ui.SetActivePanelActionsTypes(true);		
		//_manager_ui.SetActiveButtonBackSlotsBrawler(false);	
		//_manager_ui.SetActiveButtonBackTypes(true);		
	}
	
	public void CloseMenuActionsSlots()
	{
		_board_game.SetActiveButtonsBrawlers(true,_b_player_team);
		_manager_ui.SetActiveButtonBackSlotsBrawler(false);
		_manager_ui.SetActivePanelActionsSlotsBrawler(false);
		_manager_ui.SetActiveButtonEndTurn (true);
		_manager_ui.SetActiveButtonBackTypes(false);
		_manager_ui.SetActivePanelActionsTypes(false);
		SetActiveCellsForMoveAndTackle (false);
		SetActiveCellsForPass (false);
		_selected_brawler.ResetMat();
		_selected_brawler = null;
	}


	
	public void SetActiveCellsForMoveAndTackle(bool active)
	{
		if (_selected_cell != null && active)
		{
			SetActiveCellsForMoveAndTackle (false);
			SetActiveCellsForPass (false);
		}
		GridPosition cell_selected_position = CalculateCurrentPositionWithFutureActions();
		int x = (int)cell_selected_position.GetWorldPosition ().x;
		int y = (int)cell_selected_position.GetWorldPosition ().y;

		SetCellActive (x + 1, y, active);
		SetCellActive (x - 1, y, active);
		SetCellActive (x, y + 1, active);
		SetCellActive (x, y - 1, active);
	}

	public void SetActiveCellsForPass(bool active)
	{
		if (_selected_cell != null && active)
		{
			SetActiveCellsForMoveAndTackle (false);
			SetActiveCellsForPass (false);
		}
		GridPosition cell_selected_position = CalculateCurrentPositionWithFutureActions();
		// TODO int nb_cells = _game_settings._i_pass_nb_cells;
		int nb_cells = 5;
		int x = (int)cell_selected_position.GetWorldPosition ().x;
		int y = (int)cell_selected_position.GetWorldPosition ().y;
		for (int i=0; i <= nb_cells; i++) {
			for(int j=nb_cells-i;j>=0;j--){
				SetCellActive(x+j,y+i,active);
				SetCellActive(x-j,y+i,active);
				SetCellActive(x+j,y-i,active);
				SetCellActive(x-j,y-i,active);
			}
		}
	}

	/// SUMMARY : Set Active state of the cell corresponding to the coordinates to true ou false depending on the 'active' parameters 
	/// Try catch the line that fetch the cell in the array cuz it will oftenly be a wrong coordinate if the brawler is close to a wall
	/// PARAMETERS : int pos_x : the x coordinate of the cell, int pos_y : the y coordinate of the cell, bool active : the state of Activit
	/// to be applied to the cell
	/// RETURN : Void.
	private void SetCellActive(int pos_x, int pos_y,bool active){
		try{
			SC_cell cell = _board_game._cells_gameField [pos_x, pos_y];
			cell._GO_button.SetActive (active);
		}catch (IndexOutOfRangeException exception){
			// Do nothing because we don't care, it will happens in a lot of cases (mostly when the brawler is close to a wall)
		}
	}

	private GridPosition CalculateCurrentPositionWithFutureActions(){
		if (_selected_slot == 0) {
			return _selected_brawler._position;
		}
		else if (_selected_slot == 1) {
			if(_selected_brawler._actions[0]._action_type != ActionType.Move){
				return _selected_brawler._position;
			}else{
				return CalculateNextPositionWithDirection(_selected_brawler._position, _selected_brawler._actions[0]._direction_move);
			}
		}
		else{
			if(_selected_brawler._actions[0]._action_type != ActionType.Move && _selected_brawler._actions[1]._action_type != ActionType.Move){
				return _selected_brawler._position;
			}
			else if(_selected_brawler._actions[0]._action_type != ActionType.Move){				
				return CalculateNextPositionWithDirection(_selected_brawler._position, _selected_brawler._actions[1]._direction_move);
			}
			else if(_selected_brawler._actions[1]._action_type != ActionType.Move){				
				return CalculateNextPositionWithDirection(_selected_brawler._position, _selected_brawler._actions[0]._direction_move);
			}else{
				GridPosition tempNewPosition = CalculateNextPositionWithDirection(_selected_brawler._position, _selected_brawler._actions[0]._direction_move);
				return CalculateNextPositionWithDirection(tempNewPosition, _selected_brawler._actions[1]._direction_move);
			}
		}
	}

	private GridPosition CalculateNextPositionWithDirection(GridPosition position, Direction direction){
		GridPosition new_position = position;
		switch (direction) {
			case Direction.Right:
				new_position._i_x += 1;
				return new_position;
			case Direction.Left:
				new_position._i_x -= 1;
				return new_position;
			case Direction.Up:
				new_position._i_y +=1;
				return new_position;
			case Direction.Down:
				new_position._i_y -=1;
				return new_position;
			default:
				return new_position;
		}	
	}

	private Direction DetermineMoveDirection(GridPosition selected_cell_position){
		GridPosition brawlerPositionWithNextMoves = CalculateCurrentPositionWithFutureActions ();
		// TODO remplacer brawlerPositionWithNextMoves par _selected_brawler._position si on ne dit pas que le joueur choisit ses actions 
		// dans l'ordre
		if (brawlerPositionWithNextMoves._i_x < selected_cell_position._i_x) 
		{
			return Direction.Right;
		} 
		else if (brawlerPositionWithNextMoves._i_x > selected_cell_position._i_x) 
		{
			return Direction.Left;
		} 
		else if (brawlerPositionWithNextMoves._i_y < selected_cell_position._i_y) 
		{
			return Direction.Up;
		}
		else{
			return Direction.Down;
		}
	}

	public void RegisterSelectedSlot(int selected_slot_p){
		_selected_slot = selected_slot_p;
		_manager_ui.HighlightSlot (_selected_slot);
	}

	private void AddActionToBrawlerArray(){
		for (int i = _selected_slot; i < _selected_brawler._actions.Length; i++) {
			if(_selected_brawler._actions[i]._action_type != ActionType.None){
				RemoveDisplayOnCell(_img_brawler_actions_cells[_selected_brawler._i_index_in_team,i]);
				_selected_brawler._actions[i] = new Action();
				_selected_brawler._actions[i].SetNone();
			}
		}
		_selected_brawler._actions[_selected_slot] = _selected_action;
		_number_of_chosen_actions++;		
		_manager_ui.UpdateActionsSlotForBrawler(_selected_brawler);
	}

	public void RemoveDisplayOnCell(Image cell_image){
		cell_image.color = new Color (255, 255, 255, 0);
		cell_image.sprite = null;
		//cell_image = new Image ();
	}

	public void RegisterSelectedCellPositionForAction(SC_cell selected_cell){
		_selected_cell = selected_cell;
		// TODO si il y a un bug sur le tackle, voir ici car le tackle ne passera pas par cette méthode donc la 
		// la selected_cell ne sera pas a jour
		_selected_action._position = selected_cell._position;
		_selected_action._direction_move = DetermineMoveDirection (selected_cell._position);
		//_selected_action._image_cell = selected_cell._IMG_action_display;
		Image currentImageInArray = _img_brawler_actions_cells [_selected_brawler._i_index_in_team, _selected_slot];
		if (currentImageInArray != null) {
			RemoveDisplayOnCell(currentImageInArray);
		}
		_img_brawler_actions_cells[_selected_brawler._i_index_in_team, _selected_slot] = selected_cell._IMG_action_display;
		
		// TODO optimisation en mettant juste tout les cells à false?
		// go automatically to the next slot
		SetActiveCellsForMoveAndTackle (false);
		SetActiveCellsForPass (false);
	}

	public void SwitchToNextSlot(){
		if (_selected_slot != 2) {
			_selected_slot += 1;
		} else {
			_selected_slot = 0;			
		}
		_manager_ui.HighlightSlot (_selected_slot);
	}

	public void UpdateActionSlotText(){	
		_manager_ui.SetActionSlotText (_selected_action.ToString (), _selected_slot);	
		AddActionToBrawlerArray ();	
		DisplayChosenActionsOnField ();			
		SwitchToNextSlot ();
	}

	/// SUMMARY : Register the type of action chose by the player to display the right elements ( arrow if it is a move, nothing if it
	/// is a defend, etc) but DOESN'T add it yet to the array of chosen actions
	/// PARAMETERS : The name of the action type.
	/// RETURN : Void.
	public void RegisterAction(string action_name){
		_selected_action = new Action ();
		_selected_action.SetType (action_name);
		//DisplayChosenActionsOnField ();
	}

	public void DisplayChosenActionsOnField(){
		for (int i = 0; i< _selected_brawler._actions.Length ; i++) {
			Action action = _selected_brawler._actions[i];
			Image image = _img_brawler_actions_cells[_selected_brawler._i_index_in_team,i];
			// Reset the rectTransform rotation so the rotation is not applied multiple times in a row on it, since this method 
			// is called for every action changed (needed if the player decide to change action 1 when action 2 or 3 was already defined)
			if(image != null){
				image.rectTransform.rotation = new Quaternion();				
				//action._selected_cell._IMG_action_display.rectTransform.transform = new Transform();
			}
			if (action._action_type == ActionType.Move) {
				image.sprite = _Spr_ui_arrow;
				image.color = new Color (255, 255, 255, 255);
				switch (action._direction_move) {
					case Direction.Right:				
							image.rectTransform.Rotate (new Vector3 (0, 0, 180));
							break;
					case Direction.Down:
							image.rectTransform.Rotate (new Vector3 (0, 0, 90));
							break;
					case Direction.Up:
							image.rectTransform.Rotate (new Vector3 (0, 0, -90));
							break;
					default:
							break;
				}
			}else if (action._action_type == ActionType.Tackle) {
				image.sprite = _Spr_ui_arrow;
				image.color = new Color (0, 0, 0, 255);
				switch (action._direction_move) {
				case Direction.Right:				
					image.rectTransform.Rotate (new Vector3 (0, 0, 180));
					//action._selected_cell._IMG_action_display.rectTransform.
					break;
				case Direction.Down:
					image.rectTransform.Rotate (new Vector3 (0, 0, 90));
					break;
				case Direction.Up:
					image.rectTransform.Rotate (new Vector3 (0, 0, -90));
					break;
				default:
					break;
				}
			}else if (action._action_type == ActionType.Pass) {				
					image.sprite = _Spr_ui_ball;
					image.color = new Color (255, 255, 255, 255);
			} 
		}
	}

	public void RemoveAllActionDisplay(){
		int i_width = _game_settings._i_nb_brawlers_per_team;
		int i_height = 3;		
		// TODO int i_height = _game_settings._i_nb_actions_per_turn;

		for(int i = 0; i < i_width; i++)
		{
			for(int j = 0; j < i_height; j++)
			{
				if (_img_brawler_actions_cells[i,j] != null)
					RemoveDisplayOnCell(_img_brawler_actions_cells[i,j]);
			}
		}
	}

	public void EndTurn(){
		// TODO To be fixed , these two menus should be closed here but they need a selected_brawler and it wont always be set so it can make nullrefexcept.
		//CloseMenuActionsTypes();
		//CloseMenuActionsSlots();
		_board_game.SetActiveButtonsBrawlers (false,_b_player_team);
		if (Network.isServer)
			SC_game_manager_server._instance.ServerIsReadyPlanification();
		else
			_network_view.RPC("ClientIsReadyPlanification", RPCMode.Server);
	}

}