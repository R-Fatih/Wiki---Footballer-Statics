using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.Classes;
using Wiki___Footballer_Statics.Context;
using Wiki___Footballer_Statics.ExternalClasses;
using static System.Net.WebRequestMethods;

namespace Wiki___Footballer_Statics.Services.Concrete
{
    public static class MatchService
    {
        public static async Task<ExternalClasses.Match> GetMatch(string id)
        {
            var client = new HttpClient();
            var response = await client.GetFromJsonAsync<ExternalClasses.Match>("https://arsiv.mackolik.com/Match/MatchData.aspx?t=dtl&id=" + id + "&s=0");
            return response;
        }
        private static readonly AppDbContext _context = new AppDbContext();
        public static async Task<bool> AddMatchWithDetails(Classes.Match match,List<MatchLineUp> homeLU, List<MatchLineUp> awayLU, List<Classes.MatchEvent> events)
        {
            if (_context.Matches.Any(x => x.MId == match.MId))
            {
                return false;
            }
            await _context.Database.BeginTransactionAsync();

            try
            {
                //first check this match saved before
                

                await _context.Matches.AddAsync(match);
                await _context.SaveChangesAsync();


               
                homeLU.ForEach(x => x.MatchId = match.Id);
                awayLU.ForEach(x => x.MatchId = match.Id);
                events.ForEach(x => {
                    x.MatchId = match.Id;
                    x.Team=x.Team=="1"?match.HomeTeam : match.AwayTeam;
                });

               
                await _context.MatchLineUps.AddRangeAsync(homeLU);
                await _context.MatchLineUps.AddRangeAsync(awayLU);
                await _context.MatchEvents.AddRangeAsync(events);
                await _context.SaveChangesAsync();


                await _context.Database.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                return false;
            }
        }
    }
}
