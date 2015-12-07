using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SH.Utils
{
	/// <summary>
	/// Параметры для инициализации типа
	/// </summary>
	public class TypeParams
	{
		[XmlAttribute("assembly")]
		public string AssemblyName;

		[XmlAttribute("type")]
		public string TypeName;

		public override string ToString()
		{
			return AssemblyName + "/" + TypeName;
		}
	}
}
