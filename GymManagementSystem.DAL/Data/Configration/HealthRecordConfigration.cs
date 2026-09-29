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
    public class HealthRecordConfigration : IEntityTypeConfiguration<HealthRecord>
    {
        void IEntityTypeConfiguration<HealthRecord>.Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(p => p.BloodType)
                .HasMaxLength(5);

            builder.Property(p => p.Note)
                .HasMaxLength(500);

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("LastUpdate");

            builder.HasOne(h => h.Member)
                 .WithOne(m => m.HealthRecord)
                    .HasForeignKey<HealthRecord>(h => h.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
