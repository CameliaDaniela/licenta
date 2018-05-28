using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
    class DB
    {

        SqlConnection conn = new SqlConnection("Data source=DESKTOP-F3CIUE8\\SQLEXPRESS;Initial Catalog=Licenta;Integrated Security=true");
        SqlDataAdapter da = new SqlDataAdapter();
    
        public void WriteToDB(int param1, DateTime param2, String param3)
        {
            string saveDataEv = "INSERT into DataEvent (ActivityCode,StartTime,StatusEvent ) VALUES (@actCode,@stT,@status)";
            using (da.InsertCommand = new SqlCommand(saveDataEv, conn))
            {

                da.InsertCommand.Parameters.Add("@actCode", SqlDbType.Int).Value = param1;
                da.InsertCommand.Parameters.Add("@stT", SqlDbType.DateTime).Value = param2;
                da.InsertCommand.Parameters.Add("@status", SqlDbType.VarChar, 50).Value = param3;
                conn.Open();
                da.InsertCommand.ExecuteNonQuery();
            }
        }
        public void WriteToDB(int param1, String param2)
        {
            string saveDataEv = "INSERT into Car (NameCar,Speed) VALUES (@nameC,@speed)";

            using (da.InsertCommand = new SqlCommand(saveDataEv, conn))
            {

                da.InsertCommand.Parameters.Add("@speed", SqlDbType.Int).Value = param1;
                da.InsertCommand.Parameters.Add("@nameC", SqlDbType.VarChar,50).Value = param2;
                conn.Open();
                da.InsertCommand.ExecuteNonQuery();
            }
        }
        public List<DataEvent> ReadFromDB(int no) {
            List<DataEvent> list = new List<DataEvent>();

            da.SelectCommand = new SqlCommand("SELECT top (@param) * from DataEvent",conn);
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = "@param",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = no
            };
            da.SelectCommand.Parameters.Add(parameter);
            //da.SelectCommand.Parameters.Add("@value1", SqlDbType.Int).Value = no;
            conn.Open();
            using (SqlDataReader reader = da.SelectCommand.ExecuteReader())
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

            da.SelectCommand = new SqlCommand("SELECT top (@param) * from Car", conn);
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = "@param",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = no
            };
            da.SelectCommand.Parameters.Add(parameter);
            //da.SelectCommand.Parameters.Add("@value1", SqlDbType.Int).Value = no;
            conn.Open();
            using (SqlDataReader reader = da.SelectCommand.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        Car de = new Car
                        {
                            Name = reader["NameCar"].ToString(),
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
