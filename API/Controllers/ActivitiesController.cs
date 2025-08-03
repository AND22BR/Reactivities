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
        public async Task<ActionResult<List<ActivityDto>>> GetActivities()
        {
            return await Mediator.Send(new GetActivitiesList.Query());
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ActivityDto>> GetActivityDetails(Guid id)
        {
            return HandleResult(await Mediator.Send(new GetActivityDetails.Query { Id = id }));
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(CreateActivityDto acitivityDto)
        {
            return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = acitivityDto }));
        }

        [HttpPut("{id}")]
        [Authorize(Policy ="IsActivityHost")]
        public async Task<ActionResult> EditActivity(string id, EditActivityDto acitivity)
        {
            acitivity.Id = Guid.Parse(id);
            return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = acitivity }));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy ="IsActivityHost")]
        public async Task<ActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new DeleteAcitivity.Command { Id = id }));
        }

        [HttpPost("{id}/attend")]
        public async Task<ActionResult> Attend(string id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }
    }
}