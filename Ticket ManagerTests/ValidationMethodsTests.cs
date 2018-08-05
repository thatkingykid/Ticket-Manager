using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ticket_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket_Manager.Tests
{
    [TestClass()]
    public class ValidationMethodsTests
    {
        [TestMethod()]
        public void IsNumericTest()
        {
            //first test is an invalid input
            bool firstResult = ValidationMethods.IsNumeric("Hello, World!");
            Assert.AreEqual(false, firstResult);

            //second test is a valid input
            bool secondResult = ValidationMethods.IsNumeric("22");
            Assert.AreEqual(true, secondResult);
        }

        [TestMethod()]
        public void ValidDateTest()
        {
            //dates in the system are validated off whether they exist within
            //the known business dates of the theatre

            //test 1: invalid, happens outside the known business dates
            string dateTimeInput = "21/03/2016";
            DateTime dateTime = Convert.ToDateTime(dateTimeInput);
            Assert.AreEqual(false, ValidationMethods.ValidDate(dateTime));

            //test 2: extreme, happens on one of the boundaries for a valid date
            string secondDateTimeInput = "21/12/2016";
            DateTime secondDateTime = Convert.ToDateTime(secondDateTimeInput);
            Assert.AreEqual(true, ValidationMethods.ValidDate(secondDateTime));

            //test 3: valid, is a valid date
            string validInput = "21/06/2016";
            DateTime validDateTime = Convert.ToDateTime(validInput);
            Assert.AreEqual(true, ValidationMethods.ValidDate(validDateTime));
        }

        [TestMethod()]
        public void ValidPhoneNumberTest()
        {
            //test 1: invalid, input is a string literal with no numbers
            Assert.AreEqual(false, ValidationMethods.ValidPhoneNumber("Hello, World"));

            //test 2: invalid, input is too long to be a phone number
            Assert.AreEqual(false, ValidationMethods.ValidPhoneNumber("012345678901"));

            //test 3: valid, input is a correctly formatted phone number
            Assert.AreEqual(true, ValidationMethods.ValidPhoneNumber("01234567890"));
        }

        [TestMethod()]
        public void HasInputTest()
        {
            //test 1: invalid, input is null
            Assert.AreEqual(false, ValidationMethods.HasInput(null));

            //test 2: invalid, input is nothing
            Assert.AreEqual(false, ValidationMethods.HasInput(""));

            //test 3: invalid, input is nothing but spaces
            Assert.AreEqual(false, ValidationMethods.HasInput("   "));

            //test 4: valid, input is a valid string input
            Assert.AreEqual(true, ValidationMethods.HasInput("Hello, World!"));
        }
    }
}