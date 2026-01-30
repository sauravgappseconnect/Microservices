using CommandService.Data;
using CommandService.DTO;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly CommandServiceContext _commandServiceContext;

        public CommandController(CommandServiceContext commandServiceContext)
        {
            this._commandServiceContext = commandServiceContext;
        }

        [HttpGet(template: nameof(GetAllCommands))]
        public async Task<IActionResult> GetAllCommands(CancellationToken cancellationToken)
        {
            var commands = await _commandServiceContext.Commands
                .Include(nameof(Command.Platform))
                .Select(e => new CommandModel
                {
                    Id = e.Id,
                    HowTo = e.HowTo,
                    CommandLine = e.CommandLine,
                    PlatformName = e.Platform!.Name
                })
                .ToListAsync(cancellationToken);
            return Ok(commands);
        }

        [HttpGet(template: nameof(GetCommandById))]
        public async Task<IActionResult> GetCommandById(Guid id, CancellationToken cancellationToken)
        {
            var command = await _commandServiceContext.Commands
                .Include(nameof(Command.Platform))
                .Where(e => e.Id == id)
                .Select(e => new CommandModel
                {
                    Id = e.Id,
                    HowTo = e.HowTo,
                    CommandLine = e.CommandLine,
                    PlatformName = e.Platform!.Name
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (command == null)
            {
                return NotFound();
            }
            return Ok(command);
        }

        [HttpPost(template: nameof(CreateCommand))]
        public async Task<IActionResult> CreateCommand([FromBody] CommandCreateModel commandCreateModel, CancellationToken cancellationToken)
        {
            var platform = await _commandServiceContext.Platforms
                .FirstOrDefaultAsync(e => e.Id == commandCreateModel.PlatformId, cancellationToken);
            if (platform == null)
            {
                return BadRequest($"Platform with id '{commandCreateModel.PlatformId}' does not exist.");
            }
            var command = new Command
            {
                Id = Guid.NewGuid(),
                HowTo = commandCreateModel.HowTo,
                CommandLine = commandCreateModel.CommandLine,
                PlatformId = platform.Id
            };
            _commandServiceContext.Commands.Add(command);
            await _commandServiceContext.SaveChangesAsync(cancellationToken);
            var commandModel = new CommandModel
            {
                Id = command.Id,
                HowTo = command.HowTo,
                CommandLine = command.CommandLine,
                PlatformName = platform.Name
            };
            return Ok(commandModel);
        }

        [HttpDelete(template: nameof(DeleteCommand))]
        public async Task<IActionResult> DeleteCommand(Guid id, CancellationToken cancellationToken)
        {
            var command = await _commandServiceContext.Commands
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (command == null)
            {
                return NotFound();
            }
            _commandServiceContext.Commands.Remove(command);
            await _commandServiceContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

    }
}
