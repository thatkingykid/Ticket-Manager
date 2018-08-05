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
    public class GoldMemberTests
    {
        [TestMethod()]
        public void GoldMemberTest1()
        {
            GoldMember newCustomer = new GoldMember("0", "Johnny Appleseed", true, "00000000000", "22 Paper Street", System.DateTime.Now.AddYears(1));
            //Assert that the new customer is an object in memory
            Assert.AreNotEqual(null, newCustomer);
        }
        [TestMethod()]
        public void GetAddressTest()
        {
            GoldMember newCustomer = new GoldMember("0", "Johnny Appleseed", true, "00000000000", "22 Paper Street", System.DateTime.Now.AddYears(1));
            string returnedAddress = newCustomer.GetAddress();
            Assert.AreEqual("22 Paper Street", returnedAddress);
        }

        [TestMethod()]
        public void GetRenewalDateTest()
        {
            string dateTimeInput = "21/05/2017";
            DateTime renewalDate = Convert.ToDateTime(dateTimeInput);
            GoldMember newCustomer = new GoldMember("0", "Johnny Appleseed", true, "00000000000", "22 Paper Street", renewalDate);
            Assert.AreEqual(renewalDate, newCustomer.GetRenewalDate());
        }
        [TestMethod()]
        public void GoldMemberIsGoldTest()
        {
            GoldMember newCustomer = new GoldMember("0", "Johnny Appleseed", true, "00000000000", "22 Paper Street", System.DateTime.Now.AddYears(1));
            Assert.AreEqual(true, newCustomer.IsGold());
        }
    }
}