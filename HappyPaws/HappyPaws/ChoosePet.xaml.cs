using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using HappyPaws.Classes;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePet : ContentPage
    {
        private readonly Appointment SelectedAppt;
        private readonly Customer SelectedCustomer;

        public ChoosePet(Appointment appt, Customer customer)
        {
            InitializeComponent();
            SelectedCustomer = customer;
            SelectedAppt = appt;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            petsListView.ItemsSource = Pet.GetPets(SelectedCustomer.CustomerId);
        }


        private void Confirm_Clicked(object sender, EventArgs e)
        {
            SelectedAppt.Pets.Clear();
            foreach (Pet p in petsListView.SelectedItems) SelectedAppt.Pets.Add(p);
            //SelectedAppt.Pets = petsListView.SelectedItems.Cast<Pet>().ToList();
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void NewPet_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddPet(SelectedCustomer));
        }
    }
}