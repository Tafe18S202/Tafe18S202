using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        string Gender;

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }

        public void Results()
        {
            conn.CreateTable<WishList>();
            var query1 = conn.Table<WishList>();
            WishListView.ItemsSource = query1.ToList();
        }


        private async void AddPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_FirstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                if (_LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Last Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    
                    string Name = _FirstName.Text;
                    string LastName = _LastName.Text;
                    DateTime DOB = Convert.ToDateTime(_DOB.Text);
                    MessageDialog dialog = new MessageDialog("The gender you selecected is: " + Gender);




                    //double TempMoney = Convert.ToDouble(MoneyIn.Text);
                    conn.CreateTable<PersonalInfo>();
                    conn.Insert(new PersonalInfo { firstName = Name, lastName = LastName, DOB = DOB });
                    // Creating table
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Invalid data entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Wish Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private void DeletePersonalInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Male_Checked(object sender, RoutedEventArgs e)
        {
            Gender = "Male";
            MessageDialog dialog = new MessageDialog("The gender you selecected is: " + Gender);
        }

        private void _Female_Checked(object sender, RoutedEventArgs e)
        {
            Gender = "Female";
            MessageDialog dialog = new MessageDialog("The gender you selecected is: " + Gender);
        }
    }
}
