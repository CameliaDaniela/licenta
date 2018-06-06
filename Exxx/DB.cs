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
        }
        public void WriteToDB(int param1, string param2)
        {
            string saveDataEv = "INSERT into Car (RoadSegment,Speed) VALUES (@nameC,@speed)";

            using (DataAdapter.InsertCommand = new SqlCommand(saveDataEv, Connection))
            {

                DataAdapter.InsertCommand.Parameters.Add("@speed", SqlDbType.Int).Value = param1;
                DataAdapter.InsertCommand.Parameters.Add("@nameC", SqlDbType.VarChar,50).Value = param2;
                Connection.Open();
                DataAdapter.InsertCommand.ExecuteNonQuery();
            }
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
            return list;

        }
    }
    
}
