using BELibrary.Core.Entity.Repositories;
using System;

namespace BELibrary.Core.Entity
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        ICategoryRepository Categories { get; }
        IDetailPrescriptionRepository DetailPrescriptions { get; }
        IItemRepository Items { get; }
        IDetailRecordRepository DetailRecords { get; }
        IMedicineRepository Medicines { get; }
        IPatientRepository Patients { get; }
        IMedicalSupplyRepository MedicalSupplies { get; }
        IPrescriptionRepository Prescriptions { get; }
        IRecordRepository Records { get; }
        IAttachmentAssignRepository AttachmentAssigns { get; }
        IAttachmentRepository Attachments { get; }
        IDoctorRepository Doctors { get; }
        IFacultyRepository Faculties { get; }
        IPatientRecordRepository PatientRecords { get; }
        IUserVerificationRepository UserVerifications { get; }
        IDoctorScheduleRepository DoctorSchedules { get; }
        IPatientDoctorRepository PatientDoctors { get; }
        IArticleRepository Articles { get; }

        int Complete();
    }
}