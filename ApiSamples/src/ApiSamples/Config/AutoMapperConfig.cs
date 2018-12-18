using ApiSamples.ApiModels;
using ApiSamples.Domain;
using AutoMapper;

namespace ApiSamples.Config
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(
                opts =>
                {
                    opts.CreateMap<Candidate, CandidateApiModel>()
                        .ForAllMembers(x => x.ExplicitExpansion());

                    opts.CreateMap<CandidateApiModel, Candidate>()
                        .ForAllMembers(x => x.ExplicitExpansion());

                    opts.CreateMap<CreateCandidateApiModel, Candidate>();
                    opts.CreateMap<UpdateCandidateApiModel, Candidate>();

                    opts.CreateMap<Match, MatchApiModel>()
                        .ForAllMembers(x => x.ExplicitExpansion());
                    opts.CreateMap<Company, CompanyApiModel>()
                        .ForAllMembers(x => x.ExplicitExpansion());
                    opts.CreateMap<Position, PositionApiModel>()
                        .ForAllMembers(x => x.ExplicitExpansion());
                }
            );
        }
    }
}
