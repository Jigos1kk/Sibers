using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key to the Employee entity.
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// Navigation property to the associated Employee.
        /// </summary>
        public Employe? Employe { get; set; }
    }
}