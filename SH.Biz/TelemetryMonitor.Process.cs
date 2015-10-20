using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SH.Data;
using SH.BO;
using SH.Utils;

using iTeco.Lib.Base;
using iTeco.Lib.Srv;

using log4net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SH.Biz
{
    public class TelemetryMonitorProcess: ServiceProcessBase
    {
		private ILog Logger = LogManager.GetLogger("TelemetryMonitorProcess");

        private ThermStorage _stg = new ThermStorage();
        
        private class SensorData
        {
            public ThermSensor Sensor;
            public DateTime LastUpdate;
        }

        Dictionary<string, SensorData> _sensors = new Dictionary<string, SensorData>();

        protected override void DoWork(object p)
        {
            while(Event.Stopping.IsReset())
            {
                try
                {
                    Logger.Debug("Therm update ...");

                    List<ThermSensor> sensors = _stg.ListSensors().ToList();

                    foreach(var s in sensors)
                    {
                        SensorData currentData = null;
                        if (_sensors.ContainsKey(s.Id))
                            currentData = _sensors[s.Id];
                        else
                        {
                            currentData = new SensorData { Sensor = s };
                            _sensors.Add(s.Id, currentData);
                        }

                        if (currentData.LastUpdate.AddSeconds(currentData.Sensor.UpdateInteval) < DateTime.Now)
                        {
                            ReciveSensorData(s);
                            currentData.LastUpdate = DateTime.Now;
                        }
                    }                    

                    Logger.Debug("Therm end");
                }
                catch(Exception ex)
                {
                    Logger.Error("Therm update error: " + ex);
                }

                Thread.Sleep(60*1000);
            }
        }

        private void ReciveSensorData(ThermSensor sensor)
        {
            Logger.Debug("ReciveSensorData ... id="+sensor.Id+" url="+sensor.Url);

            string jsContent = HttpDownloader.Get(sensor.Url);

            JToken doc = JToken.Parse(jsContent);
            int tempValue = doc["value"].ToString().ToInt();

            ThermStamp st = new ThermStamp
            {
                SensorId = sensor.Id,
                Value = tempValue,
                Create = DateTime.Now
            };

            _stg.PushStamp(st);

            Logger.Debug("ReciveSensorData END");
        }
    }
}
