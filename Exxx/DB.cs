using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
    class DB
    {
        ConnectionStringSettings ConnectionStringSetting;
        string ConnectionString;
        SqlConnection Connection;
        SqlDataAdapter DataAdapter;
        public  DB()
        {
            ConnectionStringSetting = ConfigurationManager.ConnectionStrings["connectionStr"];
           ConnectionString = ConnectionStringSetting.ConnectionString.ToString();
           Connection = new SqlConnection(ConnectionString);
             DataAdapter = new SqlDataAdapter();
        }

        
        /// <summary>
        /// insert a DataEvent into database
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        public void WriteToDB(int param1, DateTime param2, String param3)
        {
            string saveDataEv = "INSERT into DataEvent (ActivityCode,StartTime,StatusEvent ) VALUES (@actCode,@stT,@status)";
            using (DataAdapter.InsertCommand = new SqlCommand(saveDataEv, Connection))
            {

                DataAdapter.InsertCommand.Parameters.Add("@actCode", SqlDbType.Int).Value = param1;
                DataAdapter.InsertCommand.Parameters.Add("@stT", SqlDbType.DateTime).Value = param2;
                DataAdapter.InsertCommand.Parameters.Add("@status", SqlDbType.VarChar, 50).Value = param3;
                Connection.Open();
                DataAdapter.InsertCommand.ExecuteNonQuery();
            }
            Connection.Close();
        }
        /// <summary>
        /// insert a Car into database
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void WriteToDB(int ts,int param1, string param2)
        {
            string saveDataEv = "INSERT into Car (TimeStmp,RoadSegment,Speed) VALUES (@tsmp,@nameC,@speed)";

            using (DataAdapter.InsertCommand = new SqlCommand(saveDataEv, Connection))
            {
                DataAdapter.InsertCommand.Parameters.Add("@tsmp", SqlDbType.Int).Value = ts;
                DataAdapter.InsertCommand.Parameters.Add("@speed", SqlDbType.Float).Value = (float)param1;
                DataAdapter.InsertCommand.Parameters.Add("@nameC", SqlDbType.VarChar,50).Value = param2;
                Connection.Open();
                DataAdapter.InsertCommand.ExecuteNonQuery();
               
            }
            Connection.Close();
        }
        public void WriteToDB(int df,string param1, string param2)
        {
            string saveDataEv = "INSERT into QueryLatency (dimSize,KeyQL,ValueQL) VALUES (@dimF,@key,@value)";

            using (DataAdapter.InsertCommand = new SqlCommand(saveDataEv, Connection))
            {
                DataAdapter.InsertCommand.Parameters.Add("@dimF", SqlDbType.Int).Value = df;
                DataAdapter.InsertCommand.Parameters.Add("@key", SqlDbType.VarChar,100).Value = param1;
                DataAdapter.InsertCommand.Parameters.Add("@value", SqlDbType.VarChar, 100).Value = param2;
                Connection.Open();
                DataAdapter.InsertCommand.ExecuteNonQuery();

            }
            Connection.Close();
        }
        public void Write(int ts,String s)
        {
            string[] split =s.Split(new[] { ',', ' ','{','}','='}, StringSplitOptions.RemoveEmptyEntries);
            WriteToDB(ts,float.Parse(split[1]) ,int.Parse(split[3]));

        }
        public void Write(String s)
        {
            string[] split = s.Split(new[] { ',', ' ', '{', '}', '=' }, StringSplitOptions.RemoveEmptyEntries);
            WriteToDB(1, float.Parse(split[1]), int.Parse(split[3]));

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param1">avreage</param>
        /// <param name="param2">groupId(RoadSegment)</param>
        public void WriteToDB(int ts,float param1, int param2)
        {
            string saveDataEv = "INSERT into Query (TimeStQ,Avreage,GroupId) VALUES (@tsq,@avg,@gId)";

            using (DataAdapter.InsertCommand = new SqlCommand(saveDataEv, Connection))
            {
                DataAdapter.InsertCommand.Parameters.Add("@tsq", SqlDbType.Int).Value = ts;
                DataAdapter.InsertCommand.Parameters.Add("@avg", SqlDbType.Float).Value = param1;
                DataAdapter.InsertCommand.Parameters.Add("@gId", SqlDbType.Int).Value = param2;
                Connection.Open();
                DataAdapter.InsertCommand.ExecuteNonQuery();
               
            }
            Connection.Close();
        }
        public List<DataEvent> ReadFromDB(int no) {
            List<DataEvent> list = new List<DataEvent>();

            DataAdapter.SelectCommand = new SqlCommand("SELECT top (@param) * from DataEvent",Connection);
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = "@param",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = no
            };
            DataAdapter.SelectCommand.Parameters.Add(parameter);
            //DataAdapter.SelectCommand.Parameters.Add("@value1", SqlDbType.Int).Value = no;
            Connection.Open();
            using (SqlDataReader reader = DataAdapter.SelectCommand.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        DataEvent de = new DataEvent
                        {
                            ActivityCode = Convert.ToInt32(reader["ActivityCode"]),
                            StartTime = DateTime.Parse(reader["StartTime"].ToString()),
                            Status = reader["StatusEvent"].ToString()
                        };
                        list.Add(de);
                    }
                }
            }
            Connection.Close();
            return list;

        }
        public List<Car> ReadCarsDB(int no)
        {
            List<Car> list = new List<Car>();

            DataAdapter.SelectCommand = new SqlCommand("SELECT top (@param) * from Car", Connection);
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = "@param",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = no
            };
            DataAdapter.SelectCommand.Parameters.Add(parameter);
            //DataAdapter.SelectCommand.Parameters.Add("@value1", SqlDbType.Int).Value = no;
            Connection.Open();
            using (SqlDataReader reader = DataAdapter.SelectCommand.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        Car de = new Car
                        {
                            RoadSegment = reader["RoadSegment"].ToString(),
                            Speed = int.Parse(reader["Speed"].ToString()),
                            
                        };
                        list.Add(de);
                    }
                }
                
            }
            Connection.Close();
            return list;

        }
    }
    
}
