/**
* @author Pablo Paramo
*
* @date - 27 Aug 2018
*/

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
        string DOB;
        private void DateOfBirth(string selectedDate)
        {
            DOB = selectedDate;
        }

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
            conn.CreateTable<PersonalInfo>();
            var query1 = conn.Table<PersonalInfo>();
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
                    //CalendarDatePicker DOB = new CalendarDatePicker();
                    
                    //DOB = _DOB1;
                    //_DOB1.GetValue.ToString("dd-MM-yy");
                    //DOB.ToString("dd-MM-yyyy");
                    //Gender added on top

                    string email = _Email.Text;
                    string address = _Address.Text;
                    int phoneNumber = Convert.ToInt16(_PhoneNumber.Text);
                    
                    




                    //double TempMoney = Convert.ToDouble(MoneyIn.Text);
                    conn.CreateTable<PersonalInfo>();
                    conn.Insert(new PersonalInfo { firstNamo = Name, lastName = LastName, DOB = DOB, gender = Gender, email = email,  address = address, mobileNumber = phoneNumber});
                    // Creating table
                    Results();
                   
                    _FirstName.IsReadOnly = true;
                    _FirstName.IsHitTestVisible = false;

                    _LastName.IsReadOnly = true;
                    _LastName.IsHitTestVisible = false;

                    _DOB1.IsEnabled = false;
                    _DOB1.IsGroupLabelVisible = false;
                    _DOB1.IsHitTestVisible = false;
                    _DOB1.Background.Opacity = 0;

                    _Male.IsHitTestVisible = false;
                    _Male.IsEnabled = false;

                    _Female.IsHitTestVisible = false;
                    _Female.IsEnabled = false;

                    

                    

                    _Email.IsReadOnly = true;
                    _Email.IsHitTestVisible = false;

                    _Address.IsReadOnly = true;
                    _Address.IsHitTestVisible = false;


                    _PhoneNumber.IsHitTestVisible = false;
                    _PhoneNumber.IsReadOnly = true;




                    _FirstName.BorderBrush.Opacity = 0;
                    //SQLiteCommand q = new SQLiteCommand(@"Select FirstNamo from PersonalInfo;");
                    var que = conn.Query<PersonalInfo>("Select FirstNamo from PersonalInfo");
                    MessageDialog dialog = new MessageDialog(que.ToString() + " Thanks Your Personal data has been saved!");
                    await dialog.ShowAsync();




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
                    MessageDialog dialog = new MessageDialog("Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void DeletePersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((WishList)WishListView.SelectedItem).WishName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<WishList>();
                    var query1 = conn.Table<WishList>();
                    var query3 = conn.Query<WishList>("DELETE FROM WishList WHERE WishName ='" + AccSelection + "'");
                    WishListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
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

        private void ModifyPersonalInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null) {
               
                var date = _DOB1.Date;
                DateTime time = date.Value.DateTime;
                var formatedtime = time.ToString("dd/MM/yyyy");
                System.Diagnostics.Debug.WriteLine(formatedtime);
                DateOfBirth(formatedtime);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
