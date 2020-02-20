using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebScraping.Core.Command;
using WebScraping.Core.Models.Request;

namespace WebScraping.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebScrapingController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public WebScrapingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Example: //a[contains(@class, 'js-navigation-open')].
        /// </summary>
        /// <returns>Return files, files lines and file length</returns>
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WebScrapingRequest request)
        {
            var command = new WebScrapingCommand(request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}