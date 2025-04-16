using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence.Configurations
{
    public class DrillingFormConfiguration : IEntityTypeConfiguration<DrillingForm>
    {
        public void Configure(EntityTypeBuilder<DrillingForm> builder)
        {

            builder.HasKey(f => f.ProjectFormId);

            builder.Property(f => f.NumberOfWells).IsRequired();
            builder.Property(f => f.TotalMeters).IsRequired();

            builder.HasOne(f => f.ProjectForm)
                   .WithOne(p => p.DrillingForm)
                   .HasForeignKey<DrillingForm>(f => f.ProjectFormId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
