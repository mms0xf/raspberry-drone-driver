using System;

public class DualWheelPwmDriver : DualWheelDriver
{

	PwmEmitter leftPwm;
	PwmEmitter rightPwm;
	Action<int> onEmitLeft;
	Action<int> onEmitRight;

	public DualWheelPwmDriver( WheelDriver left, WheelDriver right ) : base( left, right )
	{
		leftPwm = new PwmEmitter ( (state,count)=>{
			if( state )
				onEmitLeft?.Invoke( count );
			else
				base.left.Free();
		} );

		rightPwm = new PwmEmitter ( (state,count)=>{
			if( state )
				onEmitRight?.Invoke( count );
			else
				base.right.Free();
		} );
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


	/*
	public void ReverseAccelerate( float leftDuty, float rightDuty )
	{

		leftPwm.StopEmit ();
		this.onEmitLeft = (count) => base.left.ReverseAccelerate ();
		leftPwm.StartEmit ( leftDuty );

		rightPwm.StopEmit ();
		this.onEmitRight = (count) => base.right.ReverseAccelerate ();
		rightPwm.StartEmit ( rightDuty );

	}
	*/
	public void TurnLeft( float dutyRate )
	{
		leftPwm.StopEmit ();
		this.onEmitLeft = (count) => base.left.ReverseAccelerate ();
		leftPwm.StartEmit ( dutyRate );

		rightPwm.StopEmit ();
		this.onEmitRight = (count) => base.right.Accelerate ();
		rightPwm.StartEmit ( dutyRate );
	}

	public void TurnRight( float dutyRate )
	{
		leftPwm.StopEmit ();
		this.onEmitLeft = (count) => base.left.Accelerate ();
		leftPwm.StartEmit ( dutyRate );

		rightPwm.StopEmit ();
		this.onEmitRight = (count) => base.right.ReverseAccelerate ();
		rightPwm.StartEmit ( dutyRate );
	}

}

