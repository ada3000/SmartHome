using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public interface ISourceData
	{
		/// <summary>
		/// ПОиск или создание источника
		/// </summary>
		/// <param name="idHost"></param>
		/// <param name="url"></param>
		/// <param name="isPush"></param>
		/// <returns></returns>
		ObjSource FindOrCreate(long idHost, string url, bool isPush);
		/// <summary>
		/// Поиск ресурса для обработки
		/// </summary>
		/// <returns></returns>
		ObjSource Checkout();
		/// <summary>
		/// Завершение обработки ресурса, расчет следующей даты обработки
		/// </summary>
		/// <param name="sourceId"></param>
		/// <param name="content"></param>
		void Checkin(long sourceId, string content);
		/// <summary>
		/// Завершение обработки ресурса с ошибкой, увеличивает счетчик Retry
		/// </summary>
		/// <param name="sourceId"></param>
		/// <param name="error"></param>
		void CheckinError(long sourceId, string error);
		/// <summary>
		/// Сброс всех заблокированных ресурсов, использууется при старте сервиса
		/// </summary>
		void ResetAllCheckout();
		/// <summary>
		/// Удаление источника
		/// </summary>
		/// <param name="id"></param>
		void Remove(long id);
	}
}
