using MediatR;
using WebScraping.Core.Models.Request;
using WebScraping.Core.Models.Response;

namespace WebScraping.Core.Command
{
    public class WebScrapingCommand : IRequest<WebScrapingResponse>
    {
        public WebScrapingRequest Request;
        public WebScrapingCommand(WebScrapingRequest request)
        {
            Request = request;
        }
    }
}