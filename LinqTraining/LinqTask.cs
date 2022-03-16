using LinqTraining.DoNotChange;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqTraining
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var result = customers.Where(customer => customer.Orders.Sum(order => order.Total) > limit);
            return result;
        }

        public static IEnumerable<(Customer, IEnumerable<Supplier>)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            if (suppliers is null)
            {
                throw new ArgumentNullException(nameof(suppliers));
            }

            var result = customers.Select(customer => (customer, suppliers.Where(supplier => supplier.City == customer.City && supplier.Country == customer.Country))).ToList();
            return result;
        }

        public static IEnumerable<(Customer, IEnumerable<Supplier>)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            if (suppliers is null)
            {
                throw new ArgumentNullException(nameof(suppliers));
            }

            var result = customers.Select(customer => (customer, suppliers.Where(supplier => supplier.City == customer.City && supplier.Country == customer.Country))).ToList();
            return result;
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var result = customers.Where(customer=>customer.Orders.Any(order=>order.Total>limit)).ToList();
            return result;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var result = customers.Where(customer => customer.Orders.Count() > 0).Select(customer => (customer, customer.Orders.Select(order => order.OrderDate).Min()));
            return result;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var result = customers.Where(customer => customer.Orders.Count() > 0).Select(customer => (customer, customer.Orders.Select(order => order.OrderDate).Min())).OrderBy(customer=>customer.Item2.Year).ThenBy(customer=>customer.Item2.Month).ThenByDescending(customer=>customer.customer.Orders.Sum(order=>order.Total)).ThenBy(customer=>customer.customer.CompanyName);
            return result;
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            int parseResult;
            var result = customers.Where(customer => !customer.Phone.StartsWith('(') || !int.TryParse(customer.PostalCode,out parseResult) || string.IsNullOrWhiteSpace(customer.Region)).ToList();
            return result;
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            if (products is null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            var result = products.GroupBy(product => product.Category).
                Select(categoryGroup => new Linq7CategoryGroup()
                {
                    Category = categoryGroup.Key,
                    UnitsInStockGroup = categoryGroup.
                    OrderByDescending(group => group.UnitsInStock).
                    GroupBy(u => u.UnitsInStock, p => p.UnitPrice).
                    Select(x => new Linq7UnitsInStockGroup() { UnitsInStock = x.Key, Prices = x })
                });
            return result;

        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            if (products is null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            var result = new List<(decimal, IEnumerable<Product>)>();
            var cheapProducts = products.Where(product => product.UnitPrice <= cheap);
            result.Add((cheap, cheapProducts));

            var middleProducts = products.Where(product=>product.UnitPrice <= middle && product.UnitPrice>cheap);
            result.Add((middle, middleProducts));

            var expensiveProducts = products.Where(product => product.UnitPrice <= expensive && product.UnitPrice > middle);
            result.Add((expensive, expensiveProducts));
            return result;
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var result2 = customers.GroupBy(customer => customer.City, customer => customer.Orders).Select(grouping => (grouping.Key,(int)Math.Round(customers.Where(customer=>customer.City==grouping.Key).Select(customer=>customer.Orders.Sum(sum=>sum.Total)).Average()), (int)customers.Where(customer => customer.City == grouping.Key).Select(customer=>customer.Orders.Count()).Average())); 
            return result2;
            
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            if (suppliers is null)
            {
                throw new ArgumentNullException(nameof(suppliers));
            }

            string result = string.Concat((suppliers.OrderBy(supplier => supplier.Country.Length).ThenBy(supplier => supplier.Country.Substring(0, 1)).Select(supplier => supplier.Country).Distinct()));
            return result;
        }
    }
}
