using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;

using iTeco.Lib.Base;

namespace SH.Data
{
	public class SourceDataStorage : ISourceData
	{
		private const string FindOrCreateProc = "ObjSource$FindOrCreate";
		private const string CheckoutProc = "ObjSource$Checkout";
		private const string CheckinProc = "ObjSource$Checkin";
		private const string CheckinErrorProc = "ObjSource$CheckinError";
		private const string CheckoutResetProc = "ObjSource$Checkout_Reset";
		private const string DelProc = "ObjSource$Del";

		private DataSource _source = null;

		public SourceDataStorage(DataSource source)
		{
			_source = source;
		}

		public ObjSource FindOrCreate(long hostId, string url, bool isPush)
		{
			var row = _source
				.ExecProc(FindOrCreateProc, new PropContainer(new[] { "idHost", "sUrl", "fIsPush" }, new object[] { hostId, url, isPush }))
				.First();

			ObjSource result = ReadFromProps(row);

			return result;
		}

		private ObjSource ReadFromProps(PropContainer props)
		{
			ObjSource result = new ObjSource
			{
				Id = props["id"].ToLong(),
				HostId = props["idHost"].ToLong(),
				Url = props["sUrl"].ToString(),
				IsPush = props["fIsPush"].ToBool(),
				Create = props["dtCreate"].ToDateTime(DateTime.UtcNow),
				RunLast = props["dtRunLast"].ToDateTime(),
				RunNext = props["dtRunNext"].ToDateTime(),
				Checkout = props["dtCheckout"].ToDateTime(),
				DelaySec = props["nDelaySec"].ToInt(),
				Retry = props["nRetry"].ToInt(),
				Error = props["sError"].IsValue() ? props["sError"].ToString() : null
			};

			return result;
		}

		public void Remove(long id)
		{
			_source.ExecProcNonQuery(DelProc, new PropContainer("id", id));
		}

		public ObjSource Checkout()
		{
			var row = _source
				.ExecProc(CheckoutProc)
				.FirstOrDefault();

			return row == null ? null : ReadFromProps(row);
		}

		public void Checkin(long sourceId, string content)
		{
			_source.ExecProcNonQuery(CheckinProc, new PropContainer(new[] { "id", "sContent" }, new object[] { sourceId, content }));
		}

		public void CheckinError(long sourceId, string error)
		{
			_source.ExecProcNonQuery(CheckinErrorProc, new PropContainer(new[] { "id", "sError" }, new object[] { sourceId, error }));
		}

		public void ResetAllCheckout()
		{
			_source.ExecProcNonQuery(CheckoutResetProc);
		}
	}
}
