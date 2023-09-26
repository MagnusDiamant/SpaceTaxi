using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi_3.Entities {
    public class CustomerGroup : ICustomerGroup {
        private string[] customerArray;
        private EntityContainer<LevelEntities> levelEntities;
        private Customer newCustomer;

        public CustomerGroup(EntityContainer<LevelEntities> levelEntities) {
            Customers = new EntityContainer<Customer>();
            this.levelEntities = levelEntities;
        }

        public EntityContainer<Customer> Customers { get; }

        /// <summary>
        ///     Adds customers to the EntityContainer Customers using the StringToCustomer method.
        /// </summary>
        /// <param name="customerInfo"> A list of strings</param>
        public void CreateCustomers(List<string> customerInfo) {
            foreach (var line in customerInfo) {
                Customers.AddStationaryEntity(StringToCustomer(line));
            }
        }

        /// <summary>
        ///     Converts a string to a customer.
        /// </summary>
        /// <param name="info"> The string containing info about the customer</param>
        /// <returns> A customer </returns>
        private Customer StringToCustomer(string info) {
            customerArray = info.Split(' ');
            int spawnTime;
            char spawnPlatform;
            int dropOffTime;
            int points;
            // The following lines are used to cast the split strings to the appropriate types. 
            int.TryParse(customerArray[1], out spawnTime);
            char.TryParse(customerArray[2], out spawnPlatform);
            int.TryParse(customerArray[4], out dropOffTime);
            int.TryParse(customerArray[5], out points);
            newCustomer = new Customer(customerArray[0],
                spawnTime, spawnPlatform,
                customerArray[3], dropOffTime,
                points, new StationaryShape(SpawnPosition(spawnPlatform),
                    new Vec2F(0.05f, 0.05f)), new Image(Path.Combine("Assets",
                    "Images", "CustomerStandRight.png")));
            return newCustomer;
        }

        /// <summary>
        ///     Adds customers in the taxi to the new level, should it change.
        /// </summary>
        /// <param name="existingCustomers"> A list containing all the customers in the taxi</param>
        public void AddExistingCustomers(List<Customer> existingCustomers) {
            foreach (var customer in existingCustomers) {
                Customers.AddStationaryEntity(customer);
            }
        }

        /// <summary>
        ///     Renders all customers in the EntityContainer, if the conditions in the method
        ///     RenderEntity are fulfilled.
        /// </summary>
        public void RenderCustomers() {
            foreach (Customer customer in Customers) {
                customer.RenderEntity();
            }
        }

        /// <summary>
        ///     Finds the position in which the customer should be spawned by comparing the customers
        ///     SpawnPlatform and the levelEntity.Character. If they are the same, a vector with the
        ///     position right above the platform and a little to the right og the specific platform-
        ///     image is returned.
        /// </summary>
        /// <param name="platform"> The character representing the customer SpawnPlatform</param>
        /// <returns> A vector containing the position of the customer </returns>
        /// <exception cref="Exception">
        ///     If a level does not contain said SpawnPlatform
        ///     an exception is thrown
        /// </exception>
        private Vec2F SpawnPosition(char platform) {
            foreach (LevelEntities levelEntity in levelEntities) {
                if (platform == levelEntity.Character) {
                    return new Vec2F(
                        levelEntity.Shape.Position.X + 2.0f * levelEntity.Shape.Extent.X
                        , levelEntity.Shape.Position.Y + levelEntity.Shape.Extent.Y);
                }
            }

            throw new Exception("No platform to spawn on");
        }
    }
}