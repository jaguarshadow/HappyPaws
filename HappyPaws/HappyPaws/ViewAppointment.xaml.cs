using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HappyPaws.Classes;
using SQLite;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewAppointment : ContentPage
    {
        private Appointment SelectedAppt;
        private readonly Customer SelectedCustomer;
        public ViewAppointment(Appointment appt)
        {
            InitializeComponent();
            SelectedAppt = appt;
            SelectedCustomer = Customer.GetCustomers(appt.CustomerId).FirstOrDefault();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            dateLabel.Text = SelectedAppt.StartTime.Date.ToString("MM/dd/yyyy");
            timeLabel.Text = string.Format("{0} - {1}",SelectedAppt.StartTime.ToString("HH:mm tt"), SelectedAppt.EndTime.ToString("HH:mm tt"));
            ownerLabel.Text = SelectedCustomer.Name;
            oAddressLabel.Text = SelectedCustomer.Address;
            if (SelectedAppt.GetType() == typeof(HomeVisit))
            {
                HomeVisit h = (HomeVisit)SelectedAppt;
                visitLayout.IsVisible = true;
                typeLabel.Text = "Home Visit";
                if (h.Food) foodCheckBox.IsChecked = true;
                if (h.Play) playCheckBox.IsChecked = true;
                if (h.Walk) walkCheckBox.IsChecked = true;
                if (h.Scoop) scoopCheckBox.IsChecked = true;
            }
            else
            {
                Transportation t = (Transportation)SelectedAppt;
                transportLayout.IsVisible = true;
                typeLabel.Text = "Transportation";
                startAddressLabel.Text = t.StartingAddress;
                destAddressLabel.Text = t.DestinationAddress;
                reasonLabel.Text = t.ReasonForTrip;
                if (t.RoundTrip) roundTripCheckBox.IsChecked = true;
            }
            petsListView.ItemsSource = null;
            petsListView.ItemsSource = Pet.GetPetsForAppt(SelectedCustomer.CustomerId, SelectedAppt.PetList);

        }

        private void EditAppt_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddAppointment(SelectedAppt, SelectedCustomer));
        }

        private async void DeleteAppt_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Appointment?", string.Format("Are you sure you want to delete the appointment for {0} on {1}?", SelectedCustomer.Name, (SelectedAppt.StartTime.ToString("MM/dd/yyyy HH:mm t"))), "Delete", "Cancel");
            if (answer)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Delete(SelectedAppt);
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private void AddPet_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChoosePet(SelectedAppt, SelectedCustomer));
        }

        private void PetSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            Navigation.PushAsync(new ViewPet((Pet)e.CurrentSelection[0]));
        }
    }
}