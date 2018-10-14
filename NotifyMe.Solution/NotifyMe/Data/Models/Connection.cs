using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotifyMe.Data.Models
{
    public class Connection
    {
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
        public DateTime ConnectionDate { get; set; }
        public DateTime DisconnectionDate { get; set; }
        public ICollection<Message> Messages { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}