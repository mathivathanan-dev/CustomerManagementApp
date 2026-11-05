
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace CustomerManagementApp
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Customer> customers =
            new ObservableCollection<Customer>();

        string fullpath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "user.txt");

        public MainWindow()
        {
            InitializeComponent();

            LoadCustomers();

            dgCustomers.ItemsSource = customers;

            cmbCustomer.DisplayMemberPath = "Name";
            cmbCustomer.ItemsSource = customers;

            if (customers.Count > 0)
            {
                cmbCustomer.SelectedIndex = 0;
                dgCustomers.SelectedIndex = 0;
            }

            UpdateCount();
        }

        private void LoadCustomers()
        {
            try
            {
                if (!File.Exists(fullpath))
                {
                    Directory.CreateDirectory(
                        Path.GetDirectoryName(fullpath));

                    File.WriteAllText(fullpath, "[]");
                    return;
                }

                string jsondata = File.ReadAllText(fullpath);

                if (!string.IsNullOrWhiteSpace(jsondata))
                {
                    var data =
                        JsonConvert.DeserializeObject
                        <ObservableCollection<Customer>>(jsondata);

                    if (data != null)
                    {
                        foreach (var customer in data)
                        {
                            customers.Add(customer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading customers: " + ex.Message);
            }
        }

        private void SaveCustomers()
        {
            try
            {
                string jsondata =
                    JsonConvert.SerializeObject(
                        customers,
                        Formatting.Indented);

                File.WriteAllText(fullpath, jsondata);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving customers: " + ex.Message);
            }
        }

        private void btnRegister_Click(
            object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please fill all details");
                return;
            }

            bool exists = customers.Any(c =>
                c.Phone == txtPhone.Text.Trim());

            if (exists)
            {
                MessageBox.Show("Customer already exists!");
                return;
            }

            Customer newCustomer = new Customer
            {
                Id = customers.Count > 0
                    ? customers.Max(c => c.Id) + 1
                    : 1,

                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Location = txtLocation.Text.Trim()
            };

            customers.Add(newCustomer);

            SaveCustomers();

            txtSearch.Clear();

            dgCustomers.ItemsSource = customers;

            cmbCustomer.SelectedItem = newCustomer;
            dgCustomers.SelectedItem = newCustomer;

            UpdateCount();

            txtStatus.Text =
                newCustomer.Name + " registered successfully";

            MessageBox.Show(
                "Customer Registered Successfully!");
        }

        private void cmbCustomer_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbCustomer.SelectedItem is Customer selected)
            {
                txtName.Text = selected.Name;
                txtPhone.Text = selected.Phone;
                txtEmail.Text = selected.Email;
                txtLocation.Text = selected.Location;

                dgCustomers.SelectedItem = selected;
                dgCustomers.ScrollIntoView(selected);
            }
        }

        private void btnSearch_Click(
            object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgCustomers.ItemsSource = customers;

                txtStatus.Text = "Showing all customers";
                return;
            }

            var listitem =
                from l in customers
                where l.Name != null &&
                      l.Name.ToLower().Contains(
                          searchText.ToLower())
                orderby l.Name descending
                select l;

            var searchResults = listitem.ToList();

            dgCustomers.ItemsSource = searchResults;

            if (searchResults.Count > 0)
            {
                txtStatus.Text =
                    searchResults.Count + " customer(s) found";
            }
            else
            {
                txtStatus.Text = "No customers found";

                MessageBox.Show(
                    "No matching customers found!");
            }
        }

        private void UpdateCount()
        {
            txtCount.Text = customers.Count +
                (customers.Count == 1 ? " user" : " users");
        }
    }
}
