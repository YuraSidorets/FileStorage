//TODO: Убрать консольные команды 
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Server
{
    internal static class DBWorker
    {
        private static MySqlConnectionStringBuilder mysqlCSB;

        /// <summary>
        /// Init static field mysqlCSB
        /// </summary>
        private static void Init()
        {
            if (mysqlCSB == null)
            {
                mysqlCSB = new MySqlConnectionStringBuilder();

                mysqlCSB.Server = "mysql.sidoretsyura.myjino.ru";
                mysqlCSB.Database = "sidoretsyura_testdb";
                mysqlCSB.UserID = "sidoretsyura";
                mysqlCSB.Password = "123456";
                

                //mysqlCSB.Server = "127.0.0.1";
                //mysqlCSB.UserID = "root";
                //mysqlCSB.Database = "test";
                //mysqlCSB.Password = "1234";
                    

            }
        }


        /// <summary>
        /// Inputs a new row with file and iformation about it to DB 
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="table">Table in DB</param>
        public static void SetValue(Dictionary<string, object> info)
        {
            Init();

            string queryString = string.Format(@"INSERT INTO new_table (Name, Type, Date, Size, Data) VALUES ('{0}','{1}','{2:yyyy-MM-dd hh:mm:ss}','{3}',?file)",
                info["Name"], info["Type"], info["Date"], info["Size"]);
                
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;

                MySqlCommand mainCommand = new MySqlCommand(queryString, con);
                MySqlCommand timeoutCommand = new MySqlCommand("set net_write_timeout = 99999; set net_read_timeout = 99999", con);

                try
                {
                    byte[] data = (byte[])info["Data"];

                    MySqlParameter param = new MySqlParameter("?file", MySqlDbType.LongBlob, data.Length);
                    param.Value = data;
                    mainCommand.Parameters.Add(param);

                    con.Open();
                    timeoutCommand.ExecuteNonQuery();
                    mainCommand.CommandText = queryString;
                    mainCommand.ExecuteNonQuery();

                    con.Close();
                    mainCommand.Dispose();
                }
                catch (Exception e)
                {
                    //!!!!!!!!!
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }


        /// <summary>
        /// Returns DataTable instance that includes all the values in db except binary data
        /// </summary>
        public static DataTable GetDataTable()
        {
            Init();

            string query = @"SELECT Id, 
                               Name,     
                               Type,
                               Date,
                               Size,
                               Description               
                        FROM   new_table 
                        WHERE  Id > 5";
            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;
                MySqlCommand com = new MySqlCommand(query, con);
                con.Open();

                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows) dt.Load(dr);
                }

            }
            return dt;
        }

        public static object GetFileToWrite(int id)
        {
            Init();

            string query = string.Format(
                      @"SELECT Name,     
                               Type,
                               Data               
                        FROM   new_table 
                        WHERE  Id = {0}", id);

            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;
                MySqlCommand com = new MySqlCommand(query, con);
                con.Open();

                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows) dt.Load(dr);
                }

            }
            return (object)dt; //dt.Rows[0].ItemArray;
        }

        /// <summary>
        /// Delete instance from table
        /// </summary>
        /// <param name="name"> instance name</param>
        /// <param name="id"> instance id </param>
        public static void DeleteFromTable(string id)
        {
            string table = "new_table";

            Init();

            string query = string.Format(@"DELETE FROM {0} WHERE id='{1}'", table, id);

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;
                MySqlCommand com = new MySqlCommand(query, con);
                con.Open();

                com.ExecuteNonQuery();

                con.Close();
                com.Dispose();
            }
        }
    }
}

