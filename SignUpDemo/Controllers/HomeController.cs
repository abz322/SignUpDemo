using SignUpDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SignUpDemo.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Test()
        {
            Utils ut = new Utils();
            List<UserDetails> userList = ut.GetUserDetails();
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
            Utils ut = new Utils();
            Result result = new Result();
            try
            {
                if (ModelState.IsValid)
                {
                    //Regex is only ideal for simple email validation, so perform a in depth validation
                    if (!ut.ValidateEmailAddress(model.Email))
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "This email is not valid.";
                        throw new Exception();
                    }

                    //The password needs to be stored securely, using a hash value over encryption is the best way to do this
                    //to avoid exposing an encryption key for the passwords
                    byte[] hashedPassword = ut.HashPassword(model.Password);

                    if (hashedPassword == null)
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "An error occurred, please try again.";
                        throw new Exception();
                    }

                    var userDet = new UserDetails();

                    List<UserDetails> userDetails = ut.GetUserDetails(model.Email);

                    bool userExists = userDetails.Any();

                    if (userExists)
                    {
                        result.Status = (int)HttpStatusCode.BadRequest;
                        result.Message = "This user already exists.";
                        throw new Exception();
                    }

                    int addUserResult = ut.AddUserDetails(model.Email, hashedPassword);

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

        
    }
}
