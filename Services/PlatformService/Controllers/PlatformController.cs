using Microservices.Common.Models;
using Microservices.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.DTO;
using System.Text.Json;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly PlatformServiceContext _platformServiceContext;
        private readonly IMessageSender _messageSender;

        public PlatformController(PlatformServiceContext platformServiceContext, IMessageSender messageSender)
        {
            this._platformServiceContext = platformServiceContext;
            this._messageSender = messageSender;
        }

        [HttpGet(template: nameof(GetAllPlatforms))]
        public async Task<IActionResult> GetAllPlatforms(CancellationToken cancellationToken)
        {
            var allPlatforms = await _platformServiceContext.Platforms.Select(e => new PlatformModel
            {
                Name = e.Name,
                Cost = e.Cost,
                Publisher = e.Publisher,
                Id = e.Id,
            }).ToListAsync();

            return Ok(allPlatforms);
        }

        [HttpGet(template: nameof(GetById))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var platform = await _platformServiceContext.Platforms
                .Where(e => e.Id == id)
                .Select(e => new PlatformModel
                {
                    Name = e.Name,
                    Cost = e.Cost,
                    Publisher = e.Publisher,
                    Id = e.Id,
                }).FirstOrDefaultAsync(cancellationToken);
            if (platform == null)
                return NotFound();
            return Ok(platform);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateModel platformCreateModel, CancellationToken cancellationToken)
        {
            var platformEntity = new Models.Platform
            {
                Name = platformCreateModel.Name,
                Cost = platformCreateModel.Cost,
                Publisher = platformCreateModel.Publisher,
            };
            //check platform with same name exists
            var result = await _platformServiceContext.Platforms
                .FirstOrDefaultAsync(e => e.Name == platformEntity.Name, cancellationToken);

            if (result != null)
                return Conflict($"Platform with name {platformEntity.Name} already exists.");

            await _platformServiceContext.Platforms.AddAsync(platformEntity, cancellationToken);
            await _platformServiceContext.SaveChangesAsync(cancellationToken);
            var platformModel = new PlatformModel
            {
                Id = platformEntity.Id,
                Name = platformEntity.Name,
                Cost = platformEntity.Cost,
                Publisher = platformEntity.Publisher,
            };
            await _messageSender.SendMessageAsync(JsonSerializer.Serialize(new PlatformMessageModel
            {
                Event = "Create",
                Id = platformModel.Id,
                Name = platformModel.Name,
                Publisher = platformModel.Publisher,
                UpdatedAt = platformEntity.UpdatedAt,
                CreatedAt = platformEntity.CreatedAt,
                CreatedBy = platformEntity.CreatedBy
            }));
            return Ok(platformModel);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePlatform([FromBody] PlatformModel platformModel, CancellationToken cancellationToken)
        {
            var platformEntity = await _platformServiceContext.Platforms
                .FirstOrDefaultAsync(e => e.Id == platformModel.Id, cancellationToken);
            if (platformEntity == null)
                return NotFound();
            platformEntity.Name = platformModel.Name;
            platformEntity.Cost = platformModel.Cost;
            platformEntity.Publisher = platformModel.Publisher;
            platformEntity.UpdatedAt = DateTime.UtcNow;
            _platformServiceContext.Platforms.Update(platformEntity);
            await _platformServiceContext.SaveChangesAsync(cancellationToken);
            await _messageSender.SendMessageAsync(JsonSerializer.Serialize(new PlatformMessageModel
            {
                Event = "Update",
                Id = platformModel.Id,
                Name = platformModel.Name,
                Publisher = platformModel.Publisher,
                UpdatedAt = platformEntity.UpdatedAt,
                CreatedAt = platformEntity.CreatedAt,
                CreatedBy = platformEntity.CreatedBy
            }));
            return NoContent();
        }
    }
}
