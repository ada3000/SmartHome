using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SH.Web.Models
{
	public class SensorDisplayModel
	{
		public string TitleLeft;
		public string TitleRight;

		public string TitlePersent;
		public float PersentValue;

		public bool IsError;
		public bool IsCore;

		public SensorDisplayModelType Type;

        public long SourceId;

		public string SensorType;
		public string SensorSubType;

	}
}