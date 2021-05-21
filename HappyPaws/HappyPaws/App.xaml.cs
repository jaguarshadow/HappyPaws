using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using HappyPaws.Classes;

namespace HappyPaws
{
    public partial class App : Application
    {
        public static string FilePath;
        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.ProtectionComplete;

        public App(string filePath)
        {
            InitializeComponent();

            FilePath = filePath;
            using (SQLiteConnection conn = new SQLiteConnection(FilePath))
            {
                // Ensures all database tables exist 
                conn.CreateTable<User>();
                conn.CreateTable<Customer>();
                conn.CreateTable<Transportation>();
                conn.CreateTable<HomeVisit>();
                conn.CreateTable<Dog>();
                conn.CreateTable<Cat>();

               // Determines if test data needs to be populated 
                var cus = conn.Table<Customer>().ToList();
                if (cus.Count == 0) PopulateTestData();

                // Determines if user data needs to be populated 
                var user = conn.Table<User>().ToList();
                if (user.Count == 0) PopulateUser();
            }

            MainPage = new NavigationPage(new Login());

            ((NavigationPage)MainPage).BarBackgroundColor = System.Drawing.Color.MidnightBlue;
            ((NavigationPage)MainPage).BarTextColor = System.Drawing.Color.White;
        }

        private void PopulateUser()
        {
            using (SQLiteConnection conn = new SQLiteConnection(FilePath))
            {
                User user = new User()
                {
                    Name = "user",
                    Password = "P@55word"
                };
                conn.Insert(user);
            }
        }

        private void PopulateTestData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(FilePath))
            {
                // Populate Test Data
                Customer customer = new Customer()
                {
                    Name = "John Anderson",
                    Address = "123 Main\nSan Antonio, TX 78248",
                    Phone = "555-555-1111",
                    Email = "janderson555@email.com"
                };
                Customer customer2 = new Customer()
                {
                    Name = "Jane Michaels",
                    Address = "123 Oak St\nSan Antonio, TX 78213",
                    Phone = "555-555-2222",
                    Email = "jmichaels111@email.com"
                };
                conn.Insert(customer);
                conn.Insert(customer2);

                Dog dog = new Dog()
                {
                    Name = "Sparky",
                    OwnerId = customer.CustomerId,
                    Breed = "Golden Retriever",
                    Age = 2,
                    SpecialNeeds = false,
                    TreatsAllowed = true,
                    SocializingAllowed = true,
                    Notes = "Wears a red collar."
                };
                Dog dog2 = new Dog()
                {
                    Name = "Benji",
                    OwnerId = customer2.CustomerId,
                    Breed = "Greyhound",
                    Age = 8,
                    SpecialNeeds = true,
                    TreatsAllowed = true,
                    SocializingAllowed = false,
                    Notes = "Benji gets 100mg of Cosequin in his food with each meal."
                };
                Cat cat = new Cat()
                {
                    Name = "Fifi",
                    OwnerId = customer.CustomerId,
                    Breed = "Domestic Shorthair",
                    Age = 3,
                    SpecialNeeds = false,
                    CatnipAllowed = true,
                    Declawed = false,
                    Notes = "Wears a pink collar."
                };
                conn.Insert(dog);
                conn.Insert(dog2);
                conn.Insert(cat);

                HomeVisit visit1 = new HomeVisit()
                {
                    CustomerId = customer.CustomerId,
                    StartTime = new DateTime(2021, 5, 10, 13, 30, 00),
                    EndTime = new DateTime(2021, 5, 10, 14, 30, 00),
                    PetList = "Dog|1,Cat|1",
                    Food = true,
                    Play = true,
                    Scoop = false,
                    Walk = true
                };
                HomeVisit visit2 = new HomeVisit()
                {
                    CustomerId = customer.CustomerId,
                    StartTime = new DateTime(2021, 5, 20, 13, 30, 00),
                    EndTime = new DateTime(2021, 5, 20, 14, 30, 00),
                    PetList = "Dog|1,Cat|1",
                    Food = true,
                    Play = true,
                    Scoop = false,
                    Walk = true
                };
                HomeVisit visit3 = new HomeVisit()
                {
                    CustomerId = customer2.CustomerId,
                    StartTime = new DateTime(2021, 6, 15, 10, 00, 00),
                    EndTime = new DateTime(2021, 6, 15, 10, 30, 00),
                    PetList = "Dog|2",
                    Food = true,
                    Play = false,
                    Scoop = true,
                    Walk = false
                };
                Transportation trans1 = new Transportation()
                {
                    CustomerId = customer.CustomerId,
                    StartTime = new DateTime(2021, 5, 25, 11, 15, 00),
                    EndTime = new DateTime(2021, 5, 25, 11, 45, 00),
                    PetList = "Dog|1",
                    RoundTrip = false,
                    StartingAddress = "123 Main",
                    DestinationAddress = "5102 Park Pl",
                    ReasonForTrip = "Dropping off at groomer"
                };
                Transportation trans2 = new Transportation()
                {
                    CustomerId = customer2.CustomerId,
                    StartTime = new DateTime(2021, 6, 18, 10, 00, 00),
                    EndTime = new DateTime(2021, 6, 18, 11, 30, 00),
                    PetList = "Dog|2",
                    RoundTrip = true,
                    StartingAddress = "123 Oak St",
                    DestinationAddress = "555 Fairview St",
                    ReasonForTrip = "Taking to the vet for vaccinations"
                };
                conn.Insert(visit1);
                conn.Insert(visit2);
                conn.Insert(visit3);
                conn.Insert(trans1);
                conn.Insert(trans2);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
