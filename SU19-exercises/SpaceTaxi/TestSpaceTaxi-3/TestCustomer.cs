using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi_3.Entities;

namespace TestSpaceTaxi_3 {
    public class TestCustomer {
        private Customer customer;
        private CustomerGroup customerGroup;
        private EntityContainer<LevelEntities> levelEntities;
        private List<string> customerInfo;
        private List<Customer> customerList;

        [SetUp]
        public void CustomerSetUp() {
            Window.CreateOpenGLContext();
            levelEntities = new EntityContainer<LevelEntities>();
            levelEntities.AddStationaryEntity(new LevelEntities(new StationaryShape(
                new Vec2F(0.8f,0.8f),new Vec2F(0.2f,0.2f)),
                new Image(Path.Combine("Assets","Images","neptune-square.png")),'1',
                true));
            customerInfo = new List<string>();
            customerInfo.Add("Bob 10 1 r 10 100");
            customerInfo.Add("Carol 30 1 ^ 10 100");
            customer = new Customer("Cami",-1,'1',
                "^J",10,100,new StationaryShape(
                    new Vec2F(0.5f,0.5f),new Vec2F(0.1f,0.1f)),
                new Image(Path.Combine("Assets", "Images","CustomerStandRight.png")));
            customerGroup = new CustomerGroup(levelEntities);
            customerList = new List<Customer>();
            customerList.Add(customer);
        }

        // Tests if the bool isRendered has been changed after a call to RenderEntity 
        [Test]
        public void TestRenderEntity() {
            Assert.IsFalse(customer.IsRendered);
            customer.RenderEntity();
            Assert.IsTrue(customer.IsRendered);
        }

        // Tests that the bool PickedUp has been changed after a call to beingPickedUp. 
        [Test]
        public void TestBeingPickedUp() {
            Assert.IsFalse(customer.PickedUp);
            customer.BeingPickedUp();
            Assert.IsTrue(customer.PickedUp);
        }

        // customerInfo is a list of two strings, which CreateCustomers converts into customers, and 
        // adds them to the EntityContainer Customers. This tests if the customers are created and 
        // added to the EntityContainer. 
        [Test]
        public void TestCreateCustomers() {
            customerGroup.CreateCustomers(customerInfo);
            Assert.AreEqual(2,customerGroup.Customers.CountEntities());
        }

        
        // This tests if AddExistingCustomers adds the customers in a list to the EntityContainer 
        // Customers. 
        [Test]
        public void TestAddExistingCustomer() {
            customerGroup.AddExistingCustomers(customerList);
            Assert.AreEqual(1,customerGroup.Customers.CountEntities());
        }

        // Tests if all the customers in an EntityContainer are rendered after a call to 
        // RenderCustomers. 
        [Test]
        public void TestRenderCustomers() {
            customerGroup.RenderCustomers();
            foreach (Customer customer1 in customerGroup.Customers) {
                Assert.IsTrue(customer1.IsRendered);
            }
        }
    }
}