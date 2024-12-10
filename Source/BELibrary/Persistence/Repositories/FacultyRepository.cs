using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        public FacultyRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}