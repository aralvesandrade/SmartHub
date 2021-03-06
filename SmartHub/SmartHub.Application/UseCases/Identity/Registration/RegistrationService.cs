﻿using Microsoft.AspNetCore.Identity;
using SmartHub.Application.Common.Exceptions;
using SmartHub.Application.Common.Interfaces;
using SmartHub.Domain.Entities.Users;
using SmartHub.Domain.Entities.ValueObjects;
using SmartHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SmartHub.Application.UseCases.Identity.Registration
{
	public class RegistrationService : IRegistrationService
	{
		private readonly UserManager<User> _userManager;
		private readonly ITokenGenerator _tokenGenerator;
		private readonly IUserService _userService;

		public RegistrationService(UserManager<User> userManager, ITokenGenerator tokenGenerator, IUserService userService)
		{
			_userManager = userManager;
			_tokenGenerator = tokenGenerator;
			_userService = userService;
		}

		public async Task<AuthResponseDto> RegisterAsync(RegistrationCommand userInput)
		{
			var userFound = await _userManager.FindByNameAsync(userInput.Username);
			if (userFound != null)
			{
				throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already exists" });
			}
			var user = new User(userInput.Username, null, new PersonName("", "", ""), null);

			var created = await _userService.CreateUser(user, userInput.Password, userInput.Role);
			if (created)
			{
				return new AuthResponseDto(_tokenGenerator.CreateJwtToken(user),
				user.UserName,
				new List<string> { userInput.Role },
				DateTime.Now.AddHours(JwtExpireTimeEnum.HoursToExpire.GetValue())
				);
			}
			throw new SmartHubException("Problem Registering new User");
		}
	}
}
