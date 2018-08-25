using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotifyMe.Data.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }


    }

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

    public class Message
    {
        public long Id { get; set; }
        public string ToUser { get; set; }
        public string FromUser { get; set; } 
        public string Content { get; set; }

        public string RawContent { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
