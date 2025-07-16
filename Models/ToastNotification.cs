using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_gigabyte_graphic_cards.Models
{
    public class ToastNotification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }

        public ToastNotification()
        {

        }
        public ToastNotification(string title, string message, NotificationType type)
        {
            Title = title;
            Message = message;
            Type = type;
        }
    }
}
