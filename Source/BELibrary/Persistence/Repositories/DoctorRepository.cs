using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}