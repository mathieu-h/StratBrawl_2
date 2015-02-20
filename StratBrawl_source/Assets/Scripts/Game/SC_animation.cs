using UnityEngine;
using System.Collections;

public class SC_animation : MonoBehaviour {

	private Transform _T_object;


	void Awake()
	{
		_T_object = transform;
	}

	/// SUMMARY : Play brawler animation.
	/// PARAMETERS : ActionType of the brawler to select the animation, the position target of the animation, and the duration of th animation.
	/// RETURN : Void.
	public IEnumerator PlayAnimation(ActionType action_type, GridPosition position_target, float f_duration)
	{
		switch (action_type)
		{
		case ActionType.Move:
			StartCoroutine(RotationInterpolation(position_target.GetWorldPosition() - _T_object.position, 0.3f));
			yield return StartCoroutine(Interpolation(position_target.GetWorldPosition() + Vector3.back, f_duration));
			break;
		case ActionType.Tackle:
			Vector3 V3_position_start = _T_object.position;
			yield return StartCoroutine(RotationInterpolation(position_target.GetWorldPosition() - _T_object.position, 0.3f));
			yield return StartCoroutine(HalfInterpolation(position_target.GetWorldPosition() + Vector3.back, f_duration * 0.2f));
			yield return StartCoroutine(Interpolation(V3_position_start, f_duration * 0.2f));
			break;
		}
	}

	/// SUMMARY : Interolate the transfrom to the target position.
	/// PARAMETERS : The target position and the duration of the interpolation.
	/// RETURN : Void.
	public IEnumerator Interpolation(Vector3 V3_position_target, float f_duration)
	{
		Vector3 V3_position_start = _T_object.position;
		for (float f_time = 0; f_time < f_duration; f_time += Time.deltaTime)
		{
			yield return null;
			_T_object.position = Vector3.Lerp(V3_position_start, V3_position_target, f_time / f_duration);
		}
		_T_object.position = V3_position_target;
	}

	/// SUMMARY : Interolate the transfrom to the target position.
	/// PARAMETERS : The target position and the duration of the interpolation.
	/// RETURN : Void.
	public IEnumerator HalfInterpolation(Vector3 V3_position_target, float f_duration)
	{
		Vector3 V3_position_start = _T_object.position;
		for (float f_time = 0; f_time < f_duration; f_time += Time.deltaTime)
		{
			yield return null;
			_T_object.position = Vector3.Lerp(V3_position_start, V3_position_target, (f_time / f_duration) *  0.5f);
		}
		_T_object.position = Vector3.Lerp(V3_position_start, V3_position_target, 0.5f);
	}

	/// SUMMARY : Interolate the transfrom to the target transform.
	/// PARAMETERS : The target transform and the duration of the interpolation.
	/// RETURN : Void.
	public IEnumerator InterpolationOnMovingTagret(Transform T_target, float f_duration)
	{
		Vector3 V3_position_start = _T_object.position;
		for (float f_time = 0; f_time < f_duration; f_time += Time.deltaTime)
		{
			yield return null;
			_T_object.position = Vector3.Lerp(V3_position_start, T_target.position, f_time / f_duration);
		}
		_T_object.position = T_target.position;
	}

	public IEnumerator RotationInterpolation(Vector3 V3_look_target, float f_duration)
	{
		float f_angle_z_start = _T_object.eulerAngles.z;
		float f_angle_z_target;
		if (Mathf.Abs(V3_look_target.x) > Mathf.Abs(V3_look_target.y))
		{
			if (V3_look_target.x > 0)
				f_angle_z_target = 90;
			else 
				f_angle_z_target = 270;

		}
		else
		{
			if (V3_look_target.y > 0)
				f_angle_z_target = 180;
			else 
				f_angle_z_target = 0;
		}
		if (f_angle_z_start == 0 && f_angle_z_target == 270)
			f_angle_z_start = 360;
		else if (f_angle_z_start == 270 && f_angle_z_target == 0)
			f_angle_z_target = 360;

		for (float f_time = 0; f_time < f_duration; f_time += Time.deltaTime)
		{
			yield return null;
			float f_angle_z = Mathf.Lerp(f_angle_z_start, f_angle_z_target, f_time / f_duration);
			_T_object.eulerAngles = new Vector3(0, 0, f_angle_z);
		}
		_T_object.eulerAngles = new Vector3(0, 0, f_angle_z_target);


	}
}
