﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WindowsFormsAppDoctor
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


        public static async Task SetNewBloodPicture(Person person, double[] data)
        {
            var command = new SqlCommand($"insert into dbo.bloddpictures ([ID_PERSON],[ERYTROCYT],[FIBRINOGEN],[HEMOCYT],[LEUKOCYT],[PROTROMBIN],[TROMBOCYT],[SAW_PERSON],[SAW_DOCTOR]) values ({PersonLogged.CustomerId},{data[0]},{data[1]},{data[2]},{data[3]},{data[4]},{data[5]},0,{1})", GetConnection().Result);

            command.CommandTimeout = 30;

            person = new Person();

            using (SqlDataReader reader = command.ExecuteReader())
            {

                if (await reader.ReadAsync() == false)
                    return;

                return;
            }
        }                        
    }                            
}                                
                                 