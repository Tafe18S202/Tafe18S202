using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentAddPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentAddPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            conn.CreateTable<Appointments>();
            calEventDate.Date = DateTime.Now; // gets current date and time
            timStartTime.Time = DateTime.Now.TimeOfDay;
            timEndTime.Time = timStartTime.Time.Add(new TimeSpan(1, 0, 0));
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                await new MessageDialog("Please enter an event name").ShowAsync();
                txtEventName.Focus(FocusState.Programmatic);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                await new MessageDialog("Please enter a location").ShowAsync();
                txtLocation.Focus(FocusState.Programmatic);
                return;
            }

            try
            {
                conn.Insert(new Appointments()
                {
                    EventName = txtEventName.Text,
                    Location = txtLocation.Text,
                    EventDate = calEventDate.Date.Value.Date.AddDays(1),
                    StartTime = timStartTime.Time,
                    EndTime = timEndTime.Time
                });

                await new MessageDialog("Appointment Added", "Success").ShowAsync();
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Name, Location or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }
    }
}
