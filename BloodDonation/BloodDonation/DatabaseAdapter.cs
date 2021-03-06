﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BloodDonation
{
    public static class DatabaseAdapter
    {
        private static string UserID { get; set; } = "geartest";
        private static string UserPass { get; set; } = "Ck2143!Eq66-";
        private static string DataSource { get; set; } = "mssql6.gear.host";
        private static SqlConnectionStringBuilder dbConStringBuilder { get; set; }

        private static SqlConnection connection;

        private static Person personLogged;

        public static Person PersonLogged
        {
            get { return personLogged; }
            set { PersonLogged = value; }
        }


        public static async Task<SqlConnection> GetConnection()
        {
            if (connection == null)
            {
                dbConStringBuilder = new SqlConnectionStringBuilder();
                dbConStringBuilder.UserID = UserID;
                dbConStringBuilder.Password = UserPass;
                dbConStringBuilder.DataSource = DataSource;
                dbConStringBuilder.ConnectTimeout = 30;
                connection = new SqlConnection(dbConStringBuilder.ConnectionString);
                await connection.OpenAsync();
                return connection;
            }
            else
            {
                return connection;
            }
        }

        public static async Task LogIn(Credentials credentials, Person person)
        {
            var command = new SqlCommand($"select top 1 id, name from persons where login like '{credentials.User}' and pass like '{credentials.Pass}'", GetConnection().Result);

            command.CommandTimeout = 30;

            person = new Person();

            using (SqlDataReader reader = command.ExecuteReader())
            {

                if (await reader.ReadAsync() == false)
                    return;

                person.CustomerId = Convert.ToInt32(reader["id"]);
                person.CustomerName = Convert.ToString(reader["name"]);

                personLogged = person;

                return;
            }
        }

        public static async Task<double[]> GetBloodData()
        {
            var command = new SqlCommand($"select * from bloddpictures where [ID_PERSON] = {PersonLogged.CustomerId} and [SAW_PERSON] = 0", GetConnection().Result);

            command.CommandTimeout = 30;

            double[] data = new double[6];

            int id = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {

                if (await reader.ReadAsync() == false)
                    return null;

                data[0] = Convert.ToDouble(reader["ERYTROCYT"]);
                data[1] = Convert.ToDouble(reader["FIBRINOGEN"]);
                data[2] = Convert.ToDouble(reader["HEMOCYT"]);
                data[3] = Convert.ToDouble(reader["LEUKOCYT"]);
                data[4] = Convert.ToDouble(reader["PROTROMBIN"]);
                data[5] = Convert.ToDouble(reader["TROMBOCYT"]);
                id = Convert.ToInt16(reader["id"]);
            }

            command = new SqlCommand($"update bloddpictures set [SAW_PERSON] = 1 where [ID_PERSON] = {PersonLogged.CustomerId} and ID = {id}", GetConnection().Result);

            command.CommandTimeout = 30;

            using (SqlDataReader reader = command.ExecuteReader())
            {

            }

            return data;
        }

        internal static async Task<double[]> GetLastBloodData()
        {
            var command = new SqlCommand($"select * from bloddpictures where [ID_PERSON] = {PersonLogged.CustomerId}", GetConnection().Result);

            command.CommandTimeout = 30;

            double[] data = new double[6];

            int id = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {

                if (await reader.ReadAsync() == false)
                    return null;

                data[0] = Convert.ToDouble(reader["ERYTROCYT"]);
                data[1] = Convert.ToDouble(reader["FIBRINOGEN"]);
                data[2] = Convert.ToDouble(reader["HEMOCYT"]);
                data[3] = Convert.ToDouble(reader["LEUKOCYT"]);
                data[4] = Convert.ToDouble(reader["PROTROMBIN"]);
                data[5] = Convert.ToDouble(reader["TROMBOCYT"]);
                id = Convert.ToInt16(reader["id"]);
            }

            return data;
        }
    }                            
}                                
                                 