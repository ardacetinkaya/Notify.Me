using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotifyMe.Data.Models
{
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
