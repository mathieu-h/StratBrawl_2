using UnityEngine;
using System.Collections;

public class SC_ball : MonoBehaviour {

	private Transform _T_ball;
	public SC_animation _animation;
	public BallStatus _ball_status;
	public SC_brawler _brawler_with_the_ball { get; private set; }
	public SC_cell _cell_with_the_ball { get; private set; }
	public GridPosition _position
	{
		get 
		{
			if (_brawler_with_the_ball != null)
				return _brawler_with_the_ball._position;
			else if (_cell_with_the_ball != null)
				return _cell_with_the_ball._position;
			else
				return new GridPosition(-1, -1);
		}
	}


	/// SUMMARY : Initialize the ball.
	/// PARAMETERS : None.
	/// RETURN : Void.
	public void Init()
	{
		_T_ball = transform;
		_ball_status = BallStatus.Null;
	}

	/// SUMMARY : Set the brawler who have the ball.
	/// PARAMETERS : The brawler who have the ball.
	/// RETURN : Void.
	public void SetBrawlerWithTheBall(SC_brawler _brawler)
	{
		ResetOwner();

		_ball_status = BallStatus.OnBrawler;
		_brawler_with_the_ball = _brawler;
		_brawler_with_the_ball._b_have_the_ball = true;
		_T_ball.parent = _brawler._T_brawler;
		if (_brawler._b_team)
			_brawler._T_brawler.renderer.sharedMaterial = _brawler._Mat_team_true_ball;
		else
			_brawler._T_brawler.renderer.sharedMaterial = _brawler._Mat_team_false_ball;

		if (_brawler._b_is_KO)
			renderer.material.color = new Color(0.4f, 0.4f, 0.4f, 1f);

		_T_ball.localPosition = new Vector3(0f, -0.35f, -1f);
	}

	/// SUMMARY : Set the cell where the ball is.
	/// PARAMETERS : The cell.
	/// RETURN : Void.
	public void SetBallOnTheCell(SC_cell _cell)
	{
		ResetOwner();
		
		_ball_status = BallStatus.OnGround;
		_cell_with_the_ball = _cell;
		_cell_with_the_ball._b_is_ball_on_this_cell = true;
		_T_ball.parent = _cell._T_cell;
		_T_ball.localPosition = new Vector3(0f, 0f, -2f);
	}

	/// SUMMARY : Set the brawler who have the ball or the cell where is the ball to null.
	/// PARAMETERS : None.
	/// RETURN : Void.
	public void ResetOwner()
	{
		if (_brawler_with_the_ball != null)
		{
			if (_brawler_with_the_ball._b_team)
				_brawler_with_the_ball._T_brawler.renderer.sharedMaterial = _brawler_with_the_ball._Mat_team_true;
			else
				_brawler_with_the_ball._T_brawler.renderer.sharedMaterial = _brawler_with_the_ball._Mat_team_false;

			if (_brawler_with_the_ball._b_is_KO)
				renderer.material.color = new Color(0.4f, 0.4f, 0.4f, 1f);

			_brawler_with_the_ball._b_have_the_ball = false;
			_brawler_with_the_ball = null;
		}
		if (_cell_with_the_ball != null)
		{
			_cell_with_the_ball._b_is_ball_on_this_cell = false;
			_cell_with_the_ball = null;
		}
		_ball_status = BallStatus.Null;
		_T_ball.parent = null;
	}
}
