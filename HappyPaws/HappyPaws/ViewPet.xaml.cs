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
    public partial class ViewPet : ContentPage
    {
        private readonly Pet SelectedPet;
        private readonly Customer SelectedOwner;
        public ViewPet(Pet p)
        {
            InitializeComponent();
            SelectedPet = p;
            SelectedOwner = Customer.GetCustomers(p.OwnerId)[0];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            petNameLabel.Text = SelectedPet.Name;
            speciesLabel.Text = SelectedPet.Species;
            ownerLabel.Text = SelectedOwner.Name;
            breedLabel.Text = SelectedPet.Breed;
            ageLabel.Text = SelectedPet.Age.ToString() + " years";
            notesLabel.Text = SelectedPet.Notes;
            if (SelectedPet.SpecialNeeds)
            {
                specialLabel.Text = "Yes";
                specialLabel.TextColor = System.Drawing.Color.Red;
            }
            if (SelectedPet.GetType() == typeof(Dog))
            {
                if (((Dog)SelectedPet).TreatsAllowed) treatsLabel.Text = "Yes";
                if (((Dog)SelectedPet).SocializingAllowed) socialLabel.Text = "Yes";
            }
            else
            {
                if (((Cat)SelectedPet).CatnipAllowed) treatsLabel.Text = "Yes";
                tAllowedLabel.Text = "Catnip:";
                if (((Cat)SelectedPet).Declawed) socialLabel.Text = "Yes";
                sAllowedLabel.Text = "Declawed:";
            }
        }

        private void EditPet_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddPet(SelectedOwner, SelectedPet));
        }

        private async void DeletePet_Clicked(object sender, EventArgs e)
        {
            bool hasAppt = false;
            foreach (Appointment a in Appointment.GetAppointments(SelectedOwner.CustomerId))
            {
                foreach (string s in a.PetList.Split(','))
                {
                    string species = s.Substring(0, 3);
                    string id = s.Substring(4);
                    if (SelectedPet.Species == species && SelectedPet.PetId == int.Parse(id))
                    {
                        hasAppt = true;
                        break;
                    }
                }
            }
            if (hasAppt) await DisplayAlert("Delete Failed", "This pet has appointments associated with it, cannot delete.", "OK");
            else
            {
                bool answer = await DisplayAlert("Delete Pet?", string.Format("Are you sure you want to delete {0}?", SelectedPet.Name), "Delete", "Cancel");
                if (answer)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.FilePath)) conn.Delete(SelectedPet);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
        }
    }
}