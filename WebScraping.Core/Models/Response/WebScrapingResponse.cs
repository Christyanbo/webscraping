using System.Collections;
using System.Collections.Generic;
using WebScraping.Core.Entities;

namespace WebScraping.Core.Models.Response
{
    public class WebScrapingResponse
    {
        public IEnumerable<FileInformation> FileInformation { get; set; }
    }
}