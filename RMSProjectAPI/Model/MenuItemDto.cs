﻿using RMSProjectAPI.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Model
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class UpdateMenuItemDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
    }
}