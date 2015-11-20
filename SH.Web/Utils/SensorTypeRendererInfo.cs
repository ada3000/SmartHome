using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SH.Web.Models;

namespace SH.Web.Utils
{
    public class SensorTypeRendererInfo : SensorTypeRendererBase
    {
        public override bool IsValid(SensorInfo sensorInfo)
        {
            return sensorInfo.Value.Type == BO.SensorValueType.Info;
        }

        public override bool Render(SensorDisplayModel model, SensorInfo sensorInfo)
        {
            if (!IsValid(sensorInfo)) return false;

            model.Type = SensorDisplayModelType.Text;

            switch(sensorInfo.Value.SubType)
            {
                case "UpTime":
                    model.TitleRight = ((long)sensorInfo.Value.Value).SecondsToDaysTime();
                    break;
                default:
                    model.TitleRight = sensorInfo.Value.Value.HasValue ? sensorInfo.Value.Value.Value.ToString() : sensorInfo.Value.ValueStr;
                    break;
            }

            return true;
        }
    }
}