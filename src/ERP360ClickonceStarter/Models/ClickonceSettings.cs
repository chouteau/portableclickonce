using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PortableClickonce.Models
{
	public class ClickonceSettings
	{
		[Bindable(true)]
		public string ClickonceUrl { get; set; }
		[Bindable(true)]
		public string DestinationDirectory { get; set; }
		[Bindable(true)]
		public string Version { get; set; }
		[Bindable(true)]
		public string ExeFileName { get; set; }

		public DateTime LatestUse { get; set; }

		public bool LastRunSuccess { get; set; }

		public string FullExeFileName
		{
			get
			{
				if (DestinationDirectory == null
					|| Version == null
					|| ExeFileName == null)
				{
					return null;
				}
				return System.IO.Path.Combine(DestinationDirectory, Version, ExeFileName);
			}
		}
	}
}
