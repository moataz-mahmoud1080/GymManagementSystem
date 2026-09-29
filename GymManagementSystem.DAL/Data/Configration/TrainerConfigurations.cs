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
    internal class TrainerConfigurations :GymUserConfigurations<Trainer> ,
        IEntityTypeConfiguration<Trainer>
    {
        public new void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(p => p.CreatedAt)
                .HasColumnName("HireDate")
                .HasDefaultValueSql("getdate()");

            base.Configure(builder);
        }
    }
}
