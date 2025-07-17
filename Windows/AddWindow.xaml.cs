using CMS_gigabyte_graphic_cards.Models;
using CMS_gigabyte_graphic_cards.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace CMS_gigabyte_graphic_card.Windows
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        #region Initialization

        public string savedPath = "";
        public string savedImageName = "";
        public string startFileName = "";
        public User savedUser = new User();
        public ObservableCollection<GraphicCard> GraphicCards { get; set; }
        public AddWindow(User user)
        {
            InitializeComponent();
            savedUser = user;

            filePathRtf.Text = "Input file name";
            var bc = new BrushConverter();
            filePathRtf.Foreground = (Brush)bc.ConvertFrom("#717286");

            activeUsersTextBox.Text = "Input number of users";
            activeUsersTextBox.Foreground = (Brush)bc.ConvertFrom("#717286");

            fontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            colorComboBox.ItemsSource = typeof(Colors).GetProperties()
                                            .Where(p => p.PropertyType == typeof(Color))
                                            .Select(p => (Color)p.GetValue(null))
                                            .OrderBy(c => c.ToString())
                                            .ToList();

            fontSizeComboBox.ItemsSource = Enumerable.Range(1, 30).Select(i => (double)i).ToList();

            DataContext = this;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

        #region PreviewTextInput
        private void filePathRtfTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length >= 25)
            {
                e.Handled = true; // onemoguci dalje unosenje teksta ako je duzina veca od 25 karaktera
                return;
            }

            Regex regex = new Regex("[^a-zA-Z0-9():_-]");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true; // onemoguci unos karaktera koji nisu slova, brojevi, zagrade, dvotačka, donja crta ili crtice
            }
        }

        private void activeUsersTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // onemoguci unos karaktera koji nisu cifre
                return;
            }

            TextBox textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            int value;
            if (!int.TryParse(newText, out value))
            {
                e.Handled = true; // onemoguci unos ako bi rezultat bio van opsega int
            }
        }

        #endregion

        #region SelectionChanged
        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem != null && !editorRichTextBox.Selection.IsEmpty)
            {
                editorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
        }

        private void fontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontSizeComboBox.SelectedItem != null && !editorRichTextBox.Selection.IsEmpty)
            {
                editorRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.SelectedItem);
            }
        }

        private void colorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorComboBox.SelectedItem != null && !editorRichTextBox.Selection.IsEmpty)
            {
                if (colorComboBox.SelectedItem is Color selectedColor)
                {
                    SolidColorBrush brush = new SolidColorBrush(selectedColor);
                    editorRichTextBox.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
                }
            }
        }

        private void editorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontWeight = editorRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            boldButton.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && (fontWeight.Equals(FontWeights.Bold));

            object fontStyle = editorRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            italicButton.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && (fontStyle.Equals(FontStyles.Italic));

            object textDecoration = editorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineButton.IsChecked = (textDecoration != DependencyProperty.UnsetValue) && (textDecoration.Equals(TextDecorations.Underline));

            object fontFamily = editorRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            fontFamilyComboBox.SelectedItem = fontFamily;

            object foregroundColor = editorRichTextBox.Selection.GetPropertyValue(Inline.ForegroundProperty);
            if (foregroundColor is SolidColorBrush brush)
            {
                Color selectedColor = brush.Color;
                colorComboBox.SelectedItem = selectedColor;
            }
            
            object fontSize = editorRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue)
            {
                fontSizeComboBox.SelectedItem = (double)fontSize;
            }
        }

        #endregion

        #region Validation

        private bool Validation()
        {
            bool retVal = true;
            string input  = activeUsersTextBox.Text;

            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    int result = int.Parse(input);
                }
                catch (FormatException)
                {
                    retVal = false;
                    filePathRtfErrorLabel.Content = "Field can't be empty!";
                    filePathRtf.BorderBrush  = Brushes.Red;
                }
            }

            if (filePathRtf.Text.Trim().Equals(string.Empty) || filePathRtf.Text.Trim().Equals("Input file name"))
            {
                retVal = false;
                filePathRtfErrorLabel.Content = "Field can't be empty!";
                filePathRtf.BorderBrush = Brushes.Red;
            }
            else
            {
                filePathRtfErrorLabel.Content = string.Empty;
                filePathRtf.BorderBrush = Brushes.Black;
            }

            if (activeUsersTextBox.Text.Trim().Equals(string.Empty) || activeUsersTextBox.Text.Trim().Equals("Input number of users"))
            {
                retVal = false;
                activeUsersErrorLabel.Content = "Field can't be empty!";
                activeUsersTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                activeUsersErrorLabel.Content = string.Empty;
                activeUsersTextBox.BorderBrush = Brushes.Black;
            }

            if (selectedImageNameLabel.Content.ToString().Trim() == string.Empty || selectedImageNameLabel.Content.ToString() == "Image must be added!")
            {
                retVal = false;
                selectedImageNameLabel.Content = "Image must be added!";
                selectedImageNameLabel.Foreground = Brushes.Red;
                borderForImage.BorderBrush = Brushes.Red;
            }

            string xmlFilePath = filePathRtf.Text.Trim();

            if (File.Exists(xmlFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<GraphicCard>));
                using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open))
                {
                    GraphicCards = (ObservableCollection<GraphicCard>)serializer.Deserialize(fs);
                }
            }
            else
            {
                GraphicCards = new ObservableCollection<GraphicCard>();
            }

            foreach (GraphicCard card in GraphicCards)
            {
                if (card.RtfFilePath == filePathRtf.Text.Trim())
                {
                    retVal = false;
                    filePathRtfErrorLabel.Content = "Name is already in use";
                    filePathRtf.BorderBrush = Brushes.Red;
                    break;
                }
            }

            return retVal;
        }

        #endregion

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                string validName = filePathRtf.Text.Trim();
                string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase", "graphic_card.xml");

                GraphicCard card = new GraphicCard(Convert.ToInt32(activeUsersTextBox.Text), new TextRange(editorRichTextBox.Document.ContentStart, editorRichTextBox.Document.ContentEnd).Text, savedImageName, validName);

                if (File.Exists(xmlFilePath))
                {
                    List<GraphicCard> cards;
                    XmlSerializer serializer = new XmlSerializer(typeof(List<GraphicCard>));
                    using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open))
                    {
                        cards = (List<GraphicCard>)serializer.Deserialize(fs);
                    }

                    cards.Add(card);

                    using (FileStream fs = new FileStream(xmlFilePath, FileMode.Create))
                    {
                        serializer.Serialize(fs, cards);
                    }
                }
                else
                {
                    using (TextWriter writer = new StreamWriter(xmlFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<GraphicCard>));
                        serializer.Serialize(writer, new List<GraphicCard> { card });
                    }
                }

                string folderName = "../../RTF";
                string folderPath = Path.Combine(Environment.CurrentDirectory, folderName);

                if(!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string rtfFilePath = Path.Combine(folderPath, validName);

                if (!rtfFilePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                {
                    rtfFilePath += ".rtf";
                }

                TextRange range = new TextRange(editorRichTextBox.Document.ContentStart, editorRichTextBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(rtfFilePath, FileMode.Create))
                {
                    range.Save(fs, DataFormats.Rtf);
                }

                activeUsersTextBox.Text = "";
                filePathRtf.Text = "";
                editorRichTextBox.Document.Blocks.Clear();
                imagePreview.Source = null;
                selectedImageNameLabel.Content = "";
                MessageBox.Show("New Graphic Card successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                TableWindow tableWindow = new TableWindow(savedUser);
                tableWindow.LoadGraphicCardsFromXml();
                tableWindow.Show();
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(filePathRtf.Text) || filePathRtf.Text.Trim() == "Input file name") 
                && (string.IsNullOrWhiteSpace(activeUsersTextBox.Text) || activeUsersTextBox.Text.Trim() == "Input number of users") 
                && imagePreview.Source == null 
                && string.IsNullOrWhiteSpace(new TextRange(editorRichTextBox.Document.ContentStart, editorRichTextBox.Document.ContentEnd).Text))
            {
                TableWindow tableWindow = new TableWindow(savedUser);
                tableWindow.Show();
                this.Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    TableWindow tableWindow = new TableWindow(savedUser);
                    tableWindow.RefreshGraphicCards();
                    tableWindow.Show();
                    this.Close();

                }
            }
            
        }

        private void addImageButton_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.svg) | *.jpg; *.jpeg; *.png; *.gif; *.svg";

            //if (openFileDialog.ShowDialog() == true)
            //{
            //    string selectedImagePath = openFileDialog.FileName;
            //    savedPath = selectedImagePath;
            //    BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
            //    imagePreview.Source = bitmapImage;
            //    string selectedImageName = Path.GetFileName(selectedImagePath);
            //    selectedImageNameLabel.Content = selectedImageName;
            //    savedImageName =  "../Images/" + selectedImageName;
            //    selectedImageNameLabel.Foreground = Brushes.Black;
            //    borderForImage.BorderBrush = Brushes.Black;
            //}

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.svg) | *.jpg; *.jpeg; *.png; *.gif; *.svg";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                string selectedImageName = Path.GetFileName(selectedImagePath);

                // odredi folder gde treba da se kopira slika
                string destinationFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Images");
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                string destinationPath = Path.Combine(destinationFolder, selectedImageName);

                // ako slika vec ne postoji u folderu kopiraj je
                if (!File.Exists(destinationPath))
                {
                    File.Copy(selectedImagePath, destinationPath);
                }

                // postavi preview slike
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(destinationPath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                imagePreview.Source = bitmapImage;

                // postavi labelu i zapamti relativnu putanju
                selectedImageNameLabel.Content = selectedImageName;
                selectedImageNameLabel.Foreground = Brushes.Black;
                borderForImage.BorderBrush = Brushes.Black;

                // Relativna putanja koju koristiš npr. u XML fajlu
                savedImageName = "../Images/" + selectedImageName;
            }
        }

        #region Word count
        private void UpdateWordCount()
        {
            string text = new TextRange(editorRichTextBox.Document.ContentStart, editorRichTextBox.Document.ContentEnd).Text;
            int wordCount = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            wordCountTextBlock.Text = $"Word Count: {wordCount}";
        }

        private void editorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWordCount();
        }

        #endregion

        #region Got/Lost Focus

        private void filePathRtf_GotFocus(object sender, RoutedEventArgs e)
        {
            if(filePathRtf.Text.Trim().Equals("Input file name"))
            {
                filePathRtf.Text = "";
                filePathRtf.Foreground = Brushes.Black;
            }
            filePathRtfErrorLabel.Content = "";
            filePathRtf.BorderBrush = Brushes.Black;
        }

        private void filePathRtf_LostFocus(object sender, RoutedEventArgs e)
        {
            if (filePathRtf.Text.Trim().Equals(string.Empty))
            {
                filePathRtf.Text = "Input file name";
                var bc = new BrushConverter();
                filePathRtf.Foreground = (Brush)bc.ConvertFrom("#717286");
            }
        }

        private void activeUsersTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(activeUsersTextBox.Text.Trim().Equals("Input number of users"))
            {
                activeUsersTextBox.Text = "";
                activeUsersTextBox.Foreground = Brushes.Black;
            }
            activeUsersErrorLabel.Content = "";
            activeUsersTextBox.BorderBrush = Brushes.Black;
        }

        private void activeUsersTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (activeUsersTextBox.Text.Trim().Equals(string.Empty))
            {
                activeUsersTextBox.Text = "Input number of users";
                var bc = new BrushConverter();
                activeUsersTextBox.Foreground = (Brush)bc.ConvertFrom("#717286");
            }
        }
        #endregion
    }
}
