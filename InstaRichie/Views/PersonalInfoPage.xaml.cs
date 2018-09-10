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
            PersonalInfoListView.ItemsSource = query1.ToList();
            PersonalInfoListView.SelectedItem = 0;


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
                    string email = _Email.Text;
                    string address = _Address.Text;
                    int phoneNumber = Convert.ToInt32(_PhoneNumber.Text);


                    //Check if there is already a record - if true, delete so it can be recorded again as an updated record.
                    if (PersonalInfoListView.Items.Count != 0)
                    {
                        try
                        {
                            string AccSelection = ((PersonalInfo)PersonalInfoListView.SelectedItem).firstName;
                            if (AccSelection == null)
                            {
                                //MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                                //await dialog.ShowAsync();
                            }
                            else
                            {
                                conn.CreateTable<PersonalInfo>();
                                var query1 = conn.Table<PersonalInfo>();
                                var query3 = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName ='" + AccSelection + "'");
                                PersonalInfoListView.ItemsSource = query1.ToList();
                            }
                        }
                        catch (NullReferenceException)
                        {
                            //MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                            // await dialog.ShowAsync();
                        }
                    }


                    conn.CreateTable<PersonalInfo>();
                    conn.Insert(new PersonalInfo { firstName = Name, lastName = LastName, DOB = DOB, gender = Gender, email = email, address = address, mobileNumber = phoneNumber });
                    // Creating table
                    Results();


                    MessageDialog dialog = new MessageDialog("Thanks, " + _FirstName.Text + ". Your Personal data has been saved!");
                    await dialog.ShowAsync();

                    //Reset form
                    _FirstName.Text = "";

                    _LastName.Text = "";

                    _DOB1.Date = null;

                    _Male.IsChecked = false;

                    _Female.IsChecked = false;

                    _Email.Text = "";

                    _Address.Text = "";

                    _PhoneNumber.Text = "";

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
                    MessageDialog dialog = new MessageDialog("Personal Data already recorded, Click 'Modify' to change your data.");
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
                string AccSelection = ((PersonalInfo)PersonalInfoListView.SelectedItem).firstName;
                if (AccSelection == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<PersonalInfo>();
                    var query1 = conn.Table<PersonalInfo>();
                    var query3 = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE firstName ='" + AccSelection + "'");
                    PersonalInfoListView.ItemsSource = query1.ToList();
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

        private async void ModifyPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Please modify any field and then save your changes.");
            await dialog.ShowAsync();

            PersonalInfoListView.SelectedIndex = 0;
            //Reactivate texboxes


            _FirstName.Text = ((PersonalInfo)PersonalInfoListView.SelectedItem).firstName.ToString();
            _LastName.Text = ((PersonalInfo)PersonalInfoListView.SelectedItem).lastName.ToString();

            Gender = ((PersonalInfo)PersonalInfoListView.SelectedItem).gender.ToString();

            if (Gender == "Male")
            {
                _Male.IsChecked = true;
            }
            else
            {
                _Female.IsChecked = true;
            }

            _Email.Text = ((PersonalInfo)PersonalInfoListView.SelectedItem).email.ToString();
            _Address.Text = ((PersonalInfo)PersonalInfoListView.SelectedItem).address;
            _PhoneNumber.Text = Convert.ToString(((PersonalInfo)PersonalInfoListView.SelectedItem).mobileNumber);
            _DOB1.Date = Convert.ToDateTime(((PersonalInfo)PersonalInfoListView.SelectedItem).DOB);


        }


        private void dateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {

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
