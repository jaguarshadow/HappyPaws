using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HappyPaws.Classes;

namespace HappyPaws
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseCustomer : ContentPage
    {
        private readonly Appointment SelectedAppt;

        public ChooseCustomer()
        {
            InitializeComponent();
        }

        public ChooseCustomer(Appointment appt)
        {
            InitializeComponent();
            SelectedAppt = appt;
        }

        protected override void OnAppearing()
        {
            customerListView.ItemsSource = Customer.GetCustomers();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try 
            {
                int id = int.Parse(e.NewTextValue);
                customerListView.ItemsSource = Customer.GetCustomers(id);
            }
            catch
            {
                customerListView.ItemsSource = Customer.GetCustomers(e.NewTextValue);
            }      
        }

        private void NewCustomer_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddCustomer());
        }
        private void CustomerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedAppt != null)
            {
                Customer customer = (Customer)e.CurrentSelection[0];
                SelectedAppt.CustomerId = customer.CustomerId;
                Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                Customer customer = (Customer)e.CurrentSelection[0];
                Navigation.PushAsync(new ViewCustomer(customer));
            }
        }
    }
}