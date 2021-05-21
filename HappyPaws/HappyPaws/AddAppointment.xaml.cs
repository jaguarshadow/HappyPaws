using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using HappyPaws.Classes;
using static HappyPaws.Classes.Validation;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAppointment : ContentPage
    {
        private Appointment SelectedAppt;
        private Customer SelectedCustomer;
        private readonly bool NewAppt;

        public AddAppointment(Appointment appt)
        {
            InitializeComponent();
            SelectedAppt = appt;
            NewAppt = true;
            datePicker.MinimumDate = DateTime.Today;
        }

        public AddAppointment(Appointment appt, Customer customer)
        {
            InitializeComponent();
            SelectedCustomer = customer;
            SelectedAppt = appt;
            NewAppt = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (SelectedAppt.PetList != null) SelectedAppt.Pets = Pet.GetPetsForAppt(SelectedCustomer.CustomerId, SelectedAppt.PetList);
            SelectedCustomer = Customer.GetCustomers(SelectedAppt.CustomerId).FirstOrDefault();
            if (!NewAppt)
            {
                datePicker.Date = SelectedAppt.StartTime.Date;
                startTimePicker.Time = SelectedAppt.StartTime.TimeOfDay;
                endTimePicker.Time = SelectedAppt.EndTime.TimeOfDay;
                custNameLabel.Text = SelectedCustomer.Name;
                custAddressLabel.Text = SelectedCustomer.Phone;
            }
            else
            {
                if (SelectedCustomer != null)
                {
                    custNameLabel.Text = SelectedCustomer.Name;
                    custAddressLabel.Text = SelectedCustomer.Phone;
                }
                if (SelectedAppt.StartTime != null) 
                {
                    startTimePicker.Time = SelectedAppt.StartTime.TimeOfDay;
                    if (SelectedAppt.StartTime != null) startTimePicker.Time = SelectedAppt.StartTime.TimeOfDay;
                }
                else
                {
                    DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 9, 0, 0);
                    startTimePicker.Time = start.TimeOfDay;
                    endTimePicker.Time = start.TimeOfDay.Add(TimeSpan.Parse("00:15:00"));
                }
            }
            if (SelectedAppt.GetType() == typeof(HomeVisit))
            {
                typeLabel.Text = "Home Visit";
                transportLayout.IsVisible = false;
                homeVisitLayout.IsVisible = true;
                if (!NewAppt)
                {
                    foodCheckBox.IsChecked = (((HomeVisit)SelectedAppt).Food);
                    playCheckBox.IsChecked = (((HomeVisit)SelectedAppt).Play);
                    walkCheckBox.IsChecked = (((HomeVisit)SelectedAppt).Walk);
                    scoopCheckBox.IsChecked = (((HomeVisit)SelectedAppt).Scoop);
                }
            }
            else
            {
                typeLabel.Text = "Transportation";
                transportLayout.IsVisible = true;
                homeVisitLayout.IsVisible = false;
                if (!NewAppt)
                {
                    startAddressBox.Text = ((Transportation)SelectedAppt).StartingAddress;
                    destAddressBox.Text = ((Transportation)SelectedAppt).DestinationAddress;
                    reasonBox.Text = ((Transportation)SelectedAppt).ReasonForTrip;
                    roundTripCheckBox.IsChecked = (((Transportation)SelectedAppt).RoundTrip);
                }
            }
            if (SelectedAppt.Pets.Count != 0)
            {
                noPetsLabel.IsVisible = false;
            }
            petsListView.ItemsSource = null;
            petsListView.ItemsSource = SelectedAppt.Pets;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (SelectedAppt.Pets.Count == 0) 
            {
                await DisplayAlert("Alert", "Add at least one pet before saving appt.", "OK");
            }
            else
            {
                Appointment appt;
                string start = datePicker.Date.ToString("MM/dd/yyyy") + " " + startTimePicker.Time.ToString();
                SelectedAppt.StartTime = DateTime.Parse(start);
                string end = datePicker.Date.ToString("MM/dd/yyyy") + " " + endTimePicker.Time.ToString();
                SelectedAppt.EndTime = DateTime.Parse(end);

                List<Appointment> existing = Appointment.GetAppointments().Where(a => a.StartTime >= SelectedAppt.StartTime && a.StartTime <= SelectedAppt.EndTime).ToList();
                existing.AddRange(Appointment.GetAppointments().Where(a => a.EndTime >= SelectedAppt.StartTime && a.EndTime <= SelectedAppt.EndTime).ToList());
                if (existing.Count != 0)
                {
                    await DisplayAlert("Alert", "Cannot add appointment. Another appointment already exists during this timeframe.", "OK");
                }
                else
                {
                    if (SelectedAppt.GetType() == typeof(HomeVisit))
                    {
                        appt = (HomeVisit)SelectedAppt;
                        ((HomeVisit)appt).Food = foodCheckBox.IsChecked;
                        ((HomeVisit)appt).Play = playCheckBox.IsChecked;
                        ((HomeVisit)appt).Walk = walkCheckBox.IsChecked;
                        ((HomeVisit)appt).Scoop = scoopCheckBox.IsChecked;
                    }
                    else
                    {
                        appt = (Transportation)SelectedAppt;
                        ((Transportation)appt).StartingAddress = startAddressBox.Text;
                        destAddressBox.Text = ((Transportation)appt).DestinationAddress;
                        reasonBox.Text = ((Transportation)appt).ReasonForTrip;
                        ((Transportation)appt).RoundTrip = roundTripCheckBox.IsChecked;
                    }
                    appt.CustomerId = SelectedCustomer.CustomerId;
                    List<string> pets = new List<string>();
                    foreach (Pet p in SelectedAppt.Pets) pets.Add(string.Format("{0}|{1}", p.Species, p.PetId));
                    appt.PetList = string.Join(",", pets);
                    if (NewAppt)
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Insert(appt);
                    }
                    else
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Update(appt);
                    }
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                
            }
        }

        private void Validation_Handler(object sender, TextChangedEventArgs e)
        {
            if (SelectedAppt.ApptType == "Transportation")
            {
                if (!ValidateName(startAddressBox.Text) || !ValidateName(destAddressBox.Text) || !ValidateName(reasonBox.Text)) saveButton.IsEnabled = false;
                return;
            }
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((Entry)sender).Placeholder == "Starting Address")
            {
                if (ValidateName(e.NewTextValue)) startValidationLabel.IsVisible = false;
                else startValidationLabel.IsVisible = true;
            }
            else if (((Entry)sender).Placeholder == "Destination Address")
            {
                if (ValidateName(e.NewTextValue)) destValidationLabel.IsVisible = false;
                else destValidationLabel.IsVisible = true;
            }
            else // Validate Reason 
            {
                if (ValidateAge(e.NewTextValue)) reasonValidationLabel.IsVisible = false;
                else reasonValidationLabel.IsVisible = true;
            }
        }

        private void DateSelected(object sender, FocusEventArgs e)
        {
            string start = datePicker.Date.ToString("MM/dd/yyyy") + " " + startTimePicker.Time.ToString();
            SelectedAppt.StartTime = DateTime.Parse(start);
            string end = datePicker.Date.ToString("MM/dd/yyyy") + " " + endTimePicker.Time.ToString();
            SelectedAppt.EndTime = DateTime.Parse(end);
            saveButton.IsEnabled = false;
            if (SelectedAppt.StartTime.Hour < 8 || SelectedAppt.StartTime.Hour > 21 || SelectedAppt.EndTime.Hour < 8 || SelectedAppt.EndTime.Hour > 21) sTimeValidationLabel.IsVisible = true;
            else if (SelectedAppt.EndTime < SelectedAppt.StartTime) timeValidationLabel.IsVisible = true;
            else
            {
                sTimeValidationLabel.IsVisible = false;
                timeValidationLabel.IsVisible = false;
                saveButton.IsEnabled = true;
            }
        }

        private void ChangeCustomer_Clicked(object sender, EventArgs e)
        {
            SelectedAppt.Pets.Clear();
            Navigation.PushAsync(new ChooseCustomer(SelectedAppt));
        }

        private async void ChangePets_Clicked(object sender, EventArgs e)
        {
            if (SelectedCustomer != null)
            {
                SelectedAppt.Pets.Clear();
                await Navigation.PushAsync(new ChoosePet(SelectedAppt, SelectedCustomer));
            }
            else await DisplayAlert("Alert", "Choose a customer before adding pets.", "OK");
        }
    }
}