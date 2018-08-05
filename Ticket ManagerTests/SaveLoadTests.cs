using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ticket_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket_ManagerTests  
{
    [TestClass()]
    public class SavingLodingTests
    {
        [TestMethod()]
        public void CustomerSavingLoadingTest()
        {
            Customer NewCustomer = new Customer("0", "Bend ersgreat", false, "00000000004");
            NewCustomer.Save("TMCustomer_Tests.txt");
            Customer CustomerLoad = Customer.Load("TMCustomer_Tests.txt");
            string Name1 = CustomerLoad.GetNumber();
            string Name2 = NewCustomer.GetNumber();
            Assert.AreEqual(Name1, Name2);
        }

        [TestMethod()]
        public void PlaySavingLoadingTest()
        {
            Play NewPlay = new Play("0", "Simpson", "Disney", 0, DateTime.Now);
            NewPlay.Save("TMPlay_Tests.txt");
            Play playLoad = Play.Load("TMPlay_Tests.txt");
            string Name1 = playLoad.GetName();
            string Name2 = NewPlay.GetName();
            Assert.AreEqual(Name1, Name2);
        }

        [TestMethod()]
        public void TicketsSavingLoadingTest()
        {
            Tickets NewTicket = new Tickets("1", "BUE05", "2", 0, System.DateTime.Now, "3");
            NewTicket.Save("TMTickets_Tests.txt");
            Tickets TicketLoad = Tickets.Load("TMTickets_Tests.txt");
            string Name1 = TicketLoad.GetTicketID();
            string Name2 = NewTicket.GetTicketID();
            Assert.AreEqual(Name1, Name2);
        }

        [TestMethod()]
        public void GoldMemberSavingLoadingTest()
        {
            GoldMember NewGoldMember = new GoldMember("0", "Johnny Appleseed", true, "00000000000", "22 Paper Street", System.DateTime.Now.AddYears(1));
            NewGoldMember.Save("TMGoldMember_Tests.txt");
            GoldMember GoldMemberLoad = GoldMember.LoadGoldMember("TMGoldMember_Tests.txt");
            string Name1 = NewGoldMember.GetAddress();
            string Name2 = GoldMemberLoad.GetAddress();
            Assert.AreEqual(Name1, Name2);
        }

    }
}
