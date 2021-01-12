using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShowerQ.Models.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models.Entities
{
    public class Reservation
    {
        private readonly ILazyLoader _lazyLoader;

        private IdentityUser _tenant;
        private Dormitory _dormitory;

        public Reservation()
        {
        }

        public Reservation(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public string TenantId { get; set; }

        public IdentityUser Tenant
        {
            get => _lazyLoader.Load(this, ref _tenant);
            set => _tenant = value;
        }

        public int DormitoryId { get; set; }

        public Dormitory Dormitory
        {
            get => _lazyLoader.Load(this, ref _dormitory);
            set => _dormitory = value;
        }

        public DateTime Beginning { get; set; }

        public DateTime Finishing { get; set; }

        public DateTime Date { get; set; }
    }
}
