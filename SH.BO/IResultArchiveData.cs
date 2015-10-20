﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public interface IResultArchiveData
	{
		/// <summary>
		/// Поиск записей в архиве
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="hostId"></param>
		/// <returns></returns>
		IEnumerable<ObjResult> Find(DateTime from, DateTime to, long? hostId);
		/// <summary>
		/// Вставка в архив
		/// </summary>
		/// <param name="sourceId"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		long Insert(long sourceId, string content);
		void Remove(long sourceId);
		/// <summary>
		/// Очистка архива от записей старше days
		/// </summary>
		/// <param name="days"></param>
		void Clear(int days);
	}
}
