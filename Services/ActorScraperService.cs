using HtmlAgilityPack;
using SplititActorsApi.Data;
using SplititActorsApi.Models;

namespace SplititActorsApi.Services
{
    public class ActorScraperService
    {
        private readonly ApplicationDbContext _context;

        public ActorScraperService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ScrapeAndSaveActorsAsync()
        {
            var url = "https://www.imdb.com/list/ls054840033/";  // IMDb URL for the actor list
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            // Select relevant HTML nodes (adjust based on actual HTML structure)
            var actorNodes = doc.DocumentNode.SelectNodes("//div[@class='lister-item-content']");

            foreach (var node in actorNodes)
            {
                var name = node.SelectSingleNode(".//h3/a")?.InnerText;
                var rankText = node.SelectSingleNode(".//span[@class='lister-item-index']")?.InnerText;

                if (!string.IsNullOrEmpty(name) && int.TryParse(rankText?.Replace(".", "").Trim(), out int rank))
                {
                    var actor = new Actor
                    {
                        Name = name,
                        Rank = rank
                    };
                    _context.Actors.Add(actor);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
