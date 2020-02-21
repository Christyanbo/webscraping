using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebScraping.Core.Entities;

namespace WebScraping.Core.Models.Response
{
    public class WebScrapingResponse
    {
        public string Extension { get; set; }
        public IEnumerable<FileInformation> FileInformation { get; set; }
    }
}