using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.ExternalClasses;

namespace Wiki___Footballer_Statics.Services.Concrete
{
   public static class StandingsService
    {

        public static async Task<Standing> GetStandings(int id= 67287)
        {
            var client = new HttpClient();
            var response = await client.GetFromJsonAsync<Standing>("https://arsiv.mackolik.com/AjaxHandlers/StandingHandler.ashx?op=standing&id=" + id);
            return response;
        }
    }
}
