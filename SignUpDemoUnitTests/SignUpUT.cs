using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SignUpDemo.Controllers;
using System.Text;

namespace SignUpDemo.Tests
{
    [TestClass]
    public class SignUpUT
    {
        [TestMethod]
        public void TestVerifyPassword()
        {
            Utils ut = new Utils();

            //Byte array for the string "test"
            byte[] byteArrTest = new byte[] { 116, 101, 115, 116 };
            string stringTest = "test";

            bool result = ut.VerifyPassword(Encoding.UTF8.GetBytes(stringTest), byteArrTest);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestHashPassword()
        {
            Utils ut = new Utils();
            string password = "Test1234";
            string hashedPassword = "07480fb9e85b9396af06f006cf1c95024af2531c65fb505cfbd0add1e2f31573";

            byte[] byteArr = ut.HashPassword(password);
            StringBuilder hashResult = new StringBuilder();
            for (int i = 0; i < byteArr.Length; i++)
            {
                hashResult.Append(byteArr[i].ToString("x2"));
            }

            string result = hashResult.ToString();

            Assert.AreEqual(hashedPassword, result);
        }

        [TestMethod]
        public void TestValidateEmail()
        {
            Utils ut = new Utils();
            string email = "test@test.co.uk";
            bool result = ut.ValidateEmailAddress(email);

            Assert.IsTrue(result);
        }
    }
}
