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
    public partial class AddCustomer : ContentPage
    {
        private readonly bool NewCustomer;
        private readonly Customer SelectedCustomer;
        public AddCustomer()
        {
            // Adding a new customer
            InitializeComponent();
            titleLabel.Text = "Add Customer";
            NewCustomer = true;
        }

        public AddCustomer(Customer customer)
        {
            // Editing an existing customer 
            InitializeComponent();
            titleLabel.Text = "Edit Customer";
            NewCustomer = false;
            SelectedCustomer = customer;
            nameEntry.Text = customer.Name;
            addressEntry.Text = customer.Address;
            emailEntry.Text = customer.Email;
            phoneEntry.Text = customer.Phone;
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // Save customer
            Customer customer;
            if (NewCustomer) customer = new Customer();
            else customer = SelectedCustomer;
            customer.Name = nameEntry.Text;
            customer.Address = addressEntry.Text;
            customer.Phone = phoneEntry.Text;
            customer.Email = emailEntry.Text;

            if (NewCustomer)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Insert(customer);
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Update(customer);
            }
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void Validation_Handler(object sender, TextChangedEventArgs e)
        {
            if (ValidateName(nameEntry.Text) && ValidateName(addressEntry.Text) && ValidateEmail(emailEntry.Text) && ValidatePhone(phoneEntry.Text))
            {
                saveButton.IsEnabled = true;
            }
            else saveButton.IsEnabled = false;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((Entry)sender).Placeholder == "Customer Name")
                {
                    if (ValidateName(e.NewTextValue)) nameValidationLabel.IsVisible = false;
                    else nameValidationLabel.IsVisible = true;
                }
                else if (((Entry)sender).Placeholder == "name@domain.com")
                {
                    if (ValidateEmail(e.NewTextValue)) emailValidationLabel.IsVisible = false;
                    else emailValidationLabel.IsVisible = true;
                }
                else // Validate Phone 
                {
                    if (ValidatePhone(e.NewTextValue)) phoneValidationLabel.IsVisible = false;
                    else phoneValidationLabel.IsVisible = true;
                }
            }
            catch
            {
                if (((Editor)sender).Placeholder == "Customer Address")
                {
                    if (ValidateName(e.NewTextValue))
                    {
                        if (ValidateName(e.NewTextValue)) addressValidationLabel.IsVisible = false;
                        else addressValidationLabel.IsVisible = true;
                    }
                }
            }
            
        }
    }
}