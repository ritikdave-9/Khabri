using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Dtos;
using Common.Exceptions;
using Data.Entity;
using Microsoft.AspNetCore.Identity;
using Service.Interfaces;
using Common.Utils;
using Common.Enums;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IBaseService<User> _baseService;
        private readonly IMapper _mapper;

        public UserService(IBaseService<User> baseService, IMapper mapper)
        {
            _baseService = baseService;
            _mapper = mapper;
        }

        public async Task<User> SignUpAsync(UserSignupDto dto)
        {
            try
            {
                var existingUser = await _baseService.FindAsync(u => u.Email == dto.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("User with this email already exists.");
                }

                var user = _mapper.Map<User>(dto);
                user.UserID = Guid.NewGuid();
                user.Role = Role.User;
                user.Password = PasswordHasher.HashPassword(dto.Password);

                return await _baseService.AddAsync(user);
            }
            catch (RepositoryException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during signup.", ex);
            }
        }
      
    }
}
