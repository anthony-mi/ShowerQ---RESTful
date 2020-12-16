using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShowerQ.Models;
using ShowerQ.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ShowerQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IPhoneNumberFormatter _phoneNumberFormatter;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticationController(
            IConfiguration configuration,
            IPhoneNumberFormatter phoneNumberFormatter,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager
            )
        {
            _configuration = configuration;
            _phoneNumberFormatter = phoneNumberFormatter;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate(AuthenticationData authenticationData)
        {
            IActionResult response = Unauthorized();

            try
            {
                authenticationData.PhoneNumber = _phoneNumberFormatter.ConvertToInternationalFormat(authenticationData.PhoneNumber, _configuration["PhoneNumberRegion"]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            IdentityUser user = _dbContext.Users.FirstOrDefault(u => u.PhoneNumber.Equals(authenticationData.PhoneNumber));

            if(user is default(IdentityUser))
            {
                return response;
            }

            var signInResult = _signInManager.CheckPasswordSignInAsync(user, authenticationData.Password, false).Result;
            
            if(signInResult.Succeeded)
            {
                var claims = CreateClaims(user);

                var tokenString = BuildToken(claims);

                response = Ok(tokenString);
            }

            return response;
        }

        private IList<Claim> CreateClaims(IdentityUser user)
        {
            List<Claim> claims = new();

            foreach (var role in _userManager.GetRolesAsync(user).Result)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.Add(new Claim(ClaimTypes.UserData, user.Id));

            return claims;
        }

        private string BuildToken(IList<Claim> claims)
        {
            var keyStr = _configuration["Security:Key"];
            var keyBytesArr = Encoding.UTF8.GetBytes(keyStr);

            SymmetricSecurityKey key = new(keyBytesArr);
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _configuration["Security:JWT:Issuer"],
                audience: _configuration["Security:JWT:Audience"],
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
