using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HotelBookingAPI.Models;
using HotelBookingAPI.DTOs;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfoDTO>>> GetUsers()
        {
            var users = _userManager.Users.Where(u => u.IsActive).ToList();
            var userDtos = new List<UserInfoDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserInfoDTO
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Position = user.Position ?? "",
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }

        // GET api/users/id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDTO>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return NotFound($"Пользователь с ID {id} не найден");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Position = user.Position ?? "",
                Roles = roles.ToList()
            };

            return Ok(userDto);
        }

        // DELETE api/users/id - удаление пользователя
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден");
            }

            await _userManager.DeleteAsync(user);
         
            return Ok(new { message = "Пользователь удален" });
        }

        // PUT api/users/id/roles
        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserRoles(string id, [FromBody] List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return NotFound($"Пользователь с ID {id} не найден");
            }

            // Получаем текущие роли пользователя
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Удаляем старые роли
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest(removeResult.Errors);
            }

            // Добавляем новые роли
            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
            {
                return BadRequest(addResult.Errors);
            }

            return Ok(new { message = "Роли пользователя успешно обновлены" });
        }
    }
}