using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChangeLog.Core.Services;
using ChangeLog.Core;

namespace ChangeLog.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class ChangelogController : ControllerBase
    {
        private readonly IChangelogService _changelogService;
        public ChangelogController(IChangelogService changelogService)
        {
            _changelogService = changelogService ?? throw new ArgumentNullException(nameof(changelogService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChangelogData>), StatusCodes.Status200OK)]
        public Task<IEnumerable<ChangelogData>> GetEntries()
        {
            return _changelogService.GetChangelogEntries();
        }

        [HttpGet("{version}")]
        [ProducesResponseType(typeof(ChangelogData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ChangelogData), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChangelogData>> GetRecord(string version)
        {
            var entry = await _changelogService.GetChangelog(version);
            if (entry == null)
            {
                return NotFound();
            }
            return entry;
        }

        [HttpPut("{version}")]
        [ProducesResponseType(typeof(ChangelogData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ChangelogData), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ChangelogData), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChangelogData>> UpdateRecord(string version, [FromBody] ChangelogData changedData)
        {
            if (string.IsNullOrWhiteSpace(version) || version != changedData?.BuildVersion)
            {
                return BadRequest();
            }
            var entry = await _changelogService.GetChangelog(changedData.BuildVersion);
            if (entry == null)
            {
                return NotFound();
            }
            return await _changelogService.UpdateChangelog(changedData);
        }

        [HttpDelete("{version}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecord(string version)
        {
            var task = await _changelogService.GetChangelog(version);
            if (task != null)
            {
                var result = await _changelogService.RemoveChangelog(task);
                if (result)
                {
                    return Accepted();
                }
                return BadRequest();
            }
            return NotFound();
        }
    }
}
