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
            if (File.Exists(fullpath))
            {
                string jsondata = File.ReadAllText(fullpath);

                var data =
                    JsonConvert.DeserializeObject
                    <ObservableCollection<Customer>>(jsondata);

                if (data != null)
                {
                    customers = data;
                }
            }
            else
            {
                File.WriteAllText(fullpath, "[]");
            }
        }

        private void SaveCustomers()
        {
            string jsondata =
                JsonConvert.SerializeObject(
                    customers,
                    Formatting.Indented);

            File.WriteAllText(fullpath, jsondata);
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

            Customer newCustomer = new Customer
            {
                Id = customers.Count > 0
                    ? customers.Max(c => c.Id) + 1
                    : 0,

                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Location = txtLocation.Text.Trim()
            };

            customers.Add(newCustomer);

            SaveCustomers();

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

        private void UpdateCount()
        {
            txtCount.Text = customers.Count +
                (customers.Count == 1 ? " user" : " users");
        }
    }
}