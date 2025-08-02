using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
            return HandleResult(await Mediator.Send(new GetActivityDetails.Query { Id = id }));
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(CreateActivityDto acitivityDto)
        {
            return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = acitivityDto }));
        }

        [HttpPut]
        public async Task<ActionResult> EditActivity(EditActivityDto acitivity)
        {
            return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = acitivity }));
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new DeleteAcitivity.Command { Id = id }));
        }
    }
}