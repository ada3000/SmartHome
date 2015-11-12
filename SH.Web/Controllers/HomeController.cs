using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SH.BO;
using SH.Data;
using SH.Web.Models;
using Newtonsoft.Json;

namespace SH.Web.Controllers
{
	public class HomeController : Controller
	{
		private static DataSource _dataSource = new DataSource(Config.DBConStr);

		private HostDataStorage _hostsStg;
		private ResultDataStorage _resultsStg;

		private string[] _skipCpuSubTypes = new [] { "Average1Min", "Current" };
		
		private const string HddInfo = "free {0} of {1}";

		/// <summary>
		/// TODO: use DI IoC
		/// </summary>
		public HomeController()
		{
			_hostsStg = new HostDataStorage(_dataSource);
			_resultsStg = new ResultDataStorage(_dataSource);
		}

		public ActionResult Index()
		{
			List<GroupDisplayModel> models = new List<GroupDisplayModel>();

			var hosts = _hostsStg.All();
			var results = _resultsStg.All();

			var groups = GroupByHost(results);

			foreach (var kv in groups)
			{
				kv.Key.Data = RenderSensors(kv.Value).ToArray();
				kv.Key.Warnings = kv.Key.Data.Where(d => d.IsError).Count();

				models.Add(kv.Key);
			}

			models.Sort((a, b) => -a.Warnings.CompareTo(b.Warnings));

			return View(models);
		}
		private IEnumerable<SensorDisplayModel> RenderSensors(IEnumerable<SensorValue> sensors)
		{
			List<SensorDisplayModel> result = new List<SensorDisplayModel>();

			foreach(var s in sensors)
			{
				SensorDisplayModel model = new SensorDisplayModel 
				{  
					Type = SensorDisplayModelType.Progress,
					TitleLeft = s.Name,
					IsError = s.WarningValueMin.HasValue && s.Value.HasValue && s.WarningValueMin <= s.Value,
					PersentValue = s.Value.HasValue ? ((int)s.Value * 10) / 10.0f: 0,
				};

				model.TitlePersent = model.PersentValue + "%";

				switch(s.Type)
				{
					case SensorValueType.Info:
						model.Type = SensorDisplayModelType.Text;						
						model.TitleRight = RenderSeconds((long)s.Value);
						break;

					case SensorValueType.CPU:
						model.IsCore = true;
						if (_skipCpuSubTypes.Contains(s.SubType)) continue;
						break;

					case SensorValueType.Memory:
						model.IsCore = true;
						model.TitlePersent = RoundBytes((long)s.Children[0].Value);
						model.TitleRight = RoundBytes((long)s.Children[0].ValueMax);
						break;

					case SensorValueType.Hdd:
						model.TitleRight = string.Format(HddInfo, 
							RoundBytes((long)s.Children.Where(c=>c.SubType=="AvailableFreeSpace").First().Value),
							RoundBytes((long)s.Children.Where(c => c.SubType == "TotalSize").First().Value));
						break;
				}

				result.Add(model);
			}

			return result;
		}

		private string RenderSeconds(long sec)
		{
			int secPerDay = 86400;

			int days = (int)(sec / secPerDay);
			sec -= days * secPerDay;

			int hours = (int)sec / 3600;
			sec -= hours * 3600;

			int minutes = (int)sec / 60;
			sec -= minutes * 60;

			string result = hours + ":" + minutes + ":" + sec;

			if (days > 0) result = days + " Days " + result;

			return result;
		}

		private string RoundBytes(long value)
		{
			string[] formats = new[] {"B", "K", "M", "G", "T", "P" };
			int mult = 1024;
			int index = 0;

			float result = value;
			while(result > mult && index < formats.Length-1)
			{
				result /= mult;
				index++;
			}

			result = ((int)(result * 10)) / 10.0f;

			string ret = result.ToString() +" "+ formats[index];

			if (index > 0) ret += "B";

			return ret;
		}

		private Dictionary<GroupDisplayModel, List<SensorValue>> GroupByHost(IEnumerable<ObjResult> data)
		{
			Dictionary<GroupDisplayModel, List<SensorValue>> result = new Dictionary<GroupDisplayModel, List<SensorValue>>();

			foreach (var item in data)
			{	
				TelemetryResult tRes = JsonConvert.DeserializeObject<TelemetryResult>(item.Content);

				GroupDisplayModel model = new GroupDisplayModel
				{
					Create = tRes.Create,
					Id = item.HostId.ToString(),
					GroupName = tRes.ClusterName,
					Name = tRes.ServerName
				};

				if (!result.ContainsKey(model))
					result.Add(model, new List<SensorValue>());

				var curKey = result.Keys.Where(k => k.GetHashCode() == model.GetHashCode()).First();

				//set oldest date
				if (curKey.Create > model.Create)
					curKey.Create = model.Create;

				result[model].AddRange(tRes.Values.Where(s => s.Type != SensorValueType.Info || s.SubType == "UpTime"));
			}

			return result;
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Templates()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}