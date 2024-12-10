using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public RecordRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext
        {
            get { return Context as HospitalManagementDbContext; }
        }
    }
}