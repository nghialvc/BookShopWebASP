using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopWebASP.Models;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;

namespace BookShopWebASP.Controllers
{
    public class DataProvider
    {
        private static DataProvider instance;

        public static DataProvider Instance
        {
            get { if (instance == null) instance = new DataProvider(); return instance; }
            set { instance = value; }
        }

        private BookShopDatabaseEntities dataBase;

        public BookShopDatabaseEntities DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        private DataProvider()
        {
            DataBase = new BookShopDatabaseEntities();
        }

        public static string getHashPass(string normalPass)
        {
            string base64text = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(normalPass));
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytesmd5 = md5provider.ComputeHash(new UTF8Encoding().GetBytes(base64text));
            string md5text = string.Empty;
            foreach (var item in bytesmd5)
            {
                md5text += item;
            }
            return md5text;
        }

        public DataTable ExecuteQuery(string query, object[] parameters = null)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(DataBase.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    string[] ListPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in ListPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameters[i]);
                            i++;
                        }
                    }
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
                connection.Close();
            }
            return data;
        }
    }
}