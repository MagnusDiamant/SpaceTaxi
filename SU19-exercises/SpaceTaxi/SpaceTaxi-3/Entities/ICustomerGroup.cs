using System.Collections.Generic;
using DIKUArcade.Entities;

namespace SpaceTaxi_3.Entities {
    public interface ICustomerGroup {
        EntityContainer<Customer> Customers { get; }

        void CreateCustomers(List<string> customerInfo);
    }
}