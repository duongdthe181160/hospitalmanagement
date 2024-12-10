using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext
        {
            get { return Context as HospitalManagementDbContext; }
        }
    }
}