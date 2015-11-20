using SH.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SH.Web.Utils
{
    public abstract class SensorTypeRendererBase
    {
        /// <summary>
        /// Проверка подходит ли тип
        /// </summary>
        /// <param name="sensorInfo"></param>
        /// <returns></returns>
        public abstract bool IsValid(SensorInfo sensorInfo);

        /// <summary>
        /// Отрисовка типа, false - неуспешно
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sensorInfo"></param>
        /// <returns></returns>
        public abstract bool Render(SensorDisplayModel model, SensorInfo sensorInfo);
    }
}