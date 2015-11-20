using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SH.BO;
using SH.Data;
using SH.Web.Models;
using SH.Web.Utils;

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
                SensorDisplayModel model = SensorRenderer.Render(sInfo);
                if (model == null) continue;

                result.Add(model);
            }

            return result;
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

                result[model].AddRange(tRes.Values.Where(s => s.Type != SensorValueType.Info || s.SubType == "UpTime" || s.SubType == "ServiceVersion")
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
    }
}