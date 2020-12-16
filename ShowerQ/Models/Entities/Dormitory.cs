using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShowerQ.Models.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class Dormitory
    {
        private readonly ILazyLoader _lazyLoader;

        private University _university;
        private ICollection<IdentityUser> _tenants;
        private ICollection<IdentityUser> _administrators;
        private Schedule _currentSchedule;

        public Dormitory()
        {
        }

        public Dormitory(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public string Address { get; set; }

        public int UniversityId { get; set; }

        public virtual University University
        {
            get => _lazyLoader.Load(this, ref _university);
            set => _university = value;
        }

        public virtual ICollection<IdentityUser> Tenants
        {
            get => _lazyLoader.Load(this, ref _tenants);
            set => _tenants = value;
        }

        public virtual ICollection<IdentityUser> Administrators
        {
            get => _lazyLoader.Load(this, ref _administrators);
            set => _administrators = value;
        }

        public int CurrentScheduleId {get; set;}

        public virtual Schedule CurrentSchedule
        {
            get => _lazyLoader.Load(this, ref _currentSchedule);
            set => _currentSchedule = value;
        }
    }
}
