using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace ShowerQ.Models.Entities
{
    public class TenantsRequest
    {
        private readonly ILazyLoader _lazyLoader;

        private IdentityUser _tenant;
        private Interval _interval;

        public TenantsRequest()
        {
        }

        public TenantsRequest(ILazyLoader lazyLoader)
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

        public int IntervalId { get; set; }

        public Interval Interval
        {
            get => _lazyLoader.Load(this, ref _interval);
            set => _interval = value;
        }

        public DateTime Date { get; set; }
    }
}
