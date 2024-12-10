using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class PatientRecordRepository : Repository<PatientRecord>, IPatientRecordRepository
    {
        public PatientRecordRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext => Context as HospitalManagementDbContext;
    }
}