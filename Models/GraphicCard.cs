using Notifications.Wpf.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_gigabyte_graphic_cards.Models
{
    public class GraphicCard
    {
        public bool IsSelected { get; set; }
        public int ActiveUsersField { get; set; }
        public string DescriptionField { get; set; }
        public string ImagePath { get; set; }
        public string RtfFilePath { get; set; }
        public DateTime DateAdded { get; set; }

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
