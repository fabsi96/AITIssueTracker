using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using AutoMapper;

namespace AITIssueTracker.API.MappingProfiles
{
    public class EntityToViewProfile : Profile
    {
        public EntityToViewProfile()
        {
            CreateMap<User, UserView>();
        }
    }
}