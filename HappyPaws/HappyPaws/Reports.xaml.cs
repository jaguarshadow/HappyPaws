using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HappyPaws.Classes;
using Xamarin.Essentials;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Reports : ContentPage
    {
        public Reports()
        {
            InitializeComponent();
        }

        private void Report_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((Picker)sender).SelectedIndex == 0) GenerateMonthlyAppointments();
            if (((Picker)sender).SelectedIndex == 1) GenerateAppointmentTypes();
        }

        private void GenerateMonthlyAppointments()    // Generate a list of appointments by month 
        {
            List<int> months = new List<int>();
            string reportText = "";
            foreach (Appointment a in Appointment.GetAppointments()) if (!months.Contains(a.StartTime.Month)) months.Add(a.StartTime.Month);
            foreach (int month in months)
            {
                reportText = reportText + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "\r\n";
                foreach (Appointment a in Appointment.GetAppointments().Where(a => a.StartTime.Month == month))
                {
                    reportText += string.Format("\t\t{0}\t\t{1} - {2}\t\t{3}\r\n", a.StartTime.Date.ToString("M/d/yyyy"), a.StartTime.TimeOfDay.ToString(@"hh\:mm"), a.EndTime.TimeOfDay.ToString(@"hh\:mm"), a.CustomerName);
                }
                reportText += "\r\n";
            }
            reportEditor.Text = reportText;
        }

        private void GenerateAppointmentTypes()    // Generate a list of appointment types by month 
        {
            List<int> months = new List<int>();
            string reportText = "";
            foreach (Appointment a in Appointment.GetAppointments()) if (!months.Contains(a.StartTime.Month)) months.Add(a.StartTime.Month);
            foreach (int month in months)
            {
                reportText = reportText + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "\r\n";
                int numVisits = Appointment.GetAppointments().Where(a => a.StartTime.Month == month && a.ApptType == "HomeVisit").ToList().Count;
                int numTrans = Appointment.GetAppointments().Where(a => a.StartTime.Month == month).Where(a => a.ApptType == "Transportation").ToList().Count;
                if (numVisits > 0) reportText += string.Format("\t\t{0}\t\t\tHome Visit\r\n", numVisits);
                if (numTrans > 0) reportText += string.Format("\t\t{0}\t\t\tTransportation\r\n", numTrans);
                reportText += "\r\n";
            }
            reportEditor.Text = reportText;
        }

        private async void ShareReport_Clicked(object sender, EventArgs e)
        {
            if (reportPicker.SelectedItem != null)
            {
                string subject;
                if (reportPicker.SelectedIndex == 0) subject = "Monthly Appointments Report";
                else subject = "Appointment Types Report";
                try
                {
                    var message = new EmailMessage
                    {
                        Subject = subject,
                        Body = reportEditor.Text,
                        To = { "management@happypaws.com" },
                    };
                    await Email.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException)
                {
                    await DisplayAlert("Cannot Send", "This device does not support sending Email.", "OK");
                }
                catch (Exception)
                {
                    await DisplayAlert("Cannot Send", "An error occured.", "OK");
                }
            }
            else await DisplayAlert("Cannot Share", "Select a report to share first!", "OK");
        }
    }
}