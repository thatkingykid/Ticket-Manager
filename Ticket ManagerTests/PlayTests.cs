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
    public class PlayTests
    {
        [TestMethod()]
        public void PlayConstructorNullTest()
        {
            Play testPlay = new Play(null, null, null, 0, DateTime.Now);
            Assert.AreNotEqual("Golden Egg", testPlay.GetName());
        }
    }
}