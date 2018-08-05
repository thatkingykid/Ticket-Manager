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
    public class TicketsTests
    {
        [TestMethod()]
        public void ReservationTest()
        {
            Play newPlay = new Play("0", "Golden Egg", "Intel", 0, System.DateTime.Now);
            Customer newCustomer = new Customer("0", "Jack Morley", false, "00000000000");
            Tickets testTicket = new Tickets("0", "BUE05", "0", 0, System.DateTime.Now.AddDays(-1), "0");
            Assert.AreEqual(false, testTicket.IsConfirmed());
        }
    }
}