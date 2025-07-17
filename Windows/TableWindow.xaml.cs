using CMS_gigabyte_graphic_card.Windows;
using CMS_gigabyte_graphic_cards.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace CMS_gigabyte_graphic_cards.Windows
{
    /// <summary>
    /// Interaction logic for TableWindow.xaml
    /// </summary>
    public partial class TableWindow : Window, INotifyPropertyChanged
    {
        #region Initialization

        public ObservableCollection<GraphicCard> GraphicCards { get; set; }
        public User savedUser = new User();

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    OnPropertyChanged(nameof(IsAllSelected));
                    // oznaci sve redove kao selektovane ili ne
                    foreach (var card in GraphicCards)
                    {
                        card.IsSelected = _isAllSelected;
                    }
                }
            }
        }

        public TableWindow(User user)
        {
            InitializeComponent();
            savedUser = user;
            if (user.Role == UserRole.Admin)
            {
                AddButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
                SelectColumn.Visibility = Visibility.Visible;
            }
            else
            {
                AddButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                SelectColumn.Visibility = Visibility.Hidden;
            }

            DataContext = this;
            LoadGraphicCardsFromXml();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadGraphicCardsFromXml()
        {
            string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase", "graphic_card.xml");

            if (File.Exists(xmlFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<GraphicCard>));
                using (FileStream filestream = new FileStream(xmlFilePath, FileMode.Open))
                {
                    GraphicCards = (ObservableCollection<GraphicCard>)serializer.Deserialize(filestream);
                }
            }
            else
            {
                GraphicCards = new ObservableCollection<GraphicCard>();
            }

            GraphicCardDataGrid.ItemsSource = GraphicCards;
        }

        public void RefreshGraphicCards()
        {
            LoadGraphicCardsFromXml();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GraphicCardDataGrid.Items.SortDescriptions.Add(new SortDescription("DateAdded", ListSortDirection.Descending));
        }

        #endregion

        private void HyperLink_Click(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            var item = textBlock.DataContext as GraphicCard;

            if (item != null)
            {
                if (savedUser.Role == UserRole.Admin)
                {
                    //EditWindow editWindow = new EditWindow(item, savedUser);
                    //editWindow.Show();
                    //this.Close();
                }
                else
                {
                    //ViewWindow viewWindow = new ViewWindow(item, savedUser);
                    //viewWindow.Show();
                    //this.Close();
                }
            }
            else
            {
                MessageBox.Show("Item not found.");
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddWindow addWindow = new AddWindow(savedUser);
                addWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška prilikom otvaranja AddWindow: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sing out?", "Sing out Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            foreach (GraphicCard selectedCard in GraphicCardDataGrid.Items)
            {
                if (selectedCard.IsSelected)
                {
                    counter++;
                    break;
                }
            }

            if (counter != 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sre you want to remove these items?", "Delete items Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TableWindow tableWindow = (TableWindow)Application.Current.MainWindow;
                    List<GraphicCard> remainingCards = new List<GraphicCard>();

                    foreach (GraphicCard card in GraphicCardDataGrid.Items)
                    {
                        if (card.IsSelected)
                        {
                            string rtfFilePath = "../../RTF/" + card.RtfFilePath;
                            if (!rtfFilePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                            {
                                rtfFilePath += ".rtf";
                            }

                            if (File.Exists(rtfFilePath))
                            {
                                File.Delete(rtfFilePath);
                            }
                            continue;
                        }
                        remainingCards.Add(card);
                    }

                    GraphicCardDataGrid.ItemsSource = remainingCards;

                    XmlSerializer serializer = new XmlSerializer(typeof(List<GraphicCard>));
                    using (TextWriter tw = new StreamWriter("../../DataBase/graphic_card.xml"))
                    {
                        serializer.Serialize(tw, remainingCards);
                    }

                    MessageBox.Show("All items have been successfully deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No item has been selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
