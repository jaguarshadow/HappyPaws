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
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            string username = userEntry.Text;
            string password = passEntry.Text;
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                User user = conn.Query<User>($"select * from User where Name='{username}'").FirstOrDefault();
                if (user.Password == password)
                {
                    Application.Current.MainPage.Navigation.InsertPageBefore(new MainPage(), this);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else await DisplayAlert("Login Failed", "Username or password invalid.", "OK");
            }
        }
    }
}