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
    public class BusinessTests
    {
        [TestMethod()]
        public void EditPlayNameTest()
        {
            Business newInstance = new Business();
            newInstance.AddPlay("Golden Egg", "Intel", 0, DateTime.Now);
            newInstance.EditPlay("0", "Silver Egg", "Intel", 0, DateTime.Now);
            Play returnResult = newInstance.FetchPlayByIndex("0");
            Assert.AreEqual("Silver Egg", returnResult.GetName());
        }
        [TestMethod()]
        public void EditPlayPriceTest()
        {
            Business newInstance = new Business();
            newInstance.AddPlay("Golden Egg", "Intel", 0, DateTime.Now);
            newInstance.EditPlay("0", "Golden Egg", "Intel", (PRICE_RANGE)2, DateTime.Now);
            Play returnResult = newInstance.FetchPlayByIndex("0");
            Assert.AreEqual(2, (int)returnResult.GetPrice());
        }

        [TestMethod()]
        public void CancelBookingsTest()
        {
            Business newInstance = new Business();
            newInstance.AddPlay("Golden Egg", "Intel", 0, DateTime.Now);
            Customer newCustomer = new Customer("0", "Jack Morley", false, "00000000000");
            List<string> location = new List<string> { "BUE05" };
            newInstance.AddBooking("0", "0", location.Count, location);
            Assert.AreEqual(true, newInstance.CancelBookings("0", "0"));
        }

        [TestMethod()]
        public void ConfirmBookingTest()
        {
            Business newInstance = new Business();
            newInstance.AddPlay("Golden Egg", "Intel", 0, DateTime.Now);
            Customer newCustomer = new Customer("0", "Jack Morley", false, "00000000000");
            List<string> location = new List<string> { "BUE05" };
            newInstance.AddBooking("0", "0", location.Count, location);
            newInstance.ConfirmBooking("0", "0");
            List<Tickets> returnTicket = newInstance.FetchTicketByCustomer("0");
            Assert.AreEqual(true, returnTicket[0].IsConfirmed());
        }

        [TestMethod()]
        public void GetScheduleReportTest()
        {
            Business newInstance = new Business();
            newInstance.AddPlay("Golden Egg", "Intel", 0, DateTime.Now.AddMonths(2));
            newInstance.AddPlay("Golden Egg", "Intel", 0, DateTime.Now.AddMonths(8));
            List<Play> returnedOutput = newInstance.GetScheduleReport();
            Assert.AreEqual(1, returnedOutput.Count);
        }
    }
}