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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentEditPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        Appointments appointment;

        public AppointmentEditPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                var p = Template10.Services.SerializationService.SerializationService.Json.Deserialize<int>(e.Parameter?.ToString());
                int id = (int)p;
                appointment = conn.Get<Appointments>(id);
                txtEventName.Text = appointment.EventName;
                txtLocation.Text = appointment.Location;
                calEventDate.Date = DateTimeOffset.Parse(appointment.EventDate);
                timStartTime.Time = TimeSpan.Parse(appointment.StartTime);
                timEndTime.Time = TimeSpan.Parse(appointment.EndTime);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
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

            if (timStartTime.Time > timEndTime.Time)
            {
                await new MessageDialog("Start date must be before end date!").ShowAsync();
                timStartTime.Focus(FocusState.Programmatic);
                return;
            }

            try
            {
                string CDay = calEventDate.Date.Value.Day.ToString();
                string CMonth = calEventDate.Date.Value.Month.ToString();
                string CYear = calEventDate.Date.Value.Year.ToString();
                string FinalDate = CDay + "/" + CMonth + "/" + CYear;

                string SHour = timStartTime.Time.Hours.ToString();
                string SMin = timStartTime.Time.Minutes.ToString();
                string FinalSTime = SHour + ":" + SMin;

                string EHour = timEndTime.Time.Hours.ToString();
                string EMin = timEndTime.Time.Minutes.ToString();
                string FinalETime = EHour + ":" + EMin;

                appointment.EventName = txtEventName.Text;
                appointment.Location = txtLocation.Text;
                appointment.EventDate = FinalDate;
                appointment.StartTime = FinalSTime;
                appointment.EndTime = FinalETime;

                conn.Update(appointment);

                await new MessageDialog("Appointment Updated", "Success").ShowAsync();

                Frame.Navigate(typeof(AppointmentListPage));
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AppointmentListPage));
        }
    }        
}
