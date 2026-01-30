using CommandService.Data;
using CommandService.DTO;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly CommandServiceContext _commandServiceContext;

        public PlatformController(CommandServiceContext commandServiceContext)
        {
            this._commandServiceContext = commandServiceContext;
        }

        [HttpGet(template:nameof(GetAllPlatforms))]
        public async Task<IActionResult> GetAllPlatforms(CancellationToken cancellationToken)
        {
            var platforms = await _commandServiceContext.Platforms.Select(e=> new PlatformModel { 
                Name = e.Name,
                Id = e.Id,
                Publisher = e.Publisher
            }).ToListAsync(cancellationToken);
            return Ok(platforms);
        }

        [HttpGet(template:nameof(GetPlatformAllCommands))]
        public async Task<IActionResult> GetPlatformAllCommands(Guid platformId, CancellationToken cancellationToken)
        {
            var platform = await _commandServiceContext.Platforms
                .Include(nameof(Platform.Commands))
                .Where(e => e.Id == platformId)
                .FirstOrDefaultAsync(cancellationToken);
            if (platform == null)
            {
                return NotFound();
            }
            var commands = platform.Commands!.Select(e => new CommandModel
            {
                Id = e.Id,
                HowTo = e.HowTo,
                CommandLine = e.CommandLine,
                PlatformName = platform.Name
            });
            return Ok(commands);
        }
    }
}
