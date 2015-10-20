using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTeco.Lib.Base;
using System.Data.SqlClient;

namespace SH.Data
{
	public class DataSource
	{
		private string _conStr = null;
		public DataSource(string conStr)
		{
			_conStr = conStr;
		}

		public IEnumerable<PropContainer> ExecProc(string procName, PropContainer props)
		{
			using(SqlConnection con= new SqlConnection(_conStr))
				return con.ExecProc(procName, props);
		}

		public void ExecProcNonQuery(string procName, PropContainer props)
		{
			using (SqlConnection con = new SqlConnection(_conStr))
				con.ExecProcNonQuery(procName, props);
		}
	}
}
