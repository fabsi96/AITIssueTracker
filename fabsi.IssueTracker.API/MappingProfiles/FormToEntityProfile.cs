using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AutoMapper;

namespace AITIssueTracker.API.MappingProfiles
{
    public class FormToEntityProfile : Profile
    {
        public FormToEntityProfile()
        {
            CreateMap<UserForm, User>();
        }
    }
}