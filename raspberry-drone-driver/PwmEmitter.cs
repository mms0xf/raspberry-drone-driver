// using System.Timers;
using System;
using Raspberry.Timers;
using System.Diagnostics;

public class PwmEmitter : IDisposable
{
	const int pulasResolution = 1000000; //	detect 10 times in one frequency

	HighResolutionTimer timer;

	Action onStarted;
	Action onStop;

	Action<bool,long> onUpdate;

	Stopwatch stopwatch;

	// input ex: connection.Pins[ConnectorPin.P1Pin11]
	public PwmEmitter( Action<bool,long> onUpdate, float dutyRate=0.5f )
	{
		stopwatch = new Stopwatch ();
		timer = new HighResolutionTimer();
		this.onUpdate = onUpdate;
		SetDutyRate ( dutyRate );
	}

	public void SetDutyRate( float dutyRate )
	{
		long oneCycleTicks = TimeSpan.FromMilliseconds(15).Ticks;
		long threshold = (long)Math.Max (0, oneCycleTicks * dutyRate);

		bool prevFlag = false;

		onStarted = () => prevFlag = false;
		onStop = () => this.onUpdate(false,stopwatch.ElapsedTicks);

		timer?.Stop();
		timer.Interval = TimeSpan.FromTicks ( oneCycleTicks / pulasResolution );
		timer.Action = () => {
			var currentInOneCycle = (stopwatch.ElapsedTicks % oneCycleTicks );
			var isOveredThewshold = ( currentInOneCycle < threshold );

			if( prevFlag != isOveredThewshold )
			{
				this.onUpdate(isOveredThewshold, stopwatch.ElapsedTicks);
				prevFlag = isOveredThewshold;
			}
//			Console.WriteLine("{0:0.00} {1} {2} {3} {4}",
//				((float)currentInOneCycle/oneCycleTicks), isOveredThewshold, threshold, currentInOneCycle, stopwatch.ElapsedTicks );
		};

	}

	public void Dispose ()
	{
		timer?.Stop ();
	}

	public void StartEmit( float dutyRate )
	{
		SetDutyRate (dutyRate);
		StartEmit();
	}

	public void StartEmit()
	{
		stopwatch.Restart ();
		timer.Start (TimeSpan.Zero);
		if (onStarted != null)
			onStarted ();
	}

	public void StopEmit()
	{
		timer.Stop ();
		if (onStop != null)
			onStop ();
	}
}

