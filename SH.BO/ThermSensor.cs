using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
    public class ThermSensor
    {
        public string Id;
        public DateTime Create;
        public string Name;
        public string Category;
        public string Url;
        /// <summary>
        /// Интервал обновления в секундах
        /// </summary>
        public int UpdateInteval;
        /// <summary>
        /// Диапазон для хранения записей в днях
        /// </summary>
        public int StoreDays;
    }
}
