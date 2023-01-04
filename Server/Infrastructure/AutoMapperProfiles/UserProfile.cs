namespace Infrastructure.AutoMapperProfiles;

public class UserProfile : AutoMapper.Profile
{
	public UserProfile() : base()
	{
		CreateMap<User, UpdateUserByAdminRequestViewModel>().ReverseMap();

		CreateMap<UpdateUserRequestViewModel, User>().ReverseMap();
	}
}
