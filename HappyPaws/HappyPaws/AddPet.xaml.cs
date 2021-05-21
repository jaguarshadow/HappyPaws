using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HappyPaws.Classes;
using static HappyPaws.Classes.Validation;
using SQLite;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPet : ContentPage
    {
        private readonly Customer SelectedCustomer;
        private readonly Pet SelectedPet;
        private readonly bool NewPet;
        public AddPet(Customer customer)
        {
            // To add a new pet 
            InitializeComponent();
            titleLabel.Text = "Add Pet";
            ownerLabel.Text += customer.Name;
            SelectedCustomer = customer;
            NewPet = true;
        }

        public AddPet(Customer customer, Pet pet)
        {
            // To modify and existing pet
            InitializeComponent();
            titleLabel.Text = "Edit Pet";
            SelectedCustomer = customer;
            SelectedPet = pet;
            ownerLabel.Text += customer.Name;
            NewPet = false;
            nameEntry.Text = pet.Name;
            breedEntry.Text = pet.Breed;
            ageEntry.Text = pet.Age.ToString();
            notesEditor.Text = pet.Notes;
            if (pet.SpecialNeeds) specialCheckBox.IsChecked = true;
            if (pet.Species == "Dog")
            {
                speciesPicker.SelectedItem = "Dog";
                if (((Dog)pet).TreatsAllowed) treatCheckBox.IsChecked = true;
                if (((Dog)pet).SocializingAllowed) socialCheckBox.IsChecked = true;
            }
            else
            {
                speciesPicker.SelectedItem = "Cat";
                if (((Cat)pet).CatnipAllowed) treatCheckBox.IsChecked = true;
                if (((Cat)pet).Declawed) socialCheckBox.IsChecked = true;
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Pet pet;
            if ((string)speciesPicker.SelectedItem == "Dog")
            {
                if (NewPet) pet = new Dog();
                else pet = SelectedPet;
                ((Dog)pet).TreatsAllowed = treatCheckBox.IsChecked;
                ((Dog)pet).SocializingAllowed = socialCheckBox.IsChecked;
            }
            else
            {
                if (NewPet) pet = new Cat();
                else pet = SelectedPet;
                ((Cat)pet).CatnipAllowed = treatCheckBox.IsChecked;
                ((Cat)pet).Declawed = socialCheckBox.IsChecked;
            }
            pet.Name = nameEntry.Text;
            pet.OwnerId = SelectedCustomer.CustomerId;
            pet.Breed = breedEntry.Text;
            pet.Age = int.Parse(ageEntry.Text);
            pet.Notes = notesEditor.Text;
            pet.SpecialNeeds = specialCheckBox.IsChecked;

            if (NewPet)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Insert(pet);
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Update(pet);
            }
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void Validation_Handler(object sender, TextChangedEventArgs e)
        {
            if (ValidateName(nameEntry.Text) && ValidateName(breedEntry.Text) && ValidateAge(ageEntry.Text))
            {
                saveButton.IsEnabled = true;
            }
            else saveButton.IsEnabled = false;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((Entry)sender).Placeholder == "Pet Name")
            {
                if (ValidateName(e.NewTextValue)) nameValidationLabel.IsVisible = false;
                else nameValidationLabel.IsVisible = true;
            }
            else if (((Entry)sender).Placeholder == "Breed")
            {
                if (ValidateName(e.NewTextValue)) breedValidationLabel.IsVisible = false;
                else breedValidationLabel.IsVisible = true;
            }
            else // Validate Age 
            {
                if (ValidateAge(e.NewTextValue)) ageValidationLabel.IsVisible = false;
                else ageValidationLabel.IsVisible = true;
            }
        }

        private void PetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)speciesPicker.SelectedItem == "Dog")
            {
                treatsLabel.Text = "Treats Allowed";
                socialLabel.Text = "Socializing";
            }
            else
            {
                treatsLabel.Text = "Catnip Allowed";
                socialLabel.Text = "Declawed";
            }
        }
    }
}