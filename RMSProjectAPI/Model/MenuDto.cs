﻿using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Model
{
    public class MenuDto
    {
        public Guid Id { get; set; } // This can be generated in the backend
        public decimal? Offers { get; set; }
    }
}
