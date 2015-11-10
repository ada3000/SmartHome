using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public interface IHostData
	{
		ObjHost FindOrCreate(string name, string cluster);
		void Remove(long id);
		IEnumerable<ObjHost> All();
	}
}
