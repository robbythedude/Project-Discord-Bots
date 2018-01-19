using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoBot.Models
{
    public class Reminder
    {
        public string User { get; set; }
        public string TimeToTrigger { get; set; }
        public string Message { get; set; }
    }
}
