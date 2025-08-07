using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Commands
{
    public class SetMainPhoto
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required string PhotoId { get; set; }
        }

        public class Handler(DataContext context,
        AuthDbContext authContext,
        IUserAccessor userAccessor,
        IPhotoService photoService)
        : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userAccessor.GetUserWithPhotosAsync();
                var userAuth = await authContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var photo = user.Photos.FirstOrDefault(x => x.Id == request.PhotoId);

                if (photo == null) return Result<Unit>.Failure("Cannot find photo", 400);

                user.ImageUrl = photo.Url;
                userAuth.ImageUrl = photo.Url;

                context.Users.Update(user);
                authContext.Users.Update(userAuth);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;
                var resultAuth=await authContext.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problem setting main photo", 400);
            }
        }
    }
}