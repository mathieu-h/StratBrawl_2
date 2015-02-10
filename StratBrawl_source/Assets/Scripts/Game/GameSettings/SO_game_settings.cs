using UnityEngine;
using System.Collections;
using System;

public class SO_game_settings : ScriptableObject {

	public string _s_settings_name;

	public GameSettings _settings;
}

[Serializable]
public class GameSettings {

	[SerializeField]
	private int i_game_field_width = 12;
	public int _i_game_field_width { get{ return i_game_field_width; } }

	[SerializeField]
	private int i_game_field_height = 7;
	public int _i_game_field_height { get{ return i_game_field_height; } }

	[SerializeField]
	private int i_nb_turn_max = 20;
	public int _i_nb_turn_max { get{ return i_nb_turn_max; } }

	[SerializeField]
	private int i_nb_score_max = 5;
	public int _i_nb_score_max { get{ return i_nb_score_max; } }

	[SerializeField]
	private int i_planification_time = 60;
	public int _i_planification_time { get{ return i_planification_time; } }

	[SerializeField]
	private int i_nb_brawlers_per_team = 5;
	public int _i_nb_brawlers_per_team { get{ return i_nb_brawlers_per_team; } }

	[SerializeField]
	private GridPosition[] positions_brawlers_attack_formation = new GridPosition[] {
																		new GridPosition(5,3),
																		new GridPosition(4,1),
																		new GridPosition(4,5),
																		new GridPosition(2,2),
																		new GridPosition(2,4)};
	public GridPosition[] _positions_brawlers_attack_formation { get{ return positions_brawlers_attack_formation; } }

	[SerializeField]
	private GridPosition[] positions_brawlers_defense_formation = new GridPosition[] {
																		new GridPosition(1,3),
																		new GridPosition(2,1),
																		new GridPosition(2,5),
																		new GridPosition(4,2),
																		new GridPosition(4,4)};
	public GridPosition[] _positions_brawlers_defense_formation { get{ return positions_brawlers_defense_formation; } }
}
