using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotifyMe.Data.Models
{
    public class ApplicationFeature
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Key { get; set; }
        public string URL { get; set; }

        public bool IsRevoked { get; set; }
        public DateTimeOffset CreateDate { get; set; } 
        public DateTimeOffset RevokeDate { get; set; } 
    }
}
