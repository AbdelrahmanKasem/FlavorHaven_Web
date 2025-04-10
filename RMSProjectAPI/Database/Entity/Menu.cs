﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Menu
    {
        [Key]
        public Guid Id { get; set; }
        public decimal? Offers { get; set; }

        public virtual List<Category> Categories { get; set; }
    }
}
