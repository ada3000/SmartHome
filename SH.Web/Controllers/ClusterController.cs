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
    public class ClusterController : Controller
    {
        private static DataSource _dataSource = new DataSource(Config.DBConStr);

        private HostDataStorage _hostsStg;
        private ResultDataStorage _resultsStg;

        private string[] _skipCpuSubTypes = new[] { "Average1Min", "Current" };

        private const string HddInfo = "free {0} of {1}";

        /// <summary>
        /// TODO: use DI IoC
        /// </summary>
		public ClusterController()
        {
            _hostsStg = new HostDataStorage(_dataSource);
            _resultsStg = new ResultDataStorage(_dataSource);
        }

        public ActionResult Index()
        {            
            return View();
        }

		public ActionResult GetData(string type)
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

			return Json(models);
		}
        private IEnumerable<SensorDisplayModel> RenderSensors(IEnumerable<SensorInfo> sensors)
        {
            List<SensorDisplayModel> result = new List<SensorDisplayModel>();

            foreach (var sInfo in sensors)
            {
                SensorValue s = sInfo.Value;

                SensorDisplayModel model = new SensorDisplayModel
                {
                    SourceId = sInfo.SourceId,
                    Type = SensorDisplayModelType.Progress,
                    TitleLeft = s.Name,
                    IsError = s.WarningValueMin.HasValue && s.Value.HasValue && s.WarningValueMin <= s.Value,
                    PersentValue = s.Value.HasValue ? ((int)s.Value * 10) / 10.0f : 0,
					SensorType = s.Type.ToString(),
					SensorSubType = s.SubType
                };

                model.TitlePersent = model.PersentValue + "%";

                switch (s.Type)
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

                        if (s.Children != null && s.Children.Count > 0) //old format support
                        {
                            model.TitlePersent = RoundBytes((long)(s.Children[0].ValueMax - s.Children[0].Value));
                            model.TitleRight = RoundBytes((long)s.Children[0].ValueMax);
                        }
                        else
                        {
                            model.PersentValue = ((int)(1.0f * s.Value / s.ValueMax * 1000)) / 10;

                            //model.TitlePersent = model.PersentValue + "%";
							model.TitlePersent = RoundBytes((long)s.Value);
                            model.TitleRight = RoundBytes((long)s.Value)+" / "+ RoundBytes((long)s.ValueMax);
                        }

                        break;

                    case SensorValueType.Hdd:
                        if (s.Children != null && s.Children.Count > 0) //old format support
                        {
                            model.TitleRight = string.Format(HddInfo,
                                RoundBytes((long)s.Children.Where(c => c.SubType == "AvailableFreeSpace").First().Value),
                                RoundBytes((long)s.Children.Where(c => c.SubType == "TotalSize").First().Value));
                        }
                        else
                        {
                            model.PersentValue = ((int)(1.0f * s.Value / s.ValueMax * 1000)) / 10;
                            model.TitlePersent = model.PersentValue + "%";

                            model.TitleRight = string.Format(HddInfo,
                                RoundBytes((long)(s.ValueMax - s.Value)),
                                RoundBytes((long)s.ValueMax));
                        }

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

		private string BytesToGb(long value)
		{
			float result = value;
			result /= 1024;
			result /= 1024;
			result /= 1024;

			result = ((int)(result * 10)) / 10.0f;

			string ret = result.ToString();

			return ret;
		}
        private string RoundBytes(long value)
        {
            string[] formats = new[] { "B", "K", "M", "G", "T", "P" };
            int mult = 1024;
            int index = 0;

            float result = value;
            while (result > mult && index < formats.Length - 1)
            {
                result /= mult;
                index++;
            }

            result = ((int)(result * 10)) / 10.0f;

            string ret = result.ToString() + " " + formats[index];

            if (index > 0) ret += "B";

            return ret;
        }

        private Dictionary<GroupDisplayModel, List<SensorInfo>> GroupByHost(IEnumerable<ObjResult> data)
        {
            Dictionary<GroupDisplayModel, List<SensorInfo>> result = new Dictionary<GroupDisplayModel, List<SensorInfo>>();

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
                    result.Add(model, new List<SensorInfo>());

                var curKey = result.Keys.Where(k => k.GetHashCode() == model.GetHashCode()).First();

                //set oldest date
                if (curKey.Create > model.Create)
                    curKey.Create = model.Create;

                result[model].AddRange(tRes.Values.Where(s => s.Type != SensorValueType.Info || s.SubType == "UpTime")
                    .Select(v => new SensorInfo { Value = v, SourceId = item.SourceId }));
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

        public class SensorInfo
        {
            public SensorValue Value;
            public long SourceId;
        }
    }
}