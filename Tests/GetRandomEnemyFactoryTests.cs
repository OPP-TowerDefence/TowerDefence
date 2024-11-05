using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class GetRandomEnemyFactoryTests : GameStateTests
    {
        [TestMethod]
        public void GetRandomEnemyFactory_ReturnsFastEnemyFactory_WhenEnemyTypeIs0()
        {
            //Arrange
            var method = typeof(GameState).GetMethod("GetRandomEnemyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act       
            var factory = method.Invoke(_gameState, new object[] { 0 });

            // Assert
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(FastEnemyFactory), "Expected FastEnemyFactory when enemyType is 0.");
        }

        [TestMethod]
        public void GetRandomEnemyFactory_ReturnsStrongEnemyFactory_WhenEnemyTypeIs1()
        {
            //Arrange
            var method = typeof(GameState).GetMethod("GetRandomEnemyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act         
            var factory = method.Invoke(_gameState, new object[] { 1 });

            // Assert
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(StrongEnemyFactory), "Expected StrongEnemyFactory when enemyType is 1.");
        }

        [TestMethod]
        public void GetRandomEnemyFactory_ReturnsFlyingEnemyFactory_WhenEnemyTypeIs2()
        {
            //Arrange
            var method = typeof(GameState).GetMethod("GetRandomEnemyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var factory = method.Invoke(_gameState, new object[] { 2 });

            // Assert
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(FlyingEnemyFactory), "Expected FlyingEnemyFactory when enemyType is 2.");
        }

        [TestMethod]
        public void GetRandomEnemyFactory_ThrowsException_WhenEnemyTypeIsInvalid()
        {
            //Arrange
            var method = typeof(GameState).GetMethod("GetRandomEnemyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act         
            var exception = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(_gameState, new object[] { 3 }),
                "A TargetInvocationException should be thrown due to invalid enemy type.");

            // Assert
            Assert.IsInstanceOfType(exception.InnerException, typeof(Exception), "Inner exception should be of type Exception.");
            Assert.AreEqual("Unknown enemy type", exception.InnerException.Message, "Exception message should match the expected message.");
        }

        [TestMethod]
        public void GetRandomEnemyFactory_UsesRandomEnemyType_WhenTestEnemyTypeIsNull()
        {
            //Arrange
            var method = typeof(GameState).GetMethod("GetRandomEnemyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act         
            var factory = method.Invoke(_gameState, new object[] { null });

            // Assert
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(IEnemyFactory), "Expected a factory implementing IEnemyFactory when using random enemyType.");
        }

        [TestMethod]
        public void GetRandomEnemyFactory_ReturnsSpecificFactory_WhenTestEnemyTypeIsProvided()
        {
            //Arrange
            var method = typeof(GameState).GetMethod("GetRandomEnemyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act & Assert         
            var factoryForType0 = method.Invoke(_gameState, new object[] { 0 });
            Assert.IsInstanceOfType(factoryForType0, typeof(FastEnemyFactory), "Expected FastEnemyFactory when enemyType is 0.");

            var factoryForType1 = method.Invoke(_gameState, new object[] { 1 });
            Assert.IsInstanceOfType(factoryForType1, typeof(StrongEnemyFactory), "Expected StrongEnemyFactory when enemyType is 1.");

            var factoryForType2 = method.Invoke(_gameState, new object[] { 2 });
            Assert.IsInstanceOfType(factoryForType2, typeof(FlyingEnemyFactory), "Expected FlyingEnemyFactory when enemyType is 2.");
        }
    }
}
