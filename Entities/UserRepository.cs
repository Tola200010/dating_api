using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
        }

        public async void CreateAsync(AppUser user)
        {
            await _applicationDbContext.Users!.AddAsync(user);
        }

        public async void DeleteAsync(int id)
        {
            var user = await _applicationDbContext.Users!.FirstOrDefaultAsync(x => x.Id == id);
            _applicationDbContext.Users!.Remove(user!);
        }

        public void DeletePhotoAsync(Photo photo)
        {
            _applicationDbContext.Photos!.Remove(photo);
        }

        public async Task<MembersDto?> GetMembersAsync(string username)
        {
            return await _applicationDbContext.Users!
            .Where(x => x.Name! == username)!.ProjectTo<MembersDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync()!;
        }

        public async Task<PagedList<MembersDto>?> GetMembersAsync(UserParams userParams)
        {
            var query = _applicationDbContext.Users!.AsQueryable();
            query = query.Where(x => x.Name != userParams.CurrentUsername);
            query = query.Where(x => x.Gender == userParams.Gender);
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x=>x.Created),
                _ => query.OrderByDescending(x=>x.LastActive)
            };
            return await PagedList<MembersDto>.CreateAsync(query.ProjectTo<MembersDto>(_mapper.ConfigurationProvider).AsNoTracking(),
            userParams.PageNumber,
            userParams.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await _applicationDbContext.Users!.Include(x => x.Photos).Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await _applicationDbContext.Users!.Include(x => x.Photos).Where(x => x.Name == username).FirstOrDefaultAsync();
        }

        public async Task<string?> GetuserGender(string username)
        {
            return await _applicationDbContext.Users!.Where(x=>x.UserName == username)
            .Select(x=>x.Gender).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var query = await _applicationDbContext.Users!.Include(x => x.Photos).ToListAsync();
            return query;
        }


        public void Update(AppUser user)
        {
            _applicationDbContext.Entry(user).State = EntityState.Modified;
        }
    }
}