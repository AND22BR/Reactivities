using Application.Activities.Commands;
using Application.Activities.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        public ActivitiesController()
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Mediator.Send(new GetActivitiesList.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivityDetails(Guid id)
        {
            return await Mediator.Send(new GetActivityDetails.Query { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(Activity acitivity)
        {
            return await Mediator.Send(new CreateActivity.Command { Activity = acitivity });
        }

        [HttpPut]
        public async Task<ActionResult> EditActivity(Activity acitivity)
        {
            await Mediator.Send(new EditActivity.Command { Activity = acitivity });

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(Guid id)
        {
            await Mediator.Send(new DeleteAcitivity.Command { Id = id });
            
            return Ok();
        }
    }
}