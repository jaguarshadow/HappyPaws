using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HappyPaws.Classes;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewCustomer : ContentPage
    {
        public readonly Customer SelectedCustomer;
        public ViewCustomer(Customer customer)
        {
            InitializeComponent();
            customerNameLabel.Text = customer.Name;
            SelectedCustomer = customer;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            addressLabel.Text = SelectedCustomer.Address;
            phoneLabel.Text = SelectedCustomer.Phone;
            emailLabel.Text = SelectedCustomer.Email;
            petsListView.ItemsSource = Pet.GetPets(SelectedCustomer.CustomerId).OrderBy(p => p.Name);
        }

        private void EditCustomer_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddCustomer(SelectedCustomer));
        }

        private async void DeleteCustomer_Clicked(object sender, EventArgs e)
        {
            if (Appointment.GetAppointments(SelectedCustomer.CustomerId).Count != 0)
            {
                await DisplayAlert("Delete Failed", "This customer appointments scheduled, cannot delete.", "OK");
            }
            else
            {
                bool answer = await DisplayAlert("Delete Customer?", string.Format("Are you sure you want to delete {0}? This will also delete all associated pets.", SelectedCustomer.Name), "Delete", "Cancel");
                if (answer)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                    {
                        foreach (Pet p in Pet.GetPets(SelectedCustomer.CustomerId)) conn.Delete(p);
                        conn.Delete(SelectedCustomer);
                    }
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
        }

        private void PetSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            Pet p = (Pet)e.CurrentSelection[0];
            Navigation.PushAsync(new ViewPet(p));
        }

        private void NewPet_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddPet(SelectedCustomer));
        }
    }
}