using System.Threading.Tasks;
using System.Threading;
using System;

class TankDriver
{
	DualWheelDriver dualWheel;
	PwmEmitter pwm;
	Action<int> onTirigger;

	ThlottleChain thlottleChain;

	public int power{ get{ return thlottleChain.level; } }

	public TankDriver( DualWheelDriver dualWheel )
	{
		this.dualWheel = dualWheel;

		pwm = new PwmEmitter ( (state,count)=>{
			if( state )
				onTirigger?.Invoke( count );
			else
				dualWheel.Free();
		} );

		thlottleChain = new ThlottleChain ();
	}
		
	public void Idle()
	{
		thlottleChain.Reset ();

		pwm.StopEmit ();
		this.onTirigger = (count) => {

			if( (count%2)==0 )
				dualWheel.Accelerate();
			else
				dualWheel.ReverseAccelerate();

		};
		pwm.Reset ( 1f );
		pwm.StartEmit();
	}

	public void Accelerate()
	{
		thlottleChain.StepUp (ThlottleChain.Direction.Fore);

		pwm.StopEmit ();
		this.onTirigger = (count) => dualWheel.Accelerate ();
		pwm.Reset ( (float)thlottleChain.level/10f );
		pwm.StartEmit();
	}

	public void ReverseAccelerate()
	{
		thlottleChain.StepUp (ThlottleChain.Direction.Back);

		pwm.StopEmit ();
		this.onTirigger = ( count ) => dualWheel.ReverseAccelerate ();
		pwm.Reset ( (float)thlottleChain.level/10f );
		pwm.StartEmit();

	}

	public void Brake()
	{
		thlottleChain.Reset ();

		pwm.StopEmit ();
		dualWheel.Brake ();
	}

	public void Free()
	{
		thlottleChain.Reset ();

		pwm.StopEmit ();
		dualWheel.Free ();
	}

	public void TurnLeft()
	{
		thlottleChain.StepUp (ThlottleChain.Direction.Left);

		pwm.StopEmit ();
		this.onTirigger = ( count ) => dualWheel.TurnLeft ();
		pwm.Reset ( (float)thlottleChain.level/10f );
		pwm.StartEmit ();
	}

	public void TurnRight()
	{
		thlottleChain.StepUp (ThlottleChain.Direction.Right);

		pwm.StopEmit ();
		this.onTirigger = ( count ) => dualWheel.TurnRight ();
		pwm.Reset ( (float)thlottleChain.level/10f );
		pwm.StartEmit ();
	}

	class ThlottleChain
	{
		public enum Direction
		{
			None,
			Fore,
			Back,
			Left,
			Right,
		}

		Thlottle thlottle;
		Direction direction;

		public int level{ get{  return thlottle.level; } }

		public ThlottleChain( )
		{
			thlottle = new Thlottle();
		}

		public void Reset()
		{
			thlottle.Reset ();
			direction = Direction.None;
		}

		public void StepUp( Direction direction )
		{
			if (this.direction == direction)
				thlottle.StepUp ();
			else
				thlottle.Reset ();

			this.direction = direction;
		}

	}

	class Thlottle
	{
		public int level{ get; private set; }

		public void Reset()
		{
			level = 0;
		}

		public void StepUp()
		{
			level++;
		}

		public void StepDown()
		{
			level--;
		}
	}
}





