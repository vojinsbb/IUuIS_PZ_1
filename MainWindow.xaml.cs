using CMS_gigabyte_graphic_cards.Models;
using CMS_gigabyte_graphic_cards.Windows;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Path = System.IO.Path;


namespace CMS_gigabyte_graphic_cards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //LOGOVANJE: admin admin123 ili guest guest123
        //VAZNO ZA AddWindow: 1. Ako dodajemo sliku direktno iz Images foldera ona ce se videti u tabeli
        //                    2. Ako se dodaje bilo gde sa racunara ona ce se kopirati u folder Images u projektu
        //                    ali se nece videti u TableWindow.xaml.cs u tabeli (sitna greska u kodu)

        #region Initialization

        private const string usersFilePath = "../../DataBase/users.xml";
        //private string usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase", "users.xml");
        private List<User> users;
        private NotificationManager notificationManager;
        public MainWindow()
        {
            InitializeComponent();
            notificationManager = new NotificationManager();
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                using (FileStream stream = new FileStream(usersFilePath, FileMode.Open))
                {
                    users = (List<User>)serializer.Deserialize(stream);
                }
            }
            else
            {
                users = new List<User>
                {
                    new User {Username = "admin", Password = "admin123", Role = UserRole.Admin },
                    new User {Username = "guest", Password = "guest123", Role = UserRole.Visitor }
                };
            }
        }

        private void SaveUsers()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (FileStream stream = new FileStream(usersFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, users);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

        #region Validation
        private bool Validation()
        {
            bool retVal = true;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            if (UsernameTextBox.Text.Equals(string.Empty))
            {
                mainWindow.SendToastNotification("Invalid username", "Username field can't be empty!", NotificationType.Error);
                retVal = false;
                UsernameErrorLabel.Content = "Field can't be empty!";
                UsernameTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                UsernameErrorLabel.Content = string.Empty;
                UsernameTextBox.BorderBrush = Brushes.Black;
            }

            if (PasswordTextBox.SecurePassword.Length == 0)
            {
                mainWindow.SendToastNotification("Invalid password", "password field can't be empty!", NotificationType.Error);
                retVal = false;
                PasswordErrorLabel.Content = "Field can't be empty";
                PasswordTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                PasswordErrorLabel.Content = string.Empty;
                PasswordTextBox.BorderBrush = Brushes.Black;
            }

            return retVal;
        }
        #endregion

        #region ToastNotifications
        private void ShowToastNotification(ToastNotification toastNotification)
        {
            notificationManager.Show(toastNotification.Title, toastNotification.Message, toastNotification.Type, "WindowsNotificationArea");
        }

        private void SendToastNotification(string title, string message, NotificationType type)
        {
            ShowToastNotification(new ToastNotification(title, message, type));
        }

        #endregion

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            if (Validation())
            {
                User user = users.Find(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    TableWindow tableWindow = new TableWindow(user);
                    tableWindow.Show();
                    this.Close();
                }
                else
                {
                    mainWindow.SendToastNotification("Invalid username/password", "Invalid username or password!", NotificationType.Error);
                    UsernameErrorLabel.Content = "Invalid username!";
                    UsernameTextBox.Text = "";
                    UsernameTextBox.BorderBrush = Brushes.Red;
                    PasswordErrorLabel.Content = "Invalid password!";
                    PasswordTextBox.Clear();
                    PasswordTextBox.BorderBrush = Brushes.Red;
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs args)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveUsers();
                this.Close();
            }
        }
    }
}
