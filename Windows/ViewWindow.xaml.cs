using CMS_gigabyte_graphic_cards.Models;
using CMS_gigabyte_graphic_cards.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Path = System.IO.Path;

namespace CMS_gigabyte_graphic_card.Windows
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {

        #region Initialization

        public User savedUser = new User();
        public ViewWindow(GraphicCard card, User user)
        {
            InitializeComponent();
            nameLabel.Content = card.RtfFilePath;
            savedUser = user;

            TableWindow tableWindow = new TableWindow(savedUser);

            imagePreview.Source = new BitmapImage(new Uri(card.ImagePath, UriKind.RelativeOrAbsolute));
            string imageName = Path.GetFileName(card.ImagePath);
            selectedImageNameLabel.Content = imageName;

            activeUsersNumberLabel.Content = card.ActiveUsersField.ToString();

            try
            {
                tableWindow.RefreshGraphicCards();
                string rtfilePath = "../../RTF/" + card.RtfFilePath + ".rtf";
                //string rtfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RTF", card.RtfFilePath + ".rtf");

                if (File.Exists(rtfilePath))
                {
                    TextRange range = new TextRange(richTextBoxView.Document.ContentStart, richTextBoxView.Document.ContentEnd);
                    using (FileStream fs = new FileStream(rtfilePath, FileMode.Open))
                    {
                        range.Load(fs, DataFormats.Rtf);
                    }
                }
                else
                {
                    MessageBox.Show("RTF file not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the RTF file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            string formattedDate = card.DateAdded.ToString("dd-MM-yyyy");
            dateAddedNumberLabel.Content = formattedDate;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            TableWindow tableWindow = new TableWindow(savedUser);
            tableWindow.Show();
            this.Close();
        }
    }
}
