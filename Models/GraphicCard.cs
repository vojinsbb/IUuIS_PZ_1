using Notifications.Wpf.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CMS_gigabyte_graphic_cards.Models
{
    public class GraphicCard : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        public int ActiveUsersField { get; set; }
        public string DescriptionField { get; set; }
        public string ImagePath { get; set; }
        public string RtfFilePath { get; set; }
        public DateTime DateAdded { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GraphicCard(int activeUserField, string descriptionField, string imagePath, string rtfFilePath)
        {
            IsSelected = false;
            ActiveUsersField = activeUserField;
            DescriptionField = descriptionField;
            ImagePath = imagePath;
            RtfFilePath = rtfFilePath;
            DateAdded = DateTime.Now;
        }

        public GraphicCard()
        {

        }
    }
}
