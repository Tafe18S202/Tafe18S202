/**
* @author Chase Wilksch-Bergroth
*
* @date - 18 Sep 2018
*/
using SQLite.Net;
using StartFinance.Models;
using System;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views {
    public sealed partial class ContactDetailsPage : Page {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetailsPage() {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        private void Results() {
            //Creating a Table
            conn.CreateTable<Contacts>();
            var query = conn.Table<Contacts>();
            ContactDetailsList.ItemsSource = query.ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            Results();
        }

        private async void AddItem_Click(object sender, RoutedEventArgs e) {
            try {
                //Check for errors and get formatted date data
                CheckInputErrors(FirstName, LastName, CompanyName, MobileNumber);

                Contacts contact = new Contacts() {
                    FirstName = FirstName.Text.Trim(),
                    LastName = LastName.Text.Trim(),
                    CompanyName = CompanyName.Text.Trim(),
                    MobileNumber = int.Parse(MobileNumber.Text)
                };

                conn.Insert(contact);

                Results();
                ClearFields();

            } catch (Exception ex) {
                if (ex is FormatException) {
                    MessageDialog dialog = new MessageDialog("You entered invalid data", "Oops..!");
                    await dialog.ShowAsync();
                } else if (ex is SQLiteException) {
                    MessageDialog dialog = new MessageDialog(ex.ToString(), "Oops..!");
                    await dialog.ShowAsync();
                } else {
                    MessageDialog dialog = new MessageDialog(ex.GetType().ToString(), "Some unhandled error!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void DeleteItem_Click(object send, RoutedEventArgs e) {
            if(ContactDetailsList.SelectedIndex == -1) {
                await SelectContactForMethod("delete");
            } else {
                MessageDialog ShowConf = new MessageDialog("Deleting this Contact will delete all details permanently.", "Important");
                ShowConf.Commands.Add(new UICommand("Yes, Delete") {
                    Id = 0
                });
                ShowConf.Commands.Add(new UICommand("Cancel") {
                    Id = 1
                });
                ShowConf.DefaultCommandIndex = 0;
                ShowConf.CancelCommandIndex = 1;

                var result = await ShowConf.ShowAsync();
                if ((int)result.Id == 0) {
                    try {
                        int ContactId = ((Contacts)ContactDetailsList.SelectedItem).ID;
                        var query = conn.Query<Contacts>("DELETE FROM Contacts WHERE ID='" + ContactId + "'");
                        Results();
                    } catch (NullReferenceException) {
                        await SelectContactForMethod("delete");
                    }
                } else {
                    //User cancelled deletion.
                }
            }
        }

        private async void EditItem_Click(object send, RoutedEventArgs e) {
            try {
                if(ContactDetailsList.SelectedIndex != -1) {
                    ShowEditDialog(send, e);
                }else {
                    await SelectContactForMethod("edit");
                }
            } catch (NullReferenceException) {
                await SelectContactForMethod("edit");
            }
        }

        private async void ShowEditDialog(object send, RoutedEventArgs e) {
            Contacts Info = (Contacts)ContactDetailsList.SelectedItem;
            Button btn = send as Button;
            //Set dialog title
            ContentDialog dialog = new ContentDialog() {
                Title = "Edit contact details for " + Info.FirstName + " " + Info.LastName + " who works at " + Info.CompanyName + ".",
                MaxWidth = this.ActualWidth
            };

            StackPanel panel = new StackPanel();
            //Create textbox fields for each text input
            TextBox firstName = CreateDialogTextBox("FirstNameEdit", "First Name", Info.FirstName);
            TextBox lastName = CreateDialogTextBox("LastNameEdit", "Last Name", Info.LastName);
            TextBox companyName = CreateDialogTextBox("CompanyEdit", "Company", Info.CompanyName);
            TextBox phone = CreateDialogTextBox("PhoneEdit", "Phone Number", Info.MobileNumber.ToString());

            //Add all inputs to panel
            panel.Children.Add(firstName);
            panel.Children.Add(lastName);
            panel.Children.Add(companyName);
            panel.Children.Add(phone);

            //Set dialog content
            dialog.Content = panel;

            //Handle dialog primary button
            dialog.PrimaryButtonText = "Edit";
            dialog.PrimaryButtonClick += delegate { btn.Content = "Result: Edit"; };

            //Handle dialog secondary button
            dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate { btn.Content = "Result: Cancel"; };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.None) {
                btn.Content = "Result: NONE";
            } else if (result == ContentDialogResult.Primary) {
                //Check for errors
                CheckInputErrors(firstName, lastName, companyName, phone);

                //Update data in database
                Contacts info = new Contacts() {
                    ID = Info.ID,
                    FirstName = firstName.Text.Trim(),
                    LastName = lastName.Text.Trim(),
                    CompanyName = companyName.Text.Trim(),
                    MobileNumber = int.Parse(phone.Text)
                };

                //Update db entry
                conn.Update(info);
                //Refresh results
                Results();
            }
        }

        private TextBox CreateDialogTextBox(string name, string header, string text) {
            return new TextBox {
                Name = name,
                Header = header,
                Margin = new Thickness(10, 10, 10, 10),
                Text = text
            };
        }

        private async void CheckInputErrors(TextBox firstName, TextBox lastName, TextBox companyName, TextBox phone) {
            int _phone = -1;

            if (firstName.Text.ToString().Trim() == "") {
                MessageDialog dialog = new MessageDialog("First name not entered!", "Ooops..!");
                await dialog.ShowAsync();
            } else if (lastName.Text.ToString().Trim() == "") {
                MessageDialog dialog = new MessageDialog("Last name not entered!", "Ooops..!");
                await dialog.ShowAsync();
            } else if (companyName.Text.ToString().Trim() == "") {
                MessageDialog dialog = new MessageDialog("Company name was not entered!", "Ooops..!");
                await dialog.ShowAsync();
            } else if (phone.Text.ToString() == "") {
                MessageDialog dialog = new MessageDialog("Phone number was not entered!", "Ooops..!");
                await dialog.ShowAsync();
            } else if (!int.TryParse(phone.Text.ToString(), out _phone)) {
                MessageDialog dialog = new MessageDialog("You must enter a valid phone number!", "Ooops..!");
                await dialog.ShowAsync();
            }
        }

        private static async System.Threading.Tasks.Task SelectContactForMethod(string methodType) {
            MessageDialog clearDialog = new MessageDialog("Please selected a contact to " + methodType + "!", "Oops..!");
            await clearDialog.ShowAsync();
        }

        private void ClearFields() {
            FirstName.Text = "";
            LastName.Text = "";
            CompanyName.Text = "";
            MobileNumber.Text = "";
        }
    }
}
