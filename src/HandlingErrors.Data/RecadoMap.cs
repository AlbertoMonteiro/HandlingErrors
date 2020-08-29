using FluentNHibernate.Mapping;
using HandlingErrors.Core.Modelos;

namespace HandlingErrors.Data
{
    public sealed class RecadoMap : ClassMap<Recado>
    {
        public RecadoMap()
        {
            Table("Recados");
            Id(r => r.Id).GeneratedBy.Identity();
            Map(r => r.Remetente).Length(50);
            Map(r => r.Destinatario).Length(50);
            Map(r => r.Assunto).Length(100);
            Map(r => r.Mensagem).Length(500);
            Map(r => r.AgrupadoComId).Nullable();
            Map(r => r.DataCriacao);
            HasMany(r => r.RecadosFilhos);
            References(r => r.AgrupadoCom).Column(nameof(Recado.AgrupadoComId));
        }
    }
}
