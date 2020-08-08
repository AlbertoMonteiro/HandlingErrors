using HandlingErrors.Core.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HandlingErrors.Data
{
    class RecadoEntityTypeConfiguration : IEntityTypeConfiguration<Recado>
    {
        public void Configure(EntityTypeBuilder<Recado> builder)
        {
            builder.ToTable("Recados");
            builder.HasKey(r => r.Id);
         
            builder.Property(r => r.Remetente).HasMaxLength(50);
            builder.Property(r => r.Destinatario).HasMaxLength(50);
            builder.Property(r => r.Assunto).HasMaxLength(100);
            builder.Property(r => r.Mensagem).HasMaxLength(500);

            builder.HasOne(r => r.AgrupadoCom)
                .WithMany(x => x.RecadosFilhos)
                .HasForeignKey(r => r.AgrupadoComId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
