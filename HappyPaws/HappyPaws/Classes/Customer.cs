using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace HappyPaws.Classes
{
    [Table("Customer")]
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public static List<Customer> GetCustomers()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var customers = conn.Table<Customer>().ToList();
                return customers;
            }
        }

        public static List<Customer> GetCustomers(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var cList = from c in conn.Table<Customer>()
                            where c.CustomerId == id
                            select c;
                return cList.ToList();
            }
        }

        public static List<Customer> GetCustomers(string name)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var cList = conn.Query<Customer>($"SELECT * FROM Customer WHERE Name LIKE '%{name}%'");
                return cList;
            }
        }
    }
}
