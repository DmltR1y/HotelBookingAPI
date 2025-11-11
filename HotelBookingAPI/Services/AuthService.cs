using Microsoft.AspNetCore.Identity;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Models;
using HotelBookingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<AuthResponseDTO> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDto);
        Task<UserInfoDTO?> GetUserInfoAsync(string userId);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var response = new AuthResponseDTO();

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
            {
                response.Success = false;
                response.Errors.Add("Неверный email или пароль");
                return response;
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                response.Success = false;
                response.Errors.Add("Неверный email или пароль");
                return response;
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var token = _tokenService.CreateToken(user, roles);

            response.Success = true;
            response.Message = "Успешный вход в систему";
            response.Token = token;
            response.Expiration = DateTime.Now.AddHours(8);
            response.User = new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Position = user.Position ?? "",
                Roles = roles
            };

            return response;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            var response = new AuthResponseDTO();

            // Валидация
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                response.Success = false;
                response.Errors.Add("Пароли не совпадают");
                return response;
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Errors.Add("Пользователь с таким email уже существует");
                return response;
            }

            // Проверяем допустимые роли
            var allowedPositions = new[] { "Receptionist", "Manager" };
            if (!allowedPositions.Contains(registerDto.Position))
            {
                response.Success = false;
                response.Errors.Add("Недопустимая должность (доступны Receptionist, Manager, Admin)");
                return response;
            }

            var nextId = await GetNextUserIdAsync();

            var user = new ApplicationUser
            {
                Id = nextId.ToString(),
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Position = registerDto.Position,
                HireDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                response.Success = false;
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            // Назначаем роль - должность
            var role = registerDto.Position; // Receptionist или Manager
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);

            // Получаем токен
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var token = _tokenService.CreateToken(user, roles);

            response.Success = true;
            response.Message = "Пользователь успешно зарегистрирован";
            response.Token = token;
            response.Expiration = DateTime.Now.AddHours(8);
            response.User = new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Position = user.Position ?? "",
                Roles = roles
            };

            return response;
        }

        private async Task<int> GetNextUserIdAsync()
        {
            return await _userManager.Users.CountAsync() + 1;
        }

        public async Task<AuthResponseDTO> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDto)
        {
            var response = new AuthResponseDTO();

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                response.Success = false;
                response.Errors.Add("Новые пароли не совпадают");
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Errors.Add("Пользователь не найден");
                return response;
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                response.Success = false;
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            response.Success = true;
            response.Message = "Пароль успешно изменен";
            return response;
        }

        public async Task<UserInfoDTO?> GetUserInfoAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Position = user.Position ?? "",
                Roles = roles.ToList()
            };
        }
    }
}
