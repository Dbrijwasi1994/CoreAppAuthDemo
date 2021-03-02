using CoreAppAuthDemo.Models;
using CoreAppAuthDemo.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Services
{
    public class ApplicationUserDetailsService : IApplicationUserDetailsRepo
    {
        private readonly ApplicationDataContext _context;
        private readonly ILogger<ApplicationUserDetailsService> _logger;

        public ApplicationUserDetailsService(ApplicationDataContext context, ILogger<ApplicationUserDetailsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string GetUserTwoFactorAuthMethod(string email)
        {
            try
            {
                //return _context.AspNetUsers.Where(x => x.Email.Equals(email)).Select(x => x.TwoFactorAuthMethod).SingleOrDefault();
                return "TOTP";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ex.Message;
            }
        }
    }
}
