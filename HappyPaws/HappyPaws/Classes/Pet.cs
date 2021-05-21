using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace HappyPaws.Classes
{
    public abstract class Pet
    {
        [PrimaryKey, AutoIncrement]
        public int PetId { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public bool SpecialNeeds { get; set; }
        public string Notes { get; set; }

        public virtual string Species { get; }

        public static List<Pet> GetPets(int customerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                List<Pet> pets = new List<Pet>();
                pets.AddRange(Dog.GetDogs(customerId));
                pets.AddRange(Cat.GetCats(customerId));
                return pets;
            }
        }
        public static List<Pet> GetPetsForAppt(int customerId, string petList)
        {
            List<Pet> pets = new List<Pet>();
            foreach (string s in petList.Split(','))
            {
                string species = s.Substring(0, 3);
                string id = s.Substring(4);
                Pet p;
                if (species == "Dog")
                {
                    p = Dog.GetDogs(customerId).Where(pet => pet.PetId == int.Parse(id)).FirstOrDefault();
                }
                else
                {
                    p = Cat.GetCats(customerId).Where(pet => pet.PetId == int.Parse(id)).FirstOrDefault();
                }
                pets.Add(p);
            }
            return pets;
        }
    }

    [Table("Dog")]
    public class Dog : Pet
    {
        public override string Species { get { return "Dog"; } }
        public bool TreatsAllowed { get; set; }
        public bool SocializingAllowed { get; set; }
        public static List<Pet> GetDogs(int customerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var cList = conn.Query<Dog>($"SELECT * FROM Dog WHERE OwnerId={customerId}");
                return cList.Cast<Pet>().ToList();
            }
        }
    }

    [Table("Cat")]
    public class Cat : Pet
    {
        public override string Species { get { return "Cat"; } }
        public bool CatnipAllowed { get; set; }
        public bool Declawed { get; set; }
        public static List<Pet> GetCats(int customerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var cList = conn.Query<Cat>($"SELECT * FROM Cat WHERE OwnerId={customerId}");
                return cList.Cast<Pet>().ToList();
            }
        }
    }
}
