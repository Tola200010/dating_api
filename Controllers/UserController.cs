using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        //private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserController(IMapper mapper, IPhotoService photoService,IUnitOfWork unitOfWork)
        {
            //   _applicationDbContext = applicationDbContext;
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembersDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            string? username = User.GetUsername();
            var gender = await _unitOfWork.UserRepository.GetuserGender(username!);
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username!);
            userParams.CurrentUsername = username;
            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = gender == "male" ? "female" : "male"; 
            }
            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users!.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage);
            return Ok(users);
        } 
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MembersDto>> GetUserByUsername(string username)
        {
            AppUser? appUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (appUser is null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<MembersDto>(appUser);
            return Ok(userDto);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //Get username form toekn authencation
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username!);
            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user!);
            var results = await _unitOfWork.Complete();
            return results ? NoContent() : BadRequest("Fail to update user");
        }
        [HttpPost("add-photo")]
        [Obsolete]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username!);
            var results = await _photoService.AddPhotoAsync(file);
            if (results.Error != null)
            {
                return BadRequest(results.Error.Message);
            }
            var photo = new Photo
            {
                Url = results.SecureUri!.AbsoluteUri,
                PublicId = results.PublicId,
            };
            if (user!.Photos!.Count == 0)
            {
                photo.IsMain = true;
            }
            user!.Photos.Add(photo);
            var rowAffected = await _unitOfWork.Complete();
            if (rowAffected)
            {
                // return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new { username = user.Name }, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem adding photo");
        }
        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username!);
            var photo = user!.Photos!.FirstOrDefault(x => x.Id == photoId);
            if (photo!.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user!.Photos!.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            _unitOfWork.UserRepository.Update(user);
            var rowAffected = await _unitOfWork.Complete();
            return rowAffected ? NoContent() : BadRequest("Fail to set main photo");
        }
        [HttpGet("get-member/{username}")]
        public async Task<ActionResult<MembersDto>> GetMembersByUsernameAsync(string username)
        {
            var memberDto = await _unitOfWork.UserRepository.GetMembersAsync(username);
            if (memberDto == null)
            {
                return NotFound();
            }
            return Ok(memberDto);
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username!);
            var photo = user!.Photos!.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var results = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (results.Error != null)
                {
                    return BadRequest(results.Error.Message);
                }
            }
            _unitOfWork.UserRepository.DeletePhotoAsync(photo);
            var rowAffected = await _unitOfWork.Complete();
            if (rowAffected)
            {
                return Ok();
            }
            return BadRequest("Failed to delete photo");
        }
    }
}