using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public class ObjSource
	{
		public long Id;
		public long HostId;
		public string Url;
		/// <summary>
		/// Источник Push сервер, регламентная обработка не требуется
		/// </summary>
		public bool IsPush;

		public DateTime Create;
		public DateTime? RunLast;
		public DateTime? RunNext;
		public DateTime? Checkout;

		public int DelaySec;
		public int Retry;

		public string Error;
	}
}
