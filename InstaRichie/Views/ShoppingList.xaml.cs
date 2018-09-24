// **************************************************************************
//Start Finance - An to manage your personal finances.

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

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
    public sealed partial class ShoppingList : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingList()
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
            _ShopName.PlaceholderText = _ShopName.PlaceholderText;
            _ShopName.Text = "";
            _ShoppingItemName.PlaceholderText = _ShoppingItemName.PlaceholderText;
            _ShoppingItemName.Text = "";
            _ShoppingPriceQuoted.PlaceholderText = _ShoppingPriceQuoted.PlaceholderText;
            _ShoppingPriceQuoted.Text = "";
            _ShoppingDate.PlaceholderText = _ShoppingDate.PlaceholderText;
            DateTime time = DateTime.Now;
            _ShoppingDate.Date = time;

            conn.CreateTable<Models.ShoppingList>(); // Do not name things the same name!!!
            var query1 = conn.Table<Models.ShoppingList>();
            ShoppingListView.ItemsSource = query1.ToList();

        }

        private void _ShoppingDate_DateChanged_1(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            CalendarDatePicker calDate = sender as CalendarDatePicker;
            string date = "";

            date = "" + calDate.Date;
        }

        private async void AddBarButton_Click_2(object sender, RoutedEventArgs e)
        {

            try
            {
                if ((_ShopName.Text.ToString() == "") || (_ShoppingItemName.Text.ToString() == ""))
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    CalendarDatePicker calDate = _ShoppingDate as CalendarDatePicker;
                    string date = "";
                    date = "" + calDate.Date;
                    var tree = _ShoppingDate.Date;
                    DateTime time = tree.Value.DateTime;
                    var forDate = time.ToString("dd/MM/yyyy");

                    double TempMoney = Convert.ToDouble(_ShoppingPriceQuoted.Text);
                    conn.CreateTable<Models.ShoppingList>();
                    conn.Insert(new Models.ShoppingList // use different names next time - referenced the model instead for now.
                    {
                        ShopName = _ShopName.Text.ToString(),
                        NameOfItem = _ShoppingItemName.Text.ToString(),
                        ShoppingDate = forDate,
                        //ShoppingDate = _ShoppingDate.Date.Date, // TO DO, implement code for date picker
                        PriceQuoted = Double.Parse(_ShoppingPriceQuoted.Text.ToString()), // inside bracket for double
                    });
                    // Creating table
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Shopping List Name already exist, Try Different Name", "Oops..!" + ex.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int IDSelection = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID;
                string AccSelection = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShopName; // again, had to make it Models.ShoppingList - dont name things the same
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("An Item needs to be selected for removal.", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Models.ShoppingList>();
                    var query1 = conn.Table<Models.ShoppingList>();
                    var query3 = conn.Query<Models.ShoppingList>("DELETE FROM ShoppingList WHERE ShoppingItemID ='" + IDSelection + "'");

                    //UPDATE: Below problem has been fixed.
                    //this needs to delete the id associated with the selected shopName - currently deletes all items with that shopName ***FIX TO DO***
                    ShoppingListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("You have not selected an Item to DELETE.", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }


        private async void EditBarButton_Click(object sender, RoutedEventArgs e)
        {

            // TO DO - Google how to do
            try
            {
                int IDSelection = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID;
                string AccSelection = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShopName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("You have not selected an Item.", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //Enable the appbar button to save the edit details, disable ability to delete or add items whilst editing
                    saveButAppBar.IsEnabled = true;
                    cancelButAppBar.IsEnabled = true;
                    deleteButAppBar.IsEnabled = false;
                    addButAppBar.IsEnabled = false;
                    editButAppBar.IsEnabled = false;

                    string iDate = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShoppingDate;
                    DateTime dtDate;
                    DateTime.TryParse(iDate, out dtDate);

                    _ShopName.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShopName;
                    _ShoppingItemName.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                    _ShoppingPriceQuoted.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).PriceQuoted.ToString();
                    //_ShoppingDate.Date = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShoppingDate;
                    _ShoppingDate.Date = dtDate;

                    //CalendarDatePicker calDate = _ShoppingDate as CalendarDatePicker;
                    //string date = "";
                    //date = "" + calDate.Date;
                    //var tree = _ShoppingDate.Date;
                    //DateTime time = tree.Value.DateTime;
                    //var forDate = time.ToString("dd/MM/yyyy");

                    // --- Populated the list with selected values - need to add amend button OR make a new form for the editing <- probably this

                    conn.CreateTable<Models.ShoppingList>();
                    var query1 = conn.Table<Models.ShoppingList>();

                    //var query3 = conn.Query<Models.ShoppingList>("UPDATE ShoppingList SET ShopName ='" + _ShopName + "'" + ", NameOfItem = '" + _ShoppingItemName +
                    //    //", ShoppingDate = '" + forDate + "'" + 
                    //    ", PriceQuoted = '" + _ShoppingPriceQuoted + "WHERE ShopName =" + AccSelection + "'");
                    //ShoppingListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("You have not selected an Item to EDIT.", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void saveButAppBar_Click(object sender, RoutedEventArgs e)
        {
            //Disable the save edit details appBar button, re-enable add and delete buttons
            saveButAppBar.IsEnabled = false;
            cancelButAppBar.IsEnabled = false;
            deleteButAppBar.IsEnabled = true;
            addButAppBar.IsEnabled = true;
            editButAppBar.IsEnabled = true;

            try
            {
                int IDSelection = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID;
                string AccSelection = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShopName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("You have not selected an Item to UPDATE.", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    CalendarDatePicker calDate = _ShoppingDate as CalendarDatePicker;
                    string date = "";
                    date = "" + calDate.Date;
                    var tree = _ShoppingDate.Date;
                    DateTime time = tree.Value.DateTime;
                    var forDate = time.ToString("dd/MM/yyyy");

                    string sName = _ShopName.Text.ToString();
                    string sItem = _ShoppingItemName.Text.ToString();
                    double sPrice = Double.Parse(_ShoppingPriceQuoted.Text.ToString());
                    string sDate = forDate;

                    var query1 = conn.Table<Models.ShoppingList>();
                    var query3 = conn.Query<Models.ShoppingList>("UPDATE ShoppingList SET ShopName ='" + sName + "'" + ", NameOfItem = '" + sItem + "'" + ", ShoppingDate = '" + sDate + "'" + ", PriceQuoted = '" + sPrice + "' WHERE ShoppingItemID = '" + IDSelection + "'");

                    //Not right code
                    //var query1 = conn.Table<Models.ShoppingList>();
                    //var query2 = conn.Query<Models.ShoppingList>("UPDATE ShoppingList SET ShopName ='" + sName + "'" + ", NameOfItem = '" + sItem + "'");

                    //not sure if this belwo is needed
                    //ShoppingListView.ItemsSource = query1.ToList();

                    //Ignore below code, for reference only
                    //((Models.ShoppingList)ShoppingListView.SelectedItem).ShopName = _ShopName.Text;
                    //((Models.ShoppingList)ShoppingListView.SelectedItem).ShopName = sName;

                    Results();

                }
            }

            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Database ERROR.", "Oops..!");
                await dialog.ShowAsync();
            }

        }

        private void cancelButAppBar_Click(object sender, RoutedEventArgs e)
        {
            saveButAppBar.IsEnabled = false;
            cancelButAppBar.IsEnabled = false;
            deleteButAppBar.IsEnabled = true;
            addButAppBar.IsEnabled = true;
            editButAppBar.IsEnabled = true;
            Results();
        }
    }
}
