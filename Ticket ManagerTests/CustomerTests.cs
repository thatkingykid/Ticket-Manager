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
    public class CustomerTests
    {
        [TestMethod()]
        //leave this line if you expect an exception in your test, 
        //though if you include it where it doesn't crash it'll crash
        //[ExpectedException(typeof (Exception))]
        public void CustomerNullConstructorTest()
        {
            Customer testCustomer = new Customer(null, null, false, null);

            //constructor will never create a null object because 
            //it needs a boolean value to flag whether the new object 
            //is a gold member or not
            Assert.AreNotEqual(null, testCustomer);
        }

        [TestMethod()]
        public void GetNameTest()
        {
            Customer test = new Customer("0", "Jack Morley", false, "01234567890");
            Assert.AreEqual("Jack Morley", test.GetName());
        }

        [TestMethod()]
        public void GetNumberTest()
        {
            Customer test = new Customer("0", "Jack Morley", false, "01234567890");
            Assert.AreEqual("01234567890", test.GetNumber());
        }

        [TestMethod()]
        public void IsGoldTest()
        {
            Customer test = new Customer("0", "Jack Morley", false, "01234567890");
            Assert.AreEqual(false, test.IsGold());
        }
    }
}