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

            var result = new List<(Customer, IEnumerable<Supplier>)>();
            foreach (var customer in customers)
            {
                result.Add((customer, suppliers.Where(x => x.Country == customer.Country && x.City == customer.City)));
            }
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

            var query = from customer in customers
                        from supplier in suppliers
                        group suppliers by customer into customerSuppliers
                        select new
                        {
                            Customer = customerSuppliers.Key,
                            Suppliers = from suppliers in customerSuppliers
                                        from supplier in suppliers
                                        where supplier.Country == customerSuppliers.Key.Country && supplier.City == customerSuppliers.Key.City
                                        select supplier
                        }; 

            var result = new List<(Customer, IEnumerable<Supplier>)>();
            foreach (var item in query)
            {
                result.Add((item.Customer, item.Suppliers));
            }
            return result;
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var query = customers.Where(customer=>customer.Orders.Any(order=>order.Total>limit)).ToList();
            return query;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            var result = new List<(Customer, DateTime)>();

            var query = from customer in customers
                         where customer.Orders.Count() > 0
                         group customer.Orders by customer into customerOrders
                         select new
                         {
                             Customer = customerOrders.Key,
                             DateOfEntry = (from ordersArr in customerOrders
                                           from order in ordersArr
                                           select order.OrderDate).ToArray().Min()                                                                                     
                         };

            foreach (var item in query)
            {
                result.Add((item.Customer, item.DateOfEntry));
            }
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

            var result = new List<(Customer, DateTime)>();

            var query = from customer in customers
                        where customer.Orders.Count() > 0
                        from order in customer.Orders
                        orderby order.OrderDate.Year, order.OrderDate.Month, customer.Orders.Sum(o => o.Total) descending, customer.CompanyName
                        group customer.Orders by customer into customerOrders
                        select new
                        {
                            Customer = customerOrders.Key,
                            DateOfEntry = (from orders in customerOrders
                                           from order in orders
                                           select order.OrderDate).ToArray().Min()
                        };

            foreach (var item in query)
            {
                result.Add((item.Customer, item.DateOfEntry));
            }
            return result;
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            if (customers is null)
            {
                throw new ArgumentNullException(nameof(customers));
            }
            
            bool GetValidation(string postalCode,string region, string phone)
            {
                int parseResult;
                if(int.TryParse(postalCode,out parseResult) &&
                   !string.IsNullOrWhiteSpace(region) &&
                   phone.StartsWith('('))
                {
                    return false;
                }
                return true;
            }
            
            var result = from customer in customers
                         where  GetValidation(customer.PostalCode,customer.Region,customer.Phone) 
                         select customer;

            return result;
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            if (products is null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            var query = from product in products
                        group product by product.Category
                        into categoryProduct
                        select new Linq7CategoryGroup()
                        {
                            Category = categoryProduct.Key,
                            UnitsInStockGroup = from cat in categoryProduct
                                                orderby cat.UnitsInStock descending
                                                group cat.UnitPrice by cat.UnitsInStock
                                                into catProduct
                                                select new Linq7UnitsInStockGroup()
                                                {
                                                    UnitsInStock = catProduct.Key,
                                                    Prices = catProduct
                                                }
                        };
            return query;   
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
            var result = new List<(string, int, int)>();
            var query = from customer in customers
                        group customer.Orders by customer.City into countryOrders
                        select new
                        {
                            City = countryOrders.Key,
                            AvaregeIncome = (int)Math.Round((from customer in customers
                                                  where customer.City == countryOrders.Key
                                                  select customer.Orders.Sum(x => x.Total)).Average()),
                            AvaregeEntensity = (int)(from customer in customers
                                                     where customer.City == countryOrders.Key
                                                     select customer.Orders.Count()).Average()

                        };
            foreach (var item in query)
            {
                result.Add((item.City, item.AvaregeIncome, item.AvaregeEntensity));
            }
            return result;
            
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            if (suppliers is null)
            {
                throw new ArgumentNullException(nameof(suppliers));
            }

            string result = string.Concat((from supplier in suppliers
                                           orderby supplier.Country.Length, supplier.Country.Substring(0, 1)
                                           select supplier.Country).Distinct());
            return result;
        }
    }
}
