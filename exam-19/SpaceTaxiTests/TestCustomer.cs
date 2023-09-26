using System.Collections.Generic;
using System.IO;
using System.Threading;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi.Entities;

namespace SpaceTaxiTests {
    public class TestCustomer {
        private Customer customer;
        private Customer customer2;
        private CustomerGroup customerGroup;
        private List<string> customerInfo;
        private List<Customer> customerList;
        private EntityContainer<LevelEntities> levelEntities;

        [SetUp]
        public void CustomerSetUp() {
            Window.CreateOpenGLContext();
            levelEntities = new EntityContainer<LevelEntities>();
            levelEntities.AddStationaryEntity(new LevelEntities(new StationaryShape(
                    new Vec2F(0.8f, 0.8f), new Vec2F(0.2f, 0.2f)),
                new Image(Path.Combine("Assets", "Images", "neptune-square.png")), '1',
                true));
            customerInfo = new List<string>();
            customerInfo.Add("Bob 10 1 r 10 100");
            customerInfo.Add("Carol 30 1 ^ 10 100");
            customer = new Customer("Cami", -1, '1',
                "^J", 10, 100, new StationaryShape(
                    new Vec2F(0.5f, 0.5f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "CustomerStandRight.png")));
            customer2 = new Customer("Cami", 1, '1',
                "^J", 10, 100, new StationaryShape(
                    new Vec2F(0.5f, 0.5f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "CustomerStandRight.png")));
            customerGroup = new CustomerGroup(levelEntities);
            customerList = new List<Customer>();
            customerList.Add(customer);
        }

        /// <summary>
        ///     Tests if the bool isRendered has been changed after a call to RenderEntity
        /// </summary>
        [Test]
        public void TestRenderEntity() {
            Assert.IsFalse(customer.IsRendered);
            customer.RenderCustomer();
            Assert.IsTrue(customer.IsRendered);
        }

        /// <summary>
        ///     Tests that the bool isRendered has not been changed if the customers spawnTime has not
        ///     yet been surpassed
        /// </summary>
        [Test]
        public void TestRenderEntity2() {
            Assert.IsFalse(customer2.IsRendered);
            customer2.RenderCustomer();
            Assert.IsFalse(customer2.IsRendered);
        }

        /// <summary>
        ///     Tests that the customer is not rendered if the customer has been picked up
        /// </summary>
        [Test]
        public void TestRenderEntity3() {
            Assert.IsFalse(customer.IsRendered);
            customer.PickedUp = true;
            customer.RenderCustomer();
            Assert.IsFalse(customer.IsRendered);
        }


        /// <summary>
        ///     Tests that the bool PickedUp has been changed after a call to beingPickedUp.
        /// </summary>
        [Test]
        public void TestBeingPickedUp() {
            Assert.IsFalse(customer.PickedUp);
            customer.BeingPickedUp();
            Assert.IsTrue(customer.PickedUp);
        }

        /// <summary>
        ///     Tests that the customers DropOffTimer starts when it is picked up
        /// </summary>
        [Test]
        public void TestBeingPickedUpDropOffTime() {
            Assert.IsFalse(customer.PickedUp);
            customer.BeingPickedUp();
            Thread.Sleep(1000);
            Assert.AreNotEqual(0f, customer.DropOffTimer.ElapsedMilliseconds);
        }

        /// <summary>
        ///     customerInfo is a list of two strings, which CreateCustomers converts into customers, and
        ///     adds them to the EntityContainer Customers. This tests if the customers are created and
        ///     added to the EntityContainer.
        /// </summary>
        [Test]
        public void TestCreateCustomers() {
            customerGroup.CreateCustomers(customerInfo);
            Assert.AreEqual(2, customerGroup.Customers.CountEntities());
        }

        /// <summary>
        ///     This tests if AddExistingCustomers adds the customers in a list to the EntityContainer
        ///     Customers.
        /// </summary>
        [Test]
        public void TestAddExistingCustomer() {
            customerGroup.AddExistingCustomers(customerList);
            Assert.AreEqual(1, customerGroup.Customers.CountEntities());
        }

        /// <summary>
        ///     Tests if all the customers in an EntityContainer are rendered after a call to
        ///     RenderCustomers.
        /// </summary>
        [Test]
        public void TestRenderCustomers() {
            customerGroup.RenderCustomers();
            foreach (Customer customer1 in customerGroup.Customers) {
                Assert.IsTrue(customer1.IsRendered);
            }
        }
    }
}