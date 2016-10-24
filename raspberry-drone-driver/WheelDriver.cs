using Raspberry.IO.GeneralPurpose;

public class WheelDriver
{
	ConnectedPin phasePin;
	ConnectedPin enablePin;

	public bool isAccelerate{ get{ return ( phasePin.Enabled && !enablePin.Enabled ); } }
	public bool isReverseAccelerate{ get{ return ( !phasePin.Enabled && enablePin.Enabled ); } }

	public WheelDriver( ConnectedPin phasePin, ConnectedPin enablePin )
	{
		this.phasePin = phasePin;
		this.enablePin = enablePin;
	}

	public void Free()
	{
		phasePin.Enabled = false;
		enablePin.Enabled = false;
	}

	public void Accelerate()
	{
		phasePin.Enabled = true;
		enablePin.Enabled = false;
	}

	public void ReverseAccelerate()
	{
		phasePin.Enabled = false;
		enablePin.Enabled = true;
	}

	public void Brake()
	{
		phasePin.Enabled = true;
		enablePin.Enabled = true;
	}
}

