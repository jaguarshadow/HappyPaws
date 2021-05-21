using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HappyPaws.Classes
{
    public abstract class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int ApptId { get; set; }
        public virtual string ApptType { get; }
        public int CustomerId { get; set; }
        public string CustomerName { get { return Customer.GetCustomers(CustomerId).FirstOrDefault().Name; } }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PetList { get; set; }

        public string NumPets 
        { 
            get 
            {   int count = PetList.Split(',').ToList().Count;
                return count > 1 ? count + " pets" : count + " pet"; } 
        }

        public List<Pet> Pets = new List<Pet>();

        public static List<Appointment> GetAppointments()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                List<Appointment> appts = new List<Appointment>();
                appts.AddRange(HomeVisit.GetVisits());
                appts.AddRange(Transportation.GetTrans());
                return appts;
            }
        }

        public static List<Appointment> GetAppointments(int customerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                List<Appointment> appts = new List<Appointment>();
                appts.AddRange(HomeVisit.GetVisits().Where(a => a.CustomerId == customerId));
                appts.AddRange(Transportation.GetTrans().Where(a => a.CustomerId == customerId));
                return appts;
            }
        }
    }

    [Table("HomeVisit")]
    public class HomeVisit : Appointment
    {
        public override string ApptType { get { return "HomeVisit"; } }
        public bool Food { get; set; }
        public bool Play { get; set; }
        public bool Scoop { get; set; }
        public bool Walk { get; set; }

        public static List<Appointment> GetVisits()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var vList = conn.Table<HomeVisit>();
                return vList.Cast<Appointment>().ToList();
            }
        }
    }

    [Table("Transportation")]
    public class Transportation : Appointment
    {
        public override string ApptType { get { return "Transportation"; } }
        public bool RoundTrip { get; set; }
        public string StartingAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string ReasonForTrip { get; set; }
        public static List<Appointment> GetTrans()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var tList = conn.Table<Transportation>();
                return tList.Cast<Appointment>().ToList();
            }
        }
    }
}
