using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


[Serializable]
public struct GridPosition
{
	public int _i_x, _i_y;

	public static GridPosition zero { get { return new GridPosition(0, 0); } }
	public static GridPosition right { get { return new GridPosition(1, 0); } }
	public static GridPosition left { get { return new GridPosition(-1, 0); } }
	public static GridPosition up { get { return new GridPosition(0, 1); } }
	public static GridPosition down { get { return new GridPosition(0, -1); } }

	public GridPosition(int i_x, int i_y)
	{
		_i_x = i_x;
		_i_y = i_y;
	}

	public Vector3 GetWorldPosition()
	{
		return new Vector3(_i_x, _i_y, 0);
	}

	public Vector2 ToVector2()
	{
		return new Vector2(_i_x, _i_y);
	}

	public static GridPosition DirectionToGridPosition(Direction direction)
	{
		switch(direction)
		{
		case Direction.Right:
			return right;
		case Direction.Left:
			return left;
		case Direction.Up:
			return up;
		case Direction.Down:
			return down;
		default :
			return zero;
		}
	}

	public static GridPosition operator+(GridPosition position_a, GridPosition position_b)
	{
		return new GridPosition(position_a._i_x + position_b._i_x, position_a._i_y + position_b._i_y);
	}

	public static GridPosition operator-(GridPosition position_a, GridPosition position_b)
	{
		return new GridPosition(position_a._i_x - position_b._i_x, position_a._i_y - position_b._i_y);
	}

	public static bool operator==(GridPosition position_a, GridPosition position_b)
	{
		if (position_a._i_x == position_b._i_x && position_a._i_y == position_b._i_y)
			return true;
		else
			return false;
	}

	public static bool operator!=(GridPosition position_a, GridPosition position_b)
	{
		if (position_a._i_x == position_b._i_x && position_a._i_y == position_b._i_y)
			return false;
		else
			return true;
	}
}


[Serializable]
public struct Action
{
	public ActionType _action_type { get; private set; }

	public Direction _direction_move {get; set;}
	public GridPosition _position {get; set;}


	public void SetPosition(GridPosition position)
	{
		_position = position;
	}

	public void SetNone()
	{
		_action_type = ActionType.None;
	}

	public void SetType(string action_name){
		switch (action_name) {
			case "move":
				_action_type = ActionType.Move;
				break;
			case "pass":
				_action_type = ActionType.Pass;
				break;
			case "tackle":
				_action_type = ActionType.Tackle;
				break;
			case "defense":
				_action_type = ActionType.Defense;
				break;
			default:
				_action_type = ActionType.None;
				break;
		}
	}

	public override string ToString(){
		switch (this._action_type) {

			case ActionType.Move:
				return "Move : " + _direction_move.ToString();
			case ActionType.Pass:
				return "Pass";
			case ActionType.Tackle:
				return "Tackle : " + _direction_move.ToString();
			case ActionType.Defense:
				return "Defense";	
			case ActionType.None:
				return "None";
			default:
				return "None";
		}
	}
}


[Serializable]
public struct SimulationResult
{
	public BallSimulationResult _ball_simulation_result { get; private set; }
	public BrawlerSimulationResult[] _brawlers_simulation_result { get; private set; }

	public bool _b_is_goal;
	public bool _b_team_who_scores;

	public SimulationResult(BallSimulationResult ball_simulation_result, BrawlerSimulationResult[] brawler_simulation_result, bool b_is_goal, bool b_team_who_scores)
	{
		_ball_simulation_result = ball_simulation_result;
		_brawlers_simulation_result = brawler_simulation_result;
		_b_is_goal = b_is_goal;
		_b_team_who_scores = b_team_who_scores;
	}
}


[Serializable]
public struct BrawlerSimulationResult
{
	public ActionType _action_type { get; private set; }
	public GridPosition _position_target { get; private set; }
	public bool _b_is_KO { get; private set; }

	public BrawlerSimulationResult(ActionType action_type, GridPosition position_target, bool b_is_KO)
	{
		_action_type = action_type;
		_position_target = position_target;
		_b_is_KO = b_is_KO;
	}
}


[Serializable]
public struct BallSimulationResult
{
	public BallStatus _ball_status { get; private set; }
	public int _i_brawler_with_the_ball { get; private set; }
	public GridPosition _position_on_ground { get; private set; }

	public BallSimulationResult(BallStatus ball_status, int i_brawler_with_the_ball, GridPosition position_on_ground)
	{
		_ball_status = ball_status;
		_i_brawler_with_the_ball = i_brawler_with_the_ball;
		_position_on_ground = position_on_ground;
	}
}


[Serializable]
public struct GameSnap
{
	public int _i_score_team_true;
	public int _i_score_team_false;

	// Brawlers infos
	public BrawlerSnap[] _brawlers;

	// Ball infos
	public BallStatus _ball_status;
	public int _i_brawler_with_the_ball;
	public GridPosition _cell_with_the_ball;
}


[Serializable]
public struct BrawlerSnap
{
	public GridPosition _position;
	public bool _b_is_KO;
	public int _i_KO_round_remaining;
}


[Serializable]
public struct Replay
{
	public GameSettings _game_settings;
	public GameSnap _start_game_snap;
	public SimulationResult[][] _record;

	public Replay(GameSettings game_settings, GameSnap start_game_snap, SimulationResult[][] record)
	{
		_game_settings = game_settings;
		_start_game_snap = start_game_snap;
		_record = record;
	}
}
