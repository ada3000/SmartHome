using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTeco.Lib.Base;
using System.Data.SqlClient;
using System.Data;

namespace SH.Data
{
	public static class SqlHelper
	{
		private static string IntType = typeof(int).ToString();
		private static string LongType = typeof(long).ToString();
		private static string DateType = typeof(DateTime).ToString();
		private static string StringType = typeof(string).ToString();
		private static string BoolType = typeof(bool).ToString();
		private static string FloatType = typeof(float).ToString();
		private static string DoubleType = typeof(double).ToString();

		public static IEnumerable<PropContainer> ExecProc(this SqlConnection con, string procName, PropContainer props)
		{
			List<PropContainer> result = new List<PropContainer>();

			using (SqlCommand cmd = CreateProcCommand(procName, props, con))
			{
				if (con.State != ConnectionState.Open)
				{
					con.Close();
					con.Open();
				}

				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
					{
						PropContainer row = new PropContainer();
						for (int i = 0; i < reader.FieldCount; i++)
							row[reader.GetName(i)] = reader.GetValue(i);

						result.Add(row);
						//yield return row;
					}
			}

			return result;
		}

		public static void ExecProcNonQuery(this SqlConnection con, string procName, PropContainer props)
		{
			using (SqlCommand cmd = CreateProcCommand(procName, props, con))
			{
				if (con.State != ConnectionState.Open)
				{
					con.Close();
					con.Open();
				}

				cmd.ExecuteNonQuery();
			}
		}

		private static SqlDbType GetDbType(object value)
		{
			string curType = value.GetType().ToString();

			if (curType == IntType) return SqlDbType.Int;
			if (curType == LongType) return SqlDbType.BigInt;
			if (curType == DateType) return SqlDbType.DateTime;
			if (curType == StringType) return SqlDbType.NVarChar;
			if (curType == BoolType) return SqlDbType.Bit;
			if (curType == FloatType) return SqlDbType.Real;
			if (curType == DoubleType) return SqlDbType.Real;

			throw new ArgumentOutOfRangeException("Type of value=" + curType + " not supported!");
		}

		private static SqlCommand CreateProcCommand(string procName, PropContainer props, SqlConnection con, int timeoutSec = 30)
		{
			var cmd = new SqlCommand(procName, con);
			cmd.CommandType = CommandType.StoredProcedure;
			// Максимальное время ожидания выполнения
			cmd.CommandTimeout = timeoutSec;

			if (props != null)
				foreach (var key in props.Dictionary.Keys)
				{
					object value = props.Dictionary[key];
					SqlDbType type = GetDbType(value);
					int size = type == SqlDbType.NVarChar ? 2000000000 : 0;

					cmd.Parameters.Add(key, type, size).Value = value;
				}

			return cmd;
		}
	}
}
