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
}