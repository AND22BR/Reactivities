using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.UserData.Commands
{
    public class CreateUserData
    {
        public class Command : IRequest<Result<string>>
        {
            public required Domain.UserData UserData { get; set; }

        }

        public class Handler(DataContext context, IMapper mapper) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userInfo=mapper.Map<Domain.UserData>(request.UserData);
                context.Users.Add(userInfo);   

                var result = await context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<string>.Failure("Failed to create user info",400);
                return Result<string>.Success(userInfo.Id.ToString());
            }
        }
        
    }
}