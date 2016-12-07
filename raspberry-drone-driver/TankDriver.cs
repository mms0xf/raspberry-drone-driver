using System.Threading.Tasks;
using System.Threading;
using System;

class TankDriver
{
	DualWheelPwmDriver dualWheel;
	Thlottle leftThlottle;
	Thlottle rightThlottle;

	public int power{ get{ return leftThlottle.level; } }

	public TankDriver( DualWheelPwmDriver dualWheel )
	{
		this.dualWheel = dualWheel;
		leftThlottle = new Thlottle ();
		rightThlottle = new Thlottle ();
	}

	public void Accelerate()
	{
		leftThlottle.StepUp ();
		rightThlottle.StepUp ();

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );
	}

	public void ReverseAccelerate()
	{
		leftThlottle.StepDown ();
		rightThlottle.StepDown ();

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );
	}

	public void Brake()
	{
		leftThlottle.Reset ();
		rightThlottle.Reset ();
		dualWheel.Brake ();
	}


	void Log()
	{
		var left = leftThlottle.level;
		var right = rightThlottle.level;

		Console.WriteLine ("{0} / {1}", left.ToString (), right.ToString ());
	}


	public void TurnLeft()
	{
		leftThlottle.StepDown ();
		rightThlottle.StepUp ();

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );	
	}

	public void TurnRight()
	{
		leftThlottle.StepUp ();
		rightThlottle.StepDown ();

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );
	}

	class ThlottleChain
	{
		public enum Direction
		{
			None,
			Fore,
			Back,
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

		public void StepDown( Direction direction )
		{
			if (this.direction == direction)
				thlottle.StepDown ();
			else
				thlottle.Reset ();

			this.direction = direction;
		}

	}

	class Thlottle
	{
		public int level{ get; private set; }
		public float rate{ get{ return (float)level / 10f;} }

		public void Reset()
		{
			level = 0;
		}

		public void StepUp()
		{
			if (level > 10)
				return;
			level++;
		}

		public void StepDown()
		{
			if (level < -10)
				return;
			level--;
		}
	}
}


