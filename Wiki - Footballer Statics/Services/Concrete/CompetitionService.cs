using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.Classes;
using Wiki___Footballer_Statics.Context;

namespace Wiki___Footballer_Statics.Services.Concrete
{
    public static class CompetitionService
    {
        private readonly static AppDbContext _context = new AppDbContext();
        public static async Task<List<Competition>> GetCompetitions()
        {
            return await  _context.Competitions.ToListAsync();
        }
    }
}
