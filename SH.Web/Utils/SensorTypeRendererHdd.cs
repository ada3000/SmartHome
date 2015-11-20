using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SH.Web.Models;
using SH.BO;

namespace SH.Web.Utils
{
    public class SensorTypeRendererHdd : SensorTypeRendererBase
    {
        private const string HddInfo = "free {0} of {1}";

        public override bool IsValid(SensorInfo sensorInfo)
        {
            return sensorInfo.Value.Type == BO.SensorValueType.Hdd;
        }

        public override bool Render(SensorDisplayModel model, SensorInfo sensorInfo)
        {
            if (!IsValid(sensorInfo)) return false;

            SensorValue s = sensorInfo.Value;

            if (s.Children != null && s.Children.Count > 0) //old format support
            {
                model.TitleRight = string.Format(HddInfo,
                    ((long)s.Children.Where(c => c.SubType == "AvailableFreeSpace").First().Value).ToXBytes(),
                    ((long)s.Children.Where(c => c.SubType == "TotalSize").First().Value).ToXBytes());
            }
            else
            {
                model.PersentValue = ((int)(1.0f * s.Value / s.ValueMax * 1000)) / 10;
                model.TitlePersent = model.PersentValue + "%";

                model.TitleRight = string.Format(HddInfo,
                    ((long)(s.ValueMax - s.Value)).ToXBytes(),
                    ((long)s.ValueMax).ToXBytes());
            }

            return true;
        }
    }
}