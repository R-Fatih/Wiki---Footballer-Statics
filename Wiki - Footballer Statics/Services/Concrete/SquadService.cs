using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.ExternalClasses;

namespace Wiki___Footballer_Statics.Services.Concrete
{
   public static  class SquadService
    {
        public static bool GetPlayerExistStatus(List<Squad> squads,int playerId,string teamName)
        {


            foreach (var squad in squads)
            {
                if (squad.s.Any(x => x.s.Any(y => (y[1].ToString()) == playerId.ToString()&&squad.Team==teamName)))
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<Squad> GetSquad(string teamName, string seasonName = "2024/2025")
        {            
            var client = new HttpClient();

            var st = await StandingsService.GetStandings();
            var team = st.s.FirstOrDefault(t => t[1].ToString() == teamName);

            var response = await client.GetFromJsonAsync<Squad>($"https://arsiv.mackolik.com/Team/SquadData.aspx?id={team[0].ToString()}&season={seasonName}&sort=&dir=13");
            response.Team = teamName;
            return response;
        }
    }
}
