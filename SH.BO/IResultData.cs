using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public interface IResultData
	{
		IEnumerable<ObjResult> All();
		void CreateOrUpdate(long sourceId, string content);
		void Remove(long sourceId);
	}
}
