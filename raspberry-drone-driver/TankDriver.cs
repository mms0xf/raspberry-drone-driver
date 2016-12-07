using System.Threading.Tasks;
using System.Threading;
using System;

class TankDriver
{
	DualWheelPwmDriver dualWheel;
	Thlottle leftThlottle;
	Thlottle rightThlottle;

	public string powerText{ get{ return string.Format( "{0}:{1}", leftThlottle.level, rightThlottle.level); } }

	bool isFore{ get{ return (leftThlottle.level + rightThlottle.level) > 0; } }
	bool isBack{ get{ return (leftThlottle.level + rightThlottle.level) < 0; } }
	bool isLeft{ get{ return (leftThlottle.level < rightThlottle.level); } }
	bool isRight{ get{ return (leftThlottle.level > rightThlottle.level); } }
	int thlottleAverage{ get{ return (int)((leftThlottle.level + rightThlottle.level) / 2f); } }


	public TankDriver( DualWheelPwmDriver dualWheel )
	{
		this.dualWheel = dualWheel;
		leftThlottle = new Thlottle ();
		rightThlottle = new Thlottle ();
	}

	public void Brake()
	{
		leftThlottle.Reset ();
		rightThlottle.Reset ();
		dualWheel.Brake ();
	}

	public void Accelerate()
	{
		if (isBack) {
			Brake ();
			return;
		}

		leftThlottle.StepUp ();
		rightThlottle.StepUp ();

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );
	}

	public void ReverseAccelerate()
	{
		if (isFore) {
			Brake ();
			return;
		}

		leftThlottle.StepDown ();
		rightThlottle.StepDown ();

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );
	}

	public void TurnLeft()
	{
		if (isRight) {
			leftThlottle.level = thlottleAverage;
			rightThlottle.level = thlottleAverage;
		} else {
			leftThlottle.StepDown ();
			rightThlottle.StepUp ();
		}

		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );	
	}

	public void TurnRight()
	{
		if (isLeft) {
			leftThlottle.level = thlottleAverage;
			rightThlottle.level = thlottleAverage;
		} else {
			leftThlottle.StepUp ();
			rightThlottle.StepDown ();
		}
		dualWheel.Accelerate ( leftThlottle.rate, rightThlottle.rate );
	}

	void Log()
	{
		var left = leftThlottle.level;
		var right = rightThlottle.level;

		Console.WriteLine ("{0} / {1}", left.ToString (), right.ToString ());
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
		int _level;
		public int level{ 
			get{ return _level; } 
			set{ 
				if (IsLimit (value))	return;
				_level = value;
			}
		}
		public float rate{ get{ return (float)level / 10f;} }

		public void Reset()
		{
			level = 0;
		}

		bool IsLimit( int amount )
		{
			if (amount > 10)
				return true;

			if (amount < -10)
				return true;

			return false;
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


