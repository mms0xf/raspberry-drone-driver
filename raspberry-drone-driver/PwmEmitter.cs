// using System.Timers;
using System;
using Raspberry.Timers;
using System.Diagnostics;

public class PwmEmitter : IDisposable
{
	public class Setting
	{
		public int pulasResolution = 1000000;//	detect times in one frequency
		public float dutyRate=0.5f;
		public long oneCycleTicks;

		public long thresholdTicks {
			get{ return (long)Math.Max (0, oneCycleTicks * dutyRate); } 
		}

		public TimeSpan intervalSpan {
			get{ return TimeSpan.FromTicks (oneCycleTicks / pulasResolution); }
		}
			
		public Setting(  )
		{
			oneCycleTicks = TimeSpan.FromMilliseconds(15).Ticks;
		}
	}

	public Setting setting{ get; private set; }

//	const int pulasResolution = 1000000; //	detect times in one frequency

	HighResolutionTimer timer;

	Action onStarted;
	Action onStop;

	Action<bool,long> onUpdate;

	Stopwatch stopwatch;

	// input ex: connection.Pins[ConnectorPin.P1Pin11]
	public PwmEmitter( Action<bool,long> onUpdate, Setting setting=null )
	{
		this.stopwatch = new Stopwatch ();
		this.timer = new HighResolutionTimer();

		this.setting = setting ?? new Setting ();

		this.onUpdate = onUpdate;
		SetDutyRate ( 0.5f );
	}

	public void SetDutyRate( float dutyRate )
	{
		setting.dutyRate = dutyRate;

		bool prevFlag = false;

		onStarted = () => prevFlag = false;
		onStop = () => this.onUpdate(false,stopwatch.ElapsedTicks);

		timer?.Stop();
		timer.Interval = setting.intervalSpan;
		timer.Action = () => {
			var currentInOneCycle = (stopwatch.ElapsedTicks % setting.oneCycleTicks );
			var isOveredThewshold = ( currentInOneCycle < setting.thresholdTicks );

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

