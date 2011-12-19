/*
 * Created by SharpDevelop.
 *
 * User: goller consulting, michael
 * Date: 19.12.2011
 * Time: 12:00
 * 
 */
using System;

namespace Goller.Networking.ConnectionStateListener.Examples
{
	/// <summary>
	/// Brief example on how to use the NetworkConnectionStateListener
	/// </summary>
	public class Example_NetworkConnectionStateListener
	{
		public static void Main(string[] args)
		{
			//	Create a new NetworkConnectionStateListener instance.
			NetworkConnectionStateListener ncsl = new NetworkConnectionStateListener();
			
			//	Register event handler.
			ncsl.NetworkDiscovered += new NetworkDiscoveredHandler(ncsl_NetworkDiscovered);
			ncsl.NetworkConnected += new NetworkConnectedHandler(ncsl_NetworkConnected);
			ncsl.NetworkDisconnected += new NetworkDisconnectedHandler(ncsl_NetworkDisconnected);
			
			//	Write known network interfaces to console.
			foreach (NetworkStatus nws in ncsl.GetNetworkStatus())
			{
				Console.WriteLine("Network \"{0}\" is {1}.", nws.Name, nws.OperationalStatus);
			}
			
			//	Wait for address changes.
			Console.WriteLine();
			Console.WriteLine("Listening for address changes. Press any key to exit.");
            Console.ReadLine();
		}

		/// <summary>
		/// New network has been discovered.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="adapter"></param>
		static void ncsl_NetworkDisconnected(object sender, NetworkStatus adapter)
		{
			Console.WriteLine("Network has disconnected: {0}", adapter.Name);
		}

		/// <summary>
		/// Connected to a network.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="adapter"></param>
		static void ncsl_NetworkConnected(object sender, NetworkStatus adapter)
		{
			Console.WriteLine("Network has connected: {0}", adapter.Name);
		}

		/// <summary>
		/// Disconnected from a network.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="adapter"></param>
		static void ncsl_NetworkDiscovered(object sender, NetworkStatus adapter)
		{
			Console.WriteLine("Network discovered: {0}", adapter.Name);
		}
	}
}
