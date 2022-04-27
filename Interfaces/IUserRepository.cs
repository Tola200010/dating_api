using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        void CreateAsync(AppUser user);
        void DeleteAsync(int id);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        void DeletePhotoAsync(Photo photo);
        Task<PagedList<MembersDto>?> GetMembersAsync(UserParams userParams);
        Task<MembersDto?> GetMembersAsync(string username);
        Task<string?>GetuserGender(string username);
    }
}