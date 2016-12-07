using System;
using System.Threading.Tasks;


public class DualWheelDriver
{
	protected WheelDriver left;
	protected WheelDriver right;

	public DualWheelDriver ( WheelDriver left, WheelDriver right )
	{
		this.left = left;
		this.right = right;
	}

	public virtual void Accelerate()
	{
		left.Accelerate ();
		right.Accelerate ();
	}

	public virtual void ReverseAccelerate()
	{
		left.ReverseAccelerate ();
		right.ReverseAccelerate ();
	}

	public virtual void Brake()
	{
		left.Brake ();
		right.Brake ();
	}

	public virtual void Free()
	{
		left.Free ();
		right.Free ();
	}

	public virtual void TurnLeft()
	{
		left.ReverseAccelerate ();
		right.Accelerate ();
	}

	public virtual void TurnRight()
	{
		left.Accelerate ();
		right.ReverseAccelerate ();
	}
}



