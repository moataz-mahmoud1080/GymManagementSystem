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
    public class GymUserConfigurations<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("VarChar")
                .HasMaxLength(50);

            builder.Property(p => p.Email)
               .HasColumnType("VarChar")
               .HasMaxLength(100);

            builder.HasIndex(p => p.Email)
                .IsUnique();


            builder.Property(p => p.Phone)
               .HasColumnType("VarChar")
               .HasMaxLength(11);

            builder.HasIndex(p => p.Phone)
                .IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_GymUser_Phone", "Phone LIKE '010%' OR Phone LIKE '011%' OR Phone LIKE '012%' OR Phone LIKE '015%'");

                tb.HasCheckConstraint("CK_GymUser_Email", "Email LIKE '_%@gmail.com' OR Email LIKE '_%@yahoo.com' OR Email LIKE '_%@outlook.com'");
            });

            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(a => a.Street)
                .HasColumnName("Street")
                .HasColumnType("VarChar")
                .HasMaxLength(30);


                a.Property(a => a.City)
                .HasColumnName("City")
                .HasColumnType("VarChar")
                .HasMaxLength(30);


            });

        }
    }
}
