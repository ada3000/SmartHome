using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SH.Web.Models;

namespace SH.Web.Utils
{
    public class SensorTypeRendererCPU: SensorTypeRendererBase
    {
        private static string[] _skipCpuSubTypes = new[] { "Average1Min", "Current" };

        public override bool IsValid(SensorInfo sensorInfo)
        {
            return sensorInfo.Value.Type == BO.SensorValueType.CPU;
        }

        public override bool Render(SensorDisplayModel model, SensorInfo sensorInfo)
        {
            if (!IsValid(sensorInfo)) return false;

            model.IsCore = true;
            if (_skipCpuSubTypes.Contains(sensorInfo.Value.SubType)) return false;

            return true;
        }
    }
}