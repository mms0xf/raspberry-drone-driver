using System;

public class YawPitchSurvo : IDisposable
{
	const float minDuty = 0.05f;
	const float maxDuty = 0.2f;
	const float normalDuty = 0.1f;

	PwmEmitter yawSurvoEmitter;
	PwmEmitter pitchSurvoEmitter;

	float _currentYawDuty = normalDuty;
	float currentYawDuty{ 
		get{ return _currentYawDuty; } 
		set{ 
			_currentYawDuty = value;
			if (_currentYawDuty > maxDuty)
				_currentYawDuty = maxDuty;
			if (_currentYawDuty < minDuty)
				_currentYawDuty = minDuty;
		}
	}

	float _currentPitchDuty = normalDuty;
	float currentPitchDuty{ 
		get{ return _currentPitchDuty; } 
		set{ 
			_currentPitchDuty = value;
			if (currentPitchDuty > maxDuty)
				currentPitchDuty = maxDuty;
			if (currentPitchDuty < minDuty)
				currentPitchDuty = minDuty;
		}
	}

	public YawPitchSurvo (	Action<bool,long> onYawEmit, Action<bool,long> onPitchEmit )
	{
		yawSurvoEmitter = new PwmEmitter (onYawEmit);
		pitchSurvoEmitter = new PwmEmitter (onPitchEmit);

//		yawLarp = new FloatLerp( duty => Yaw(duty) );
//		pitchLerp = new FloatLerp( duty => Pitch(duty) );
	}

	public void Dispose()
	{
		yawSurvoEmitter.Dispose();
		pitchSurvoEmitter.Dispose();
	}

	#region core methods
	void Yaw( float duty )
	{
		currentYawDuty = duty;
		yawSurvoEmitter.StartEmit (currentYawDuty);
	}

	void Pitch( float duty )
	{
		currentPitchDuty = duty;
		pitchSurvoEmitter.StartEmit (currentPitchDuty);
	}
	#endregion


	#region fixed control
	public void FullYawRight()
	{
		Yaw( minDuty );	// clockwise 90deg
	}

	public void NormalizeYaw()
	{
		Yaw (normalDuty);
	}

	public void FullYawLeft()
	{
		Yaw( maxDuty );	// anticlockwise 90deg
	}


	public void FullPitchUp()
	{
		pitchSurvoEmitter.StartEmit( minDuty );	// clockwise 90deg
	}

	public void NromalizePitch()
	{
		Pitch (normalDuty);
	}

	public void FullPitchDown()
	{
		Pitch( maxDuty );	// anticlockwise 90deg
	}

	public void Fold()
	{
		FullPitchDown ();
		NormalizeYaw ();
	}

	#endregion

	#region step control
	const float stepDuty = 0.02f;

	public void StepYawLeft( bool isSmooth )
	{
		if (isSmooth)
			yawLarp.SetTarget ( currentYawDuty += stepDuty );
		else
			Yaw (currentYawDuty += stepDuty);
	}

	public void StepYawRight(bool isSmooth)
	{
		if( isSmooth )
			yawLarp.SetTarget(currentYawDuty -= stepDuty);
		else		
			Yaw (currentYawDuty -= stepDuty);
	}

	public void StepPitchUp(bool isSmooth)
	{
		if( isSmooth )
			pitchLerp.SetTarget(currentPitchDuty -= stepDuty);
		else
			Pitch (currentPitchDuty -= stepDuty);
	}

	public void StepPitchDown(bool isSmooth)
	{
		if( isSmooth )
			pitchLerp.SetTarget(currentPitchDuty += stepDuty);
		else
			Pitch (currentPitchDuty += stepDuty);
	}
	#endregion

	FloatLerp yawLarp;
	FloatLerp pitchLerp;
	public void Update()
	{
//		yawLarp.Update();
//		pitchLerp.Update();

	}

}



public class FloatLerp
{	
	float target;
	float current{ get; set; }

	Action<float> onCurrentChanged;

	public FloatLerp(Action<float> onCurrentChanged)
	{
		this.onCurrentChanged = onCurrentChanged;
	}

	public void SetTarget( float target )
	{
		this.target = target;
	}

	public void Update()
	{
		float rawDiff = target - current;

		if (rawDiff == 0f)
			return;

		Console.WriteLine ( "hit!" );

		float diff = Math.Abs (rawDiff);
		float direction = diff / rawDiff;	// -1 or 1 or 0 div

		Console.WriteLine ( string.Format("diff {0} / rawDiff {1}",diff.ToString(), rawDiff.ToString()) );

		if( diff > 0.01f )
		{
			current = diff/2 * direction;
			onCurrentChanged (current);
		}

	}
}


