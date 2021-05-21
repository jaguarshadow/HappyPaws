using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using HappyPaws.Classes;

namespace HappyPaws
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            apptListView.ItemsSource = Appointment.GetAppointments().Where(a => a.StartTime > DateTime.Today).OrderBy(a => a.StartTime.Date);
        }

        private async void AddAppointment_Clicked(object sender, EventArgs e)
        {
            Appointment appt;
            string action = await DisplayActionSheet("Add Appointment", "Cancel", null, "Home Visit", "Transportation");
            if (action == "Home Visit") appt = new HomeVisit();
            else if (action == "Transportation") appt = new Transportation();
            else appt = null;
            if (appt != null) await Navigation.PushAsync(new AddAppointment(appt));
        }

        private void ApptSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            Appointment a = ((Appointment)e.CurrentSelection[0]);
            Navigation.PushAsync(new ViewAppointment(a));
        }

        private void Customers_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChooseCustomer());
        }

        private void Reports_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Reports());
        }

        private void ApptPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)apptPicker.SelectedItem == "All Appts")
            {
                apptListView.ItemsSource = null;
                apptListView.ItemsSource = Appointment.GetAppointments().OrderBy(a => a.StartTime.Date);
                filterLabel.Text = "All Appointments";
            }
            else
            {
                apptListView.ItemsSource = null;
                apptListView.ItemsSource = Appointment.GetAppointments().Where(a => a.StartTime > DateTime.Today).OrderBy(a => a.StartTime.Date);
                filterLabel.Text = "Upcoming Appointments";
            }
        }
    }
}
