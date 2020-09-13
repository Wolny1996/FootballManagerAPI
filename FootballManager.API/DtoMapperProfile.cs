using AutoMapper;
using FootballManager.API.DTOs.Command;
using FootballManager.API.DTOs.Response;
using FootballManager.API.DTOs.Transfer;
using FootballManager.Data.Models;
using System;
using System.Linq;

namespace FootballManager.API
{
    public class DtoMapperProfile : Profile
    {
        public DtoMapperProfile()
        {
            CreateMap<ClubCommand, Club>()
                .ForMember(dest => dest.Founded, opt => opt.MapFrom(src => src.Founded.Date));
            CreateMap<CoachCommand, Coach>();
            CreateMap<FootballerCommand, Footballer>();
            CreateMap<StadiumCommand, Stadium>();
            CreateMap<TournamentCommand, Tournament>();

            CreateMap<Club, ClubResponse>()
                .ForMember(dest => dest.Founded, opt => opt.MapFrom(src => src.Founded.Date))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Today.Year - src.Founded.Year))
                .ForMember(dest => dest.Stadium, opt => opt.MapFrom(src => new StadiumDto
                {
                    StadiumName = $"{src.Stadium.StadiumName}",
                    Capacity = src.Stadium.Capacity
                }))
                .ForMember(dest => dest.Coach, opt => opt.MapFrom(src => new CoachDto
                {
                    FullName = $"{src.Coach.Name} {src.Coach.Surname}"
                }))
                .ForMember(dest => dest.Footballers, opt => opt.MapFrom(src => src.Footballers.Select(f => $"{f.Name} {f.Surname}")))
                .ForMember(dest => dest.Tournaments, opt => opt.MapFrom(src => src.ClubTournaments.Select(t => $"{t.Tournaments.TournamentName}")));

            CreateMap<Coach, CoachResponse>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
                .ForMember(dest => dest.CurrentClub, opt => opt.MapFrom(src => $"{src.Club.ClubName}"));

            CreateMap<Footballer, FootballerResponse>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
                .ForMember(dest => dest.CurrentClub, opt => opt.MapFrom(src => src.Club.ClubName));

            CreateMap<Stadium, StadiumResponse>()
                .ForMember(dest => dest.Club, opt => opt.MapFrom(src => src.Club.ClubName));

        }
    }
}