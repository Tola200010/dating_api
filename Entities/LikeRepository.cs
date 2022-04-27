using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    public class LikesRepository : ILikesRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public LikesRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<UserLike?> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _applicationDbContext.Likes!.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _applicationDbContext.Users!.OrderBy(x => x.Name).AsQueryable();
            var likes = _applicationDbContext.Likes!.AsQueryable();
            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(like => like.LikedUser!);
            }
            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser!);
            }
            var liksUsers = users.Select(user => new LikeDto
            {
                Name = user.Name,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos!.FirstOrDefault(p => p.IsMain)!.Url,
                City = user.City,
                Id = user.Id
            });
            return await PagedList<LikeDto>.CreateAsync(liksUsers, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<AppUser?> GetUserWithLikes(int userId)
        {
            return await _applicationDbContext.Users!.Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}