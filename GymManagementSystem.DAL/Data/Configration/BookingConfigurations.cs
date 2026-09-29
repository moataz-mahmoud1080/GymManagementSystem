using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configration
{
    internal class BookingConfigurations : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(p => p.Id);
            builder.HasKey(p => new {p.MemberId ,p.SessionId});

            builder.Property(p => p.CreatedAt)
                .HasColumnName("BookingAt")
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
