using System.Timers;
using System;

public class PwmEmitter : IDisposable
{
	const int pulasResolution = 10; //	detect 10 times in one frequency

	Timer timer;

	Action onStart;
	Action onStop;

	Action<bool,int> onUpdate;

	// input ex: connection.Pins[ConnectorPin.P1Pin11]
	public PwmEmitter( Action<bool,int> onUpdate, float dutyRate=1f )
	{
		this.onUpdate = onUpdate;
		Reset ( dutyRate );
	}

	public void Reset( float dutyRate )
	{
		int threshold = (int)Math.Max (0f, dutyRate * pulasResolution);
		int elapsedCount = 0;

		onStart = () => elapsedCount = 0;
		onStop = () => this.onUpdate(false,elapsedCount);

		// pulased
		//			timer = new Timer ( 1 );	// 1000Hz
		timer?.Dispose();
		timer = new Timer ( 5 );	// 1000 = 1Hz
		timer.Elapsed += (object sender, ElapsedEventArgs e) => {
			if( elapsedCount < threshold )
				this.onUpdate(true,elapsedCount);
			else
				this.onUpdate(false,elapsedCount);
			elapsedCount++;

			if( elapsedCount >= pulasResolution )
				elapsedCount=0;
		};

	}

	public void Dispose ()
	{
		timer.Dispose ();
	}

	public void StartEmit()
	{
		timer.Start ();
		if (onStart != null)
			onStart ();
	}

	public void StopEmit()
	{
		timer.Stop ();
		if (onStop != null)
			onStop ();
	}
}

