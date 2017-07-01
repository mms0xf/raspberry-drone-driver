using System;

public class DualWheelPwmDriver : DualWheelDriver
{

	PwmEmitter leftPwm;
	PwmEmitter rightPwm;
	Action<long> onEmitLeft;
	Action<long> onEmitRight;

	public DualWheelPwmDriver( WheelDriver left, WheelDriver right ) : base( left, right )
	{
		var settingLeft = new PwmEmitter.Setting ();
		settingLeft.oneCycleTicks = TimeSpan.FromSeconds (1).Ticks;
		settingLeft.pulasResolution = 10;

		var settingRight = new PwmEmitter.Setting ();
		settingRight.oneCycleTicks = TimeSpan.FromSeconds (1).Ticks;
		settingRight.pulasResolution = 10;

		leftPwm = new PwmEmitter ( (state,count)=>{
			if( state )
				onEmitLeft?.Invoke( count );
			else
				base.left.Free();
		} ,settingLeft);

		rightPwm = new PwmEmitter ( (state,count)=>{
			if( state )
				onEmitRight?.Invoke( count );
			else
				base.right.Free();
		} ,settingRight);
	}

	public override void Brake ()
	{
		leftPwm.StopEmit ();
		rightPwm.StopEmit ();
	}

	public void Accelerate( float leftDuty, float rightDuty )
	{
		AccelerateLeft( leftDuty );
		AccelerateRight( rightDuty );
	}

	void AccelerateLeft( float leftDuty )
	{
		leftPwm.StopEmit ();
		if( leftDuty > 0 )
			this.onEmitLeft = (count) => base.left.Accelerate ();
		else
			this.onEmitLeft = (count) => base.left.ReverseAccelerate();
		leftPwm.StartEmit ( Math.Abs(leftDuty) );
	}

	void AccelerateRight( float rightDuty )
	{
		rightPwm.StopEmit ();
		if( rightDuty > 0 )
			this.onEmitRight = (count) => base.right.Accelerate ();
		else
			this.onEmitRight = (count) => base.right.ReverseAccelerate ();
		rightPwm.StartEmit ( Math.Abs(rightDuty) );
	}


}

