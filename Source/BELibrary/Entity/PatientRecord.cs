using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("PatientRecord")]
    public class PatientRecord
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime TestDate { get; set; }
        public double BloodVessel { get; set; }
        public double BodyTemperature { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double Breathing { get; set; }
        public string VisionWithoutGlassesRight { get; set; }
        public string VisionWithoutGlassesLeft { get; set; }
        public string VisionWithGlassHoleLeft { get; set; }
        public string VisionWithGlassHoleRight { get; set; }
        public string VisionWithGlassLeft { get; set; }
        public string VisionWithGlassRight { get; set; }
        public string EyePressureRight { get; set; }
        public string EyePressureLeft { get; set; }
        public string ClinicalSymptoms { get; set; }
        public string DiagnosingTwoEyes { get; set; }
        public string DiagnosingLeftEyes { get; set; }
        public string DiagnosingRightEyes { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid RecordId { get; set; }
        public Guid PatientId { get; set; }
        public virtual Record Record { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
    }
}