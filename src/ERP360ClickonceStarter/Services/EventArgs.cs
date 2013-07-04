using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableClickonce.Services
{
	public class EventArgs<TData> : EventArgs
	{
		TData data;

		/// <summary>
		/// Initializes the EventArgs class.
		/// </summary>
		/// <param name="data">Information related to the event.</param>
		/// <exception cref="ArgumentNullException">The data is null.</exception>
		public EventArgs(TData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.data = data;
		}

		/// <summary>
		/// Gets the information related to the event.
		/// </summary>
		public TData Data
		{
			get { return data; }
		}

		/// <summary>
		/// Provides a string representation of the argument data.
		/// </summary>
		public override string ToString()
		{
			return data.ToString();
		}
	}
}
