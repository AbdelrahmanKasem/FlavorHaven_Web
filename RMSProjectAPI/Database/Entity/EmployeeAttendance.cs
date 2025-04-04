﻿using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class EmployeeAttendance
    {
        public Guid Id { get; set; }
        public DateTime CheckInDateTime { get; set; }
        public DateTime CheckOutDateTime { get; set; }
        public TimeSpan TotalWorkedHours => CheckOutDateTime - CheckInDateTime;
        public Guid EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public virtual User Employee { get; set; }
    }
}
