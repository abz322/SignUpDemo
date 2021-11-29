using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using SignUpDemo.Models;
using System.Configuration;

namespace SignUpDemo.Controllers
{
    public class Utils
    {
        public int AddUserDetails(string email, byte[] password)
        {
            int result = 0;
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string sql = "INSERT INTO [UserDetails] (Email,Password) VALUES(@email,@password)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = email;
                        cmd.Parameters.Add("@password", SqlDbType.Binary, 32).Value = password;
                        cmd.CommandType = CommandType.Text;
                        result = cmd.ExecuteNonQuery();
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }

        public List<UserDetails> GetUserDetails(string email = null)
        {
            List<UserDetails> userList = new List<UserDetails>();
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd;
                    if (email == null)
                    {
                        cmd = new SqlCommand("SELECT * FROM [UserDetails]", con);
                    }
                    else
                    {
                        cmd = new SqlCommand("SELECT * FROM [UserDetails] WHERE Email = @email", con);
                        cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = email;
                    }
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var userDet = new UserDetails();

                        userDet.UserID = Convert.ToInt32(rdr["UserID"]);
                        userDet.Email = rdr["Email"].ToString();
                        byte[] hashValue = (byte[])rdr["Password"];
                        StringBuilder hashResult = new StringBuilder();
                        for (int i = 0; i < hashValue.Length; i++)
                        {
                            hashResult.Append(hashValue[i].ToString("x2"));
                        }
                        userDet.Password = hashResult.ToString();
                        userList.Add(userDet);
                    }
                }
                return userList;
            }
            catch
            {
                return userList;
            }
        }

        public byte[] HashPassword(string password)
        {
            using (SHA256 shaHasher = SHA256.Create())
            {
                try
                {
                    byte[] hashValue = shaHasher.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return hashValue;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool ValidateEmailAddress(string emailAddress)
        {
            try
            {
                MailAddress address = new MailAddress(emailAddress);
                if (address.Address == emailAddress)
                {
                    return true;
                }
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool VerifyPassword(byte[] passToVerify, byte[] storedPass)
        {
            try
            {
                if (passToVerify.SequenceEqual(storedPass))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}