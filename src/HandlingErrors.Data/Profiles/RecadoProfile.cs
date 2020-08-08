using AutoMapper;
using HandlingErrors.Core.Modelos;
using HandlingErrors.Shared.ViewModels;

namespace HandlingErrors.Data.Profiles
{
    public sealed class RecadoProfile : Profile
    {
        public RecadoProfile() 
            => CreateMap<Recado, RecadoViewModel>()
            .ForMember(r => r.TotalFilhos, c => c.MapFrom(r => r.RecadosFilhos.Count));
    }
}
