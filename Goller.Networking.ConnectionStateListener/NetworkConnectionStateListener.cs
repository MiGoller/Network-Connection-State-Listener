/*
 * Created by SharpDevelop.
 *
 * User: goller consulting, michael
 * Date: 19.12.2011
 * Time: 11:57
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Goller.Networking.ConnectionStateListener
{
	#region Delegates
	public delegate void NetworkConnectedHandler(object sender, NetworkStatus adapter);
	public delegate void NetworkDisconnectedHandler(object sender, NetworkStatus adapter);
	public delegate void NetworkDiscoveredHandler(object sender, NetworkStatus adapter);
	#endregion
	
	/// <summary>
	/// Description of NetworkConnectionStateListener.
	/// </summary>
	public class NetworkConnectionStateListener
	{
		#region Class variables
		/// <summary>
		/// This dictionary will keep the global network adapter state list.
		/// </summary>
		private Dictionary<string, NetworkStatus> networks = new Dictionary<string, NetworkStatus>();
		
		/// <summary>
		/// Set to true to ignore networks with GUIDs in network names.
		/// </summary>
		private bool ignoreGuidNetworks = false;
		#endregion
		
		#region Getters and setters
		/// <summary>
		/// Gets or sets to ignore networks with GUID in network name.
		/// </summary>
		public bool IgnoreGuidNetworks {
			get { return ignoreGuidNetworks; }
			set { ignoreGuidNetworks = value; }
		}
		#endregion
		
		#region Events
		public event NetworkConnectedHandler NetworkConnected;
		public event NetworkDisconnectedHandler NetworkDisconnected;
		public event NetworkDiscoveredHandler NetworkDiscovered;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Create a new NetworkConnectionStateListener instance.
		/// Networks with GUID in the network name will be ignored by default.
		/// </summary>
		public NetworkConnectionStateListener()
			: this(true)
		{
		}
		
		/// <summary>
		/// Create a new NetworkConnectionStateListener instance.
		/// </summary>
		/// <param name="ignoreGuidNetworks">Set to true to ignore networks with GUID in network name.</param>
		public NetworkConnectionStateListener(bool ignoreGuidNetworks)
		{
			this.ignoreGuidNetworks = ignoreGuidNetworks;
			
			UpdateNetworkState(true);
			
			NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
		}
		#endregion
		
		/// <summary>
		/// Gets the global network state list.
		/// </summary>
		/// <returns></returns>
		public NetworkStatus[] GetNetworkStatus()
		{
			NetworkStatus[] dummy = new NetworkStatus[networks.Values.Count];
			networks.Values.CopyTo(dummy, 0);
			return dummy;
		}
		
		/// <summary>
		/// Updates or initializes the network state list
		/// </summary>
		/// <param name="initList">Set to true, if you want to init the list. In that case no events will be fired.</param>
		private void UpdateNetworkState(bool initList)
		{
			Dictionary<string, NetworkStatus> dummyState = new Dictionary<string, NetworkStatus>();
			
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			
            foreach(NetworkInterface n in adapters)
            {
            	//	Add interface to current network state list.
            	dummyState.Add(n.Id, new NetworkStatus(n));
            	
            	if (!networks.ContainsKey(n.Id))
            	{
            		//	Add network to global state list.
            		networks.Add(n.Id, new NetworkStatus(n));
            		
            		if (!initList)
            		{
	            		//	Report new network.
	            		NotifyDiscovered(dummyState[n.Id]);
	            		
	            		//	Report operational status.
	            		if (n.OperationalStatus.Equals(OperationalStatus.Up))
	        			{
	        				NotifyConnected(dummyState[n.Id]);
	        			}
	        			else
	        			{
	        				NotifyDisconnected(dummyState[n.Id]);
	        			}
            		}
            	}
            	else
            	{
            		//	Network is known to the listenen.
            		if (networks[n.Id].OperationalStatus.Equals(n.OperationalStatus))
            		{
            			//	Status has not changed.
            		}
            		else
            		{
            			if (!initList)
            			{
	            			//	Status has changed.
	            			if (n.OperationalStatus.Equals(OperationalStatus.Up))
	            			{
	            				NotifyConnected(dummyState[n.Id]);
	            			}
	            			else
	            			{
	            				NotifyDisconnected(dummyState[n.Id]);
	            			}
            			}
            		}
            	}
            }
            
            //	Check for adapters that are not in the current list anymore.
            foreach (string adapterID in networks.Keys)
            {
            	if (!dummyState.ContainsKey(adapterID))
            	{
            		//	Network not found in the current list: Assuming it's disconnected.
            		networks[adapterID].OperationalStatus = OperationalStatus.Down;
            		
            		if (!initList)
            		{
            			//	Report network disconnection.
            			NotifyDisconnected(networks[adapterID]);
            		}
            	}
            }
		}
		
		#region Event-notification
		/// <summary>
		/// Fire event for network connection.
		/// </summary>
		/// <param name="adapter">Connected network interface</param>
		private void NotifyConnected(NetworkStatus adapter)
		{
			if (NetworkConnected != null)
			{
				if (!ContainsGUID(adapter.Name) || !ignoreGuidNetworks)
				{
					NetworkConnected(this, adapter);
				}
				
				
			}
		}
		
		/// <summary>
		/// Fire event for network disconnection.
		/// </summary>
		/// <param name="adapter">Disconnected network interface</param>
		private void NotifyDisconnected(NetworkStatus adapter)
		{
			if (NetworkDisconnected != null)
			{
				if (!ContainsGUID(adapter.Name) || !ignoreGuidNetworks)
				{
					NetworkDisconnected(this, adapter);
				}
				
			}
		}
		
		/// <summary>
		/// Fire event for network discovery.
		/// </summary>
		/// <param name="adapter">Discovered network interface</param>
		private void NotifyDiscovered(NetworkStatus adapter)
		{
			if (NetworkDiscovered != null)
			{
				if (!ContainsGUID(adapter.Name) || !ignoreGuidNetworks)
				{
					NetworkDiscovered(this, adapter);
				}
				
			}
		}
		#endregion
		
		/// <summary>
		/// A network address has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
		{
			//	Update the network state list and fire events.
			UpdateNetworkState(false);
		}
		
		#region GUID helpers
		/// <summary>
		/// Checks if the given expression is a valid GUID string.
		/// </summary>
		/// <param name="expression">String expression</param>
		/// <returns>True, if the expression is a valid GUID string.</returns>
		public static bool IsGUID(string expression)
		{
		    if (expression != null)
		    {
		        Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
		
		        return guidRegEx.IsMatch(expression);
		    }
		    
		    return false;
		}
		
		/// <summary>
		/// Checks if the given expression contains a valid GUID string.
		/// </summary>
		/// <param name="expression">String expression</param>
		/// <returns>True, if the expression contains at least one valid GUID string.</returns>
		public static bool ContainsGUID(string expression)
		{
		    if (expression != null)
		    {
		        Regex guidRegEx = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");
		
		        return guidRegEx.IsMatch(expression);
		    }
		    
		    return false;
		}
		#endregion
	}
}
