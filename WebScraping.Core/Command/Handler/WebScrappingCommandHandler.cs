using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediatR;
using Microsoft.VisualBasic;
using WebScraping.Core.Entities;
using WebScraping.Core.Interface.Repository;
using WebScraping.Core.Models.Response;

namespace WebScraping.Core.Command.Handler
{
    public class WebScrappingCommandHandler : IRequestHandler<WebScrapingCommand, IEnumerable<WebScrapingResponse>>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IFileInformationRepository _fileInformationrepository;

        public WebScrappingCommandHandler(IHttpClientFactory clientFactory,
            IFileInformationRepository fileInformationrepository)
        {
            _clientFactory = clientFactory;
            _fileInformationrepository = fileInformationrepository;
        }

        public async Task<IEnumerable<WebScrapingResponse>> Handle(WebScrapingCommand request, CancellationToken cancellationToken)
        {
            var url = "https://github.com/gregoryyoung/m-r";
            var node = "//a[contains(@class, 'js-navigation-open')]";

            var listFiles = await ReturnFileUrl(url, node);

            var listFileInformation = new List<FileInformation>();

            foreach (var itemFile in listFiles)
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetStringAsync(itemFile);

                var html = new HtmlDocument();
                html.LoadHtml(response);

                var itemNode = html.DocumentNode;

                var nameFile = itemNode.SelectNodes(
                        "//strong[contains(@class, 'final-path')]")
                    .Select(p => p.InnerText).FirstOrDefault();

                var htmlInformation = itemNode.SelectNodes(
                        "//div[contains(@class, 'text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0')]")
                    .Select(p => p.InnerText).FirstOrDefault();

                string lineCount = "0 lines";
                string fileLenth = "0 bytes";

                if (htmlInformation != null && htmlInformation.Contains("line"))
                {
                    var splitInformation = htmlInformation.Split(" ");
                    lineCount = splitInformation[6] + " " + splitInformation[7];
                    fileLenth = splitInformation[19] + " " + splitInformation[20].Replace("\n", "");
                }
                else
                {
                    var splitInformation = htmlInformation?.Split(" ");
                    lineCount = "0 lines";
                    if (splitInformation != null)
                        fileLenth = splitInformation[4] + " " + splitInformation[5].Replace("\n", "");
                }

                var fileItem = new FileInformation()
                {
                    Name = nameFile,
                    Length = fileLenth,
                    Lines = lineCount,
                    Extension = nameFile?.Substring(nameFile.LastIndexOf('.') + 1,
                        nameFile.Length - nameFile.LastIndexOf('.') - 1)
                };

                //Ao configurar um banco MySql, descomentar para salvar
                //await _fileInformationrepository.Add(fileItem);
                listFileInformation.Add(fileItem);
            }
            
            var result = listFileInformation.GroupBy(g => g.Extension,
                (key, g) => new WebScrapingResponse(){ Extension = key, FileInformation = g.ToList() });
            
            return result;
        }

        public async Task<List<string>> ReturnFileUrl(string url, string node)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync(url);

            var html = new HtmlDocument();
            html.LoadHtml(response);

            var listNodes = html.DocumentNode.SelectNodes(node);
            var listFiles = new List<string>();

            foreach (var itemNode in listNodes)
            {
                if (!itemNode.HasAttributes || string.IsNullOrEmpty(itemNode.Attributes["href"].Value) ||
                    itemNode.InnerHtml == "..") continue;
                
                var link = itemNode.Attributes["href"].Value;

                if (link.Contains("."))
                {
                    listFiles.Add("https://github.com" + link);
                }
                else
                {
                    var urlDirectory = "https://github.com" + link;
                    listFiles.AddRange(await ReturnFileUrl(urlDirectory, node));
                }
            }

            return listFiles;
        }
    }
}