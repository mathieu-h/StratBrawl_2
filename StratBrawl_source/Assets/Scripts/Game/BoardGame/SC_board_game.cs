using UnityEngine;
using System.Collections;

public partial class SC_board_game : MonoBehaviour {

	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private Transform _T_camera;

	[SerializeField]
	private SC_ball _ball;

	private GameSettings _game_settings;


	public void Init(GameSettings game_settings)
	{
		_game_settings = game_settings;
		GenerateGameField(game_settings._i_game_field_width, game_settings._i_game_field_height);
		GenerateBrawlers(game_settings._i_nb_brawlers_per_team);
		_ball.Init();
	}


	/// SUMMARY : Initialize brawlers and ball for engage.
	/// PARAMETERS : The team who have the ball.
	/// RETURN : Void.
	private void SetEngagePosition(bool b_team_with_ball)
	{
		SetBrawlersEngagePositions(_game_settings._positions_brawlers_attack_formation, _game_settings._positions_brawlers_defense_formation, b_team_with_ball);
		_ball.SetBrawlerWithTheBall(GetTeamCaptain(b_team_with_ball));
	}
	
	
	/// SUMMARY : 
	/// PARAMETERS : 
	/// RETURN : Void.
	public void SetGameFromSnap(GameSnap game_snap)
	{	
		for (int i = 0; i < _brawlers.Length; ++i)
		{
			_brawlers[i].SetPosition(game_snap._brawlers[i]._position);
			_brawlers[i]._b_is_KO = game_snap._brawlers[i]._b_is_KO;
			_brawlers[i]._i_KO_round_remaining = game_snap._brawlers[i]._i_KO_round_remaining;
		}
		
		_ball._ball_status = game_snap._ball_status;
		
		if (_ball._ball_status == BallStatus.OnBrawler)
			_ball.SetBrawlerWithTheBall(_brawlers[game_snap._i_brawler_with_the_ball]);
		
		if (_ball._ball_status == BallStatus.OnGround)
			_ball.SetBallOnTheCell(_cells_gameField[game_snap._cell_with_the_ball._i_x, game_snap._cell_with_the_ball._i_y]);
	}
}
