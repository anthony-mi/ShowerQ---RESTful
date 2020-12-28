using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models.Entities.Validators
{
    public class ReservationRequestValidator : AbstractValidator<ReservationRequest>
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationRequestValidator(ApplicationDbContext context)
        {
            _dbContext = context;

            RuleFor(tenantsRequest => tenantsRequest.TenantId)
                .Must(TenantExists)
                .WithMessage("Tenant id is uncorrect.");

            RuleFor(tenantsRequest => tenantsRequest.IntervalId)
                .Must(IntervalExists)
                .WithMessage("Interval id is uncorrect.");

            RuleFor(tenantsRequest => tenantsRequest.Date)
                .Must(date => date > DateTime.Now)
                .WithMessage("Date must be greater than today.");

            /// TODO: check if there is an interval (id) in the current dormitories schedule.
        }

        private bool TenantExists(ReservationRequest request, string tenantId)
        {
            /// TODO: check if user is in `Tenant` role.
            return _dbContext.Users
                .FirstOrDefault(d => d.Id.Equals(tenantId)) is not default(IdentityUser);
        }

        private bool IntervalExists(ReservationRequest request, int intervalId)
        {
            return _dbContext.Intervals
                .FirstOrDefault(d => d.Id.Equals(intervalId)) is not default(Interval);
        }
    }
}
