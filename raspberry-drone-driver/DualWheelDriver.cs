using System;
using System.Threading.Tasks;


public class DualWheelDriver
{
	WheelDriver left;
	WheelDriver right;

	public DualWheelDriver ( WheelDriver left, WheelDriver right )
	{
		this.left = left;
		this.right = right;
	}

	public void Accelerate()
	{
		left.Accelerate ();
		right.Accelerate ();
	}

	public void ReverseAccelerate()
	{
		left.ReverseAccelerate ();
		right.ReverseAccelerate ();
	}

	public void Brake()
	{
		left.Brake ();
		right.Brake ();
	}

	public void Free()
	{
		left.Free ();
		right.Free ();
	}

	public void TurnLeft()
	{
		left.ReverseAccelerate ();
		right.Accelerate ();
	}

	public void TurnRight()
	{
		left.Accelerate ();
		right.ReverseAccelerate ();
	}
}



