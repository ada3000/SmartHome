using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SH.Web.Models;
using SH.BO;

namespace SH.Web.Utils
{
    public static class SensorRenderer
    {
        private static SensorTypeRendererBase[] _renderers = new SensorTypeRendererBase[]
        {
            new SensorTypeRendererCPU(),
            new SensorTypeRendererHdd(),
            new SensorTypeRendererInfo(),
            new SensorTypeRendererMem()
        };

        public static SensorDisplayModel Render(SensorInfo sensorInfo)
        {
            SensorValue s = sensorInfo.Value;

            SensorDisplayModel model = RenderHeader(sensorInfo);

            bool isSuccess = false;

            foreach(var ren in _renderers)
            {
                isSuccess = ren.Render(model, sensorInfo);
                if (isSuccess) break;
            }

            return isSuccess ? model : null;
        }



        private static SensorDisplayModel RenderHeader(SensorInfo sensorInfo)
        {
            var result= new SensorDisplayModel
            {
                SourceId = sensorInfo.SourceId,
                Type = SensorDisplayModelType.Progress,
                TitleLeft = sensorInfo.Value.Name,
                IsError = sensorInfo.Value.WarningValueMin.HasValue && sensorInfo.Value.Value.HasValue && sensorInfo.Value.WarningValueMin <= sensorInfo.Value.Value,
                PersentValue = sensorInfo.Value.Value.HasValue ? ((int)sensorInfo.Value.Value * 10) / 10.0f : 0,
                SensorType = sensorInfo.Value.Type.ToString(),
                SensorSubType = sensorInfo.Value.SubType
            };

            result.TitlePersent = result.PersentValue + "%";

            return result;
        }
    }
}