using System.Collections.Generic;
using MediatR;
using WebScraping.Core.Models.Request;
using WebScraping.Core.Models.Response;

namespace WebScraping.Core.Command
{
    public class WebScrapingCommand : IRequest<IEnumerable<WebScrapingResponse>>
    {
        public WebScrapingRequest Request;
        public WebScrapingCommand(WebScrapingRequest request)
        {
            Request = request;
        }
    }
}