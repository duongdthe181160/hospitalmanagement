using AutoMapper;

namespace BELibrary.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMapFromEntitiesToViewModels();
            CreateMapFromViewModelsToEntites();
        }

        private void CreateMapFromViewModelsToEntites()
        {
        }

        private void CreateMapFromEntitiesToViewModels()
        {
        }
    }
}