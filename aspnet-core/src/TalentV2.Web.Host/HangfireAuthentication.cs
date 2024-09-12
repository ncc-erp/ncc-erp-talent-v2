using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace TalentV2.Web.Host
{
	/// <summary>
	/// Represents the authentication logic for Hangfire Dashboard.
	/// </summary>
	public class HangfireAuthentication : IDashboardAuthorizationFilter
	{
		private readonly IConfiguration _configuration;
		private readonly ILoggerFactory _loggerFactory;
		/// <summary>
		/// Initializes a new instance of the <see cref="HangfireAuthentication"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="loggerFactory">The logger factory.</param>
		public HangfireAuthentication(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_configuration = configuration;
			_loggerFactory = loggerFactory;
		}
		/// <summary>
		/// Authorizes the dashboard context.
		/// </summary>
		/// <param name="context">The dashboard context.</param>
		/// <returns>True if the context is authorized, otherwise false.</returns>
		public bool Authorize([NotNull] DashboardContext context)
		{
			var httpContext = ((AspNetCoreDashboardContext)context).HttpContext;
			try
			{
				var data = httpContext.Request.Cookies["Abp.AuthToken"];
				if (!TryGetToken(httpContext, out var token)) return false;
				var principal = Decode(data);
				return principal != null && IsAdmin(principal);
			}
			catch (Exception ex)
			{
				_loggerFactory.CreateLogger<HangfireAuthentication>().LogError(ex, "Token validation failed");
				return false;
			}
		}
		private bool TryGetToken(HttpContext httpContext, out string token)
		{
			token = httpContext.Request.Cookies["Abp.AuthToken"];
			if (string.IsNullOrEmpty(token))
			{
				_loggerFactory.CreateLogger<HangfireAuthentication>().LogError("No token found in cookies.");
				return false;
			}
			return true;
		}

		private ClaimsPrincipal Decode(string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtSettings = _configuration.GetSection("Authentication:JwtBearer");
				var key = Encoding.ASCII.GetBytes(jwtSettings["SecurityKey"]);

				var validationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};

				return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
			}
			catch (Exception ex)
			{
				_loggerFactory.CreateLogger<HangfireAuthentication>().LogError(ex, "Token decoding failed.");
				return null;
			}
		}

		private static bool IsAdmin(ClaimsPrincipal principal)
		{
			var roles = principal.Claims
				.Where(c => c.Type == ClaimTypes.Role)
				.Select(c => c.Value)
				.ToList();

			return roles.Contains("Admin");
		}
	}
}
