using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SH.Web.Models;
using SH.BO;

namespace SH.Web.Utils
{
    public class SensorTypeRendererMem : SensorTypeRendererBase
    {
        public override bool IsValid(SensorInfo sensorInfo)
        {
            return sensorInfo.Value.Type == BO.SensorValueType.Memory;
        }

        public override bool Render(SensorDisplayModel model, SensorInfo sensorInfo)
        {
            if (!IsValid(sensorInfo)) return false;

            model.IsCore = true;

            SensorValue s = sensorInfo.Value;

            if (s.Children != null && s.Children.Count > 0) //old format support
            {
                model.TitlePersent = ((long)(s.Children[0].ValueMax - s.Children[0].Value)).ToXBytes();
                model.TitleRight = ((long)s.Children[0].ValueMax).ToXBytes();
            }
            else
            {
                model.PersentValue = ((int)(1.0f * s.Value / s.ValueMax * 1000)) / 10;

                //model.TitlePersent = model.PersentValue + "%";
                model.TitlePersent = ((long)s.Value).ToXBytes();
                model.TitleRight = ((long)s.Value).ToXBytes() + " / " + ((long)s.ValueMax).ToXBytes();
            }
            
            return true;
        }
    }
}