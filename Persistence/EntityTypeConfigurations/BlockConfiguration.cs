using Domain.Blockchain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityTypeConfigurations
{
    public class BlockConfiguration : IEntityTypeConfiguration<Block>
    {
        public void Configure(EntityTypeBuilder<Block> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasAlternateKey(b => b.Hash);

            builder
                .HasIndex(b => b.Hash)
                .IsUnique();

            builder
                .Property(b => b.Hash)
                .IsRequired();

            builder
                .Property(b => b.PreviousHash)
                .IsRequired();

            builder
                .HasIndex(b => b.Timestamp);

            builder
                .OwnsMany(b => b.Transactions, builder => builder.ToJson());
        }
    }
}
