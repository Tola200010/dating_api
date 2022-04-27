using API.Interfaces;
using AutoMapper;

namespace API.Entities
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public UnitOfWork(ApplicationDbContext applicationDbContext,IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(_applicationDbContext,_mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_applicationDbContext,_mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_applicationDbContext);

        public async Task<bool> Complete()
        {
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _applicationDbContext.ChangeTracker.HasChanges();
        }
    }
}