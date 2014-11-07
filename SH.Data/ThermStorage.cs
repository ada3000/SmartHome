using System;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SH.BO;

namespace SH.Data
{
    public class ThermStorage
    {
        private SqlConnection _con = null;
        public ThermStorage()
        {
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["main"].ConnectionString);
            _con.Open();
        }
        public void PushStamp(ThermStamp stamp)
        {
            SqlCommand cmd = new SqlCommand("", _con);

            cmd.CommandText = string.Format("insert into [ThermData](dtCreate,idSensor, nValue) values('{0}','{1}',{2})", stamp.Create.ToString("s"), stamp.SensorId, stamp.Value);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        public void ListStamp(string sensorId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
        public void ApplySensor(ThermSensor sensor)
        {
            throw new NotImplementedException();
        }
        public void DelSensor(string id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<ThermSensor> ListSensors()
        {
            SqlCommand cmd = new SqlCommand("",_con);

            cmd.CommandText = string.Format("SELECT [id],[dtCreate],[sName],[sCategory],[sUrl],[nInterval],nStoreDays FROM [ThermSensor]");
            using (cmd)
            {
                var dr = cmd.ExecuteReader();
                using (dr)
                {
                    while(dr.Read())
                    {
                        ThermSensor s = new ThermSensor();

                        s.Id = dr.GetString(0);
                        s.Create = dr.GetDateTime(1);
                        s.Name = dr.GetString(2);
                        s.Category = dr.GetString(3);
                        s.Url = dr.GetString(4);

                        s.UpdateInteval = dr.GetInt32(5);
                        s.StoreDays = dr.GetInt32(5);

                        yield return s;
                    }
                }
            }
        }
    }
}
