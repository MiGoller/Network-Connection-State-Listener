/*
 * Created by SharpDevelop.
 *
 * User: goller consulting, michael
 * Date: 19.12.2011
 * Time: 11:57
 * 
 */
using System;
using System.Net.NetworkInformation;

namespace Goller.Networking.ConnectionStateListener
{
	/// <summary>
	/// Description of NetworkStatus.
	/// </summary>
	public class NetworkStatus
	{
		#region Class variables
		private string id = null;
		private string name = null;
		private string description = null;
		private OperationalStatus operationalStatus = OperationalStatus.Unknown;
		#endregion
		
		#region Getters and setters
		public string Id {
			get { return id; }
			internal set { id = value; }
		}
		
		public string Name {
			get { return name; }
			internal set { name = value; }
		}
		
		public string Description {
			get { return description; }
			internal set { description = value; }
		}
		
		public OperationalStatus OperationalStatus {
			get { return operationalStatus; }
			internal set { operationalStatus = value; }
		}
		#endregion
		
		#region Constructors
		public NetworkStatus()
		{
			id = null;
			name = null;
			description = null;
			operationalStatus = OperationalStatus.Unknown;
		}
		
		public NetworkStatus(string id, string name, string description, OperationalStatus operationalStatus)
			: this()
		{
			this.id = id;
			this.name = name;
			this.description = description;
			this.operationalStatus = operationalStatus;
		}
		
		public NetworkStatus(NetworkInterface adapter)
			: this(adapter.Id, adapter.Name, adapter.Description, adapter.OperationalStatus)
		{
		}
		#endregion
	}
}
