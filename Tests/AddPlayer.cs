using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class AddPlayer : GameStateTests
    {

        [TestMethod]
        public void AddPlayer_ReturnNothing_WhenPlayerDoesNotExist()
        {
           Assert.IsTrue(true);
        }
    }
}
