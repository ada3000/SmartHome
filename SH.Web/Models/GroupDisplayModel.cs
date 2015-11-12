using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SH.Web.Models
{
	public class GroupDisplayModel
	{
		public string Id;
		public string Name;
		public string GroupName;
		public DateTime Create;
		/// <summary>
		/// Warnings count
		/// </summary>
		public int Warnings;

		public SensorDisplayModel[] Data;

		public override int GetHashCode()
		{
			return (Id+"_"+Name).GetHashCode();
		}
	}
}