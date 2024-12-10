using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class DetailPrescriptionRepository : Repository<DetailPrescription>, IDetailPrescriptionRepository
    {
        public DetailPrescriptionRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext
        {
            get { return Context as HospitalManagementDbContext; }
        }
    }
}