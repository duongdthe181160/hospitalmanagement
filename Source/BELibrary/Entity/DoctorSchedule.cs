using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("DoctorSchedule")]
    public class DoctorSchedule
    {
        public int Id { get; set; }

        public Guid DoctorId { get; set; }

        public Guid PatientId { get; set; }

        public DateTime ScheduleBook { get; set; }

        public int Status { get; set; }

        public virtual Doctor Doctor { get; set; }

        public virtual Patient Patient { get; set; }
    }
}