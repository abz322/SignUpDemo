using SignUpDemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SignUpDemo.Controllers
{
    public class HomeController : Controller
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        // GET: Home

        public ActionResult Test()
        {
            List<UserDetails> userList = GetUserDetails();
            ViewBag.UserList = userList;
            return PartialView();
        }

        public ActionResult Index()
        {
            ModelState.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserDetails model)
        {
            Result result = new Result();
            try
            {
                if (ModelState.IsValid)
                {
                    //Regex is only ideal for simple email validation, so perform a in depth validation
                    if (!ValidateEmailAddress(model.Email))
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "This email is not valid.";
                        throw new Exception();
                    }

                    //The password needs to be stored securely, using a hash value over encryption is the best way to do this
                    //to avoid exposing an encryption key for the passwords
                    byte[] hashedPassword = HashPassword(model.Password);

                    if (hashedPassword == null)
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "An error occurred, please try again.";
                        throw new Exception();
                    }

                    var userDet = new UserDetails();

                    List<UserDetails> userDetails = GetUserDetails(model.Email);

                    bool userExists = userDetails.Any();

                    if (userExists)
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "This user already exists.";
                        throw new Exception();
                    }

                    int addUserResult = AddUserDetails(model.Email, hashedPassword);

                    if (addUserResult == 0)
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "An error occurred, please try again.";
                        throw new Exception();
                    }

                    ModelState.Clear();

                    result.Status = (int)HttpStatusCode.OK;
                    result.Message = "User was successfully created.";

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.Status = (int)HttpStatusCode.BadRequest;
                    result.Message = "The information entered could not be validated.";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        private int AddUserDetails(string email, byte[] password)
        {
            int result = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string sql = "INSERT INTO [dbo].[UserDetails](Email,Password) VALUES(@email,@password)";
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

        private List<UserDetails> GetUserDetails(string email = null)
        {
            List<UserDetails> userList = new List<UserDetails>();
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd;
                    if(email == null)
                    {
                        cmd = new SqlCommand("SELECT * FROM [dbo].[UserDetails]", con);
                    }
                    else
                    {
                        cmd = new SqlCommand("SELECT * FROM [dbo].[UserDetails] WHERE Email = @email", con);
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

        private byte[] HashPassword(string password)
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

        private bool ValidateEmailAddress(string emailAddress)
        {
            try
            {
                MailAddress address = new MailAddress(emailAddress);
                if(address.Address == emailAddress)
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

        private bool VerifyPassword(byte[] passToVerify, byte[] storedPass)
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