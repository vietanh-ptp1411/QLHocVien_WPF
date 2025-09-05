using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.ConnectDatabase
{
    public class Connect
    {
        private static string sConnect = @"Server=DESKTOP-JPD75R6\SQLEXPRESS;Database=QLClass; Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        private static SqlConnection con = null;

        public Connect()
        {
            OpenConnect();
        }
        private static void OpenConnect()
        {
            con = new SqlConnection(sConnect);
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }
        public static DataTable DataTransport(string sSQL) //lấy dữ liệu từ sql lên qua câu truy vấn SQL
        {
            OpenConnect();
            SqlDataAdapter adapter = new SqlDataAdapter(sSQL, con);
            DataTable dtData = new DataTable();
            dtData.Clear();
            adapter.Fill(dtData);
            return dtData;
        }
        public static int DataExcution(string sSQL) // dduaw du lieu xuong database
        {
            int iResult = 0;
            OpenConnect();
            if(con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = sSQL;
            cmd.CommandType = CommandType.Text;
            iResult = cmd.ExecuteNonQuery();
            return iResult;
        }


    }
}
