﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Database.Entity
{
    public class Location
    {
        [Key]
        public Guid Id { get; set; }
        public string UserLocation { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
