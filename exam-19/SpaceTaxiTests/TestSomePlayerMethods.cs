using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.Entities;
using SpaceTaxi.States;
using SpaceTaxi.Taxi;

namespace SpaceTaxiTests {
    public class TestSomePlayerMethods {
        private Customer customer;
        private List<Customer> customerList;
        private Game game;
        private CustomerGroup Group;
        private string level;
        private EntityContainer<LevelEntities> levelEntities;
        public Player player;
        private StateMachine stateMachine;


        [SetUp]
        public void PlayerSetUp() {
            Window.CreateOpenGLContext();
            player = new Player();
            levelEntities = new EntityContainer<LevelEntities>();
            levelEntities.AddStationaryEntity(new LevelEntities(new StationaryShape(
                    new Vec2F(0.8f, 0.8f), new Vec2F(0.2f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "neptune-square.png")), '1',
                true));
            levelEntities.AddStationaryEntity(new LevelEntities(new StationaryShape(
                    new Vec2F(0.5f, 0.5f), new Vec2F(0.2f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "neptune-square.png")), 'l',
                true));
            customer = new Customer("Cami", -1, '1',
                "^J", 10, 100, new StationaryShape(
                    new Vec2F(0.8f, 0.9f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "CustomerStandRight.png")));
            level = "the-beach.txt";
            stateMachine = new StateMachine();
            Group = new CustomerGroup(levelEntities);
            customerList = new List<Customer>();
            customerList.Add(customer);
            Group.AddExistingCustomers(customerList);
        }


        /// <summary>
        ///     When levelCollision is called the player is given a direction based on its velocity,
        ///     which is why we first set its direction before calling LevelCollision. Then we test
        ///     if the direction has been changed.
        /// </summary>
        [Test]
        public void TestLevelCollisionDirectionChange() {
            player.SetPosition(0.5f, 0.5f);
            player.Entity.Shape.AsDynamicShape().Direction = new Vec2F(0.1f, 0.0f);
            player.VelocityX = new Vec2F(0.2f, 0.0f);
            player.VelocityY = new Vec2F(0.0f, -0.001f);
            levelEntities.Iterate(player.LevelCollision);
            Assert.IsTrue(0.2f == player.Entity.Shape.AsDynamicShape().Direction.X
                          && -0.001f == player.Entity.Shape.AsDynamicShape().Direction.Y);
        }

        /// <summary>
        ///     If the player collides with a platform, the direction is set to 0. This test forces a
        ///     collision with a platform and checks if the direction has been set to 0.
        /// </summary>
        [Test]
        public void TestLevelCollisionPlatformLanding() {
            player.SetPosition(0.8f, 0.9f);
            player.VelocityX = new Vec2F(0.2f, 0.0f);
            player.VelocityY = new Vec2F(0.0f, -0.001f);
            levelEntities.Iterate(player.LevelCollision);
            Assert.IsTrue(0.0f == player.Entity.Shape.AsDynamicShape().Direction.X
                          && 0.0f == player.Entity.Shape.AsDynamicShape().Direction.Y);
        }

        /// <summary>
        ///     If the player collides too hard with a platform , it explodes and is deleted.
        ///     This forces a collision with a velocity that is too high and checks if the player is
        ///     deleted.
        /// </summary>
        [Test]
        public void TestLevelCollisionExplosion() {
            player.SetPosition(0.8f, 0.9f);
            player.VelocityX = new Vec2F(0.2f, 0.0f);
            player.VelocityY = new Vec2F(0.0f, -0.2f);
            levelEntities.Iterate(player.LevelCollision);
            Assert.IsTrue(player.Entity.IsDeleted());
        }

        /// <summary>
        ///     If a player collides with a customer and is landed on a platform, the customer should be
        ///     picked up. This tests that.
        /// </summary>
        [Test]
        public void TestCustomerCollision() {
            player.SetPosition(0.75f, 0.85f);
            player.VelocityX = new Vec2F(0.2f, 0.0f);
            player.VelocityY = new Vec2F(0.0f, -0.001f);
            levelEntities.Iterate(player.LevelCollision);
            customer.RenderCustomer();
            Group.Customers.Iterate(player.CustomerCollision);
            Assert.IsTrue(customer.PickedUp);
        }

        /// <summary>
        ///     Tests the same as TestLevelCollisionPlatformLanding but with a customer inside
        /// </summary>
        [Test]
        public void TestLevelCollisionPlatformLandingWithCustomer() {
            player.SetPosition(0.8f, 0.9f);
            player.VelocityX = new Vec2F(0.2f, 0.0f);
            player.VelocityY = new Vec2F(0.0f, -0.001f);
            levelEntities.Iterate(player.LevelCollision);
            Assert.IsTrue(0.0f == player.Entity.Shape.AsDynamicShape().Direction.X
                          && 0.0f == player.Entity.Shape.AsDynamicShape().Direction.Y);
        }

        /// <summary>
        ///     Tests that the level can be changed through a call to ChangeLevel
        /// </summary>
        [Test]
        public void TestChangeLevel() {
            Console.WriteLine(level);
            level = player.ChangeLevel(level);
            Console.WriteLine(level);
            Assert.AreEqual("short-n-sweet.txt", level);
        }


        /// <summary>
        ///     When the level is changed the player is given a position, so it won't die immediately
        ///     after changing the level. This method tests if the player is given the correct position
        ///     after a level change.
        /// </summary>
        [Test]
        public void TestChangeLevelPlayerPosition() {
            player.SetPosition(0.2f, 0.2f);
            player.ChangeLevel(level);
            Assert.IsTrue(0.5f == player.Entity.Shape.Position.X
                          && 0.85f == player.Entity.Shape.Position.Y);
        }

        /// <summary>
        ///     Just as the position is changed after a level change, so is the velocity. This tests the
        ///     players velocity after a changing of the level.
        /// </summary>
        [Test]
        public void TestChangeLevelPlayerVelocity() {
            player.ChangeLevel(level);
            Assert.IsTrue(0.0f == player.VelocityX.X && -0.001f == player.VelocityY.Y);
        }

        /// <summary>
        ///     Tests that the method SetExtent can set the extent of the player
        /// </summary>
        [Test]
        public void TestSetExtent() {
            player.SetExtent(0.5f, 0.4f);
            Assert.AreEqual(0.5f, player.Entity.Shape.Extent.X);
            Assert.AreEqual(0.4f, player.Entity.Shape.Extent.Y);
        }

        /// <summary>
        ///     Tests that the method SetPosition can set the position of the player
        /// </summary>
        [Test]
        public void TestSetPosition() {
            player.SetPosition(0.5f, 0.4f);
            Assert.AreEqual(0.5f, player.Entity.Shape.Position.X);
            Assert.AreEqual(0.4f, player.Entity.Shape.Position.Y);
        }
    }
}