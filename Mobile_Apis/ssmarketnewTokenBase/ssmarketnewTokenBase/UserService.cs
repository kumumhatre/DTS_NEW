using ssmarketnewTokenBase.ClassFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase
{
    public class UserService
    {
        SqlConnection con = Utils.conn;
        userinfo usrpro = new userinfo();
        public userinfo ValidateUser(string username, string password)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            using (SqlCommand cmd = new SqlCommand("validateloginUserForMobile", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            usrpro.message = reader.GetString(0);
                        if (usrpro.message == "Successful")
                        {
                            if (!reader.IsDBNull(1))
                                usrpro.id = reader.GetValue(1).ToString();
                            if (!reader.IsDBNull(2))
                                usrpro.name = reader.GetString(2);
                            if (!reader.IsDBNull(3))
                                usrpro.email_id = reader.GetString(3);
                            else
                                usrpro.email_id = "";
                            if (!reader.IsDBNull(4))
                                usrpro.clientcode = reader.GetString(4);
                            if (!reader.IsDBNull(5))
                                usrpro.exchanhges = reader.GetString(5);
                            if (!reader.IsDBNull(6))
                                usrpro.userstatus = reader.GetInt32(6);
                            // if (!reader.IsDBNull(7))
                            //   usrpro.CountryName = reader.GetString(7);                                              
                            if (!reader.IsDBNull(7))
                                usrpro.usertype = reader.GetInt32(7);

                            if (!reader.IsDBNull(8))
                                usrpro.createdby = reader.GetString(8);
                            if (!reader.IsDBNull(9))
                                usrpro.validity = reader.GetString(9);
                            if (!reader.IsDBNull(10))
                                usrpro.producttype = reader.GetString(10);

                        }
                    }
                }
            }
            return usrpro;
        }
    }
}