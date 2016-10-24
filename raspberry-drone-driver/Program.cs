using System;
using Raspberry.IO.GeneralPurpose;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;


namespace RaspberryDroneDriver
{
	

	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello io!" );

//			TestServer ();


			TestGpio ();

		}

		static async void TestServer(  )
		{
			var listener = new TcpListener ( IPAddress.Any, 80 );
			listener.Start ();

			while (true)
			{
				var client = await listener.AcceptTcpClientAsync ();

				Task.Run (async() => {

					using (var stream = client.GetStream ())
					using (var reader = new StreamReader (stream))
					using (var writer = new StreamWriter (stream)) 
					{
						List<string> requestHeadder = new List<string> ();

						while (true) {
							var line = await reader.ReadLineAsync ();
							if (String.IsNullOrWhiteSpace (line))
								break;
							requestHeadder.Add (line);
						}

						var requestLine = requestHeadder.FirstOrDefault ();
						var requestParts = requestLine?.Split (new[]{ ' ' }, 3);
						if (!requestHeadder.Any () || requestParts.Length != 3) {
							await writer.WriteLineAsync ("HTTP/1.0 400 Bad Request");
							await writer.WriteLineAsync ("Content-Type: text/plain; charset=UTF-8");
							await writer.WriteLineAsync ();
							await writer.WriteLineAsync ("Bad request");
							return;
						}

						Console.WriteLine (client.Client.RemoteEndPoint);

						var path = requestParts [1];
						if (path == "/") {
							await writer.WriteLineAsync ("HTTP/1.0 200 OK");
							await writer.WriteLineAsync ("Content-Type: text/plain; charset=UTF-8");
							await writer.WriteLineAsync ();
							await writer.WriteLineAsync ("here is root");
						} else if (path == "/test") {
							await writer.WriteLineAsync ("HTTP/1.0 200 OK");
							await writer.WriteLineAsync ("Content-Type: text/plain; charset=UTF-8");
							await writer.WriteLineAsync ();
							await writer.WriteLineAsync ("here is test");
						}
					} 

				} );

			}
//			Console.ReadLine ();
		}


		// 37	out vcc immediate

		// 35	out	B Enable2
		// 33	out	B Phase1 

		// 31	out	A Enable2
		// 29	out	A Phase1
		static async void TestGpio()
		{
			//var inputConfig = ConnectorPin.P1Pin15.Input ();
			//inputConfig.OnStatusChanged (status => {Console.WriteLine (status);});


			var connection = new GpioConnection ( ConnectorPin.P1Pin29.Output (),
			                                     ConnectorPin.P1Pin31.Output (),

			                                     ConnectorPin.P1Pin33.Output (),
			                                     ConnectorPin.P1Pin35.Output (),

			                                     ConnectorPin.P1Pin37.Output ()
																											);

			var connected29 = connection.Pins [ConnectorPin.P1Pin29];
			var connected31 = connection.Pins [ConnectorPin.P1Pin31];

			var connected33 = connection.Pins [ConnectorPin.P1Pin33];
			var connected35 = connection.Pins [ConnectorPin.P1Pin35];

			var connected37 = connection.Pins [ConnectorPin.P1Pin37];

			connected37.Enabled = true;	// vcc on


			var rightWheel = new WheelDriver ( phasePin: connected29, enablePin:connected31 );
			var leftWheel = new WheelDriver ( phasePin: connected33, enablePin:connected35 );
			var dualWheel = new DualWheelDriver( leftWheel, rightWheel );
			var tank = new TankDriver( dualWheel );

				var task = Task.Run(async() => {
				while(true)
				{

					if( Console.KeyAvailable )
					{
						var res = Console.ReadKey(true);
						switch( res.Key )
						{
						case ConsoleKey.Escape:
						case ConsoleKey.Q:
							Console.WriteLine (" console exit");
							return;
						case ConsoleKey.S:
						case ConsoleKey.DownArrow:
							tank.ReverseAccelerate();
							Console.WriteLine ("console back "+ tank.power );
							break;
						case ConsoleKey.W:
						case ConsoleKey.UpArrow:
							tank.Accelerate();
							Console.WriteLine ("console fore "+ tank.power);
							break;
						case ConsoleKey.A:
						case ConsoleKey.LeftArrow:
							tank.TurnLeft();
							Console.WriteLine ("console left "+ tank.power);
							break;
						case ConsoleKey.D:
						case ConsoleKey.RightArrow:
							tank.TurnRight();
							Console.WriteLine ("console right "+ tank.power);
							break;
						case ConsoleKey.Spacebar:
							tank.Brake();
							Console.WriteLine ("console Spacebar");
							break;
						}
					}
					await Task.Delay(100);
//					await Task.Yield();
				}

			} );

			await task;

			connection.Close ();
		}




		static void InputTest( InputPinConfiguration config )
		{
			var connection = new GpioConnection (config);
//			connection.PinStatusChanged += (sender, e) => {
//				Console.WriteLine ("PinStatusChanged() : "+e.Enabled );
//			};

			for (int i=0; i<10000; i++) 
			{
				Console.WriteLine (connection.Pins[config].Enabled );
				System.Threading.Thread.Sleep (10);
			}


			connection.Close ();	

		}


	}
}
