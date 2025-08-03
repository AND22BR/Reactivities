using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using Application.Profiles.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles :Profile
    {
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>();
            CreateMap<CreateActivityDto, Activity>();
            CreateMap<EditActivityDto, Activity>();
            CreateMap<Domain.UserData, Domain.UserData>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(d => d.HostDisplayName, o => o.MapFrom(s =>
                    s.Attendees.FirstOrDefault(x => x.IsHost).UserData.DisplayName))
                .ForMember(d => d.HostId, o => o.MapFrom(s =>
                    s.Attendees.FirstOrDefault(x => x.IsHost)!.UserData.Id));
            CreateMap<ActivityAttendee, UserProfile>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.UserData.DisplayName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.UserData.Bio))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.UserData.ImageUrl))
                .ForMember(d=>d.Id, o=>o.MapFrom(s=>s.UserData.Id));
        }
    }
}