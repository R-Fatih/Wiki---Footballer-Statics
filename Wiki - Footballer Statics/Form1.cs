using IniParser;
using IniParser.Model;
using Wiki___Footballer_Statics.Classes;
using Wiki___Footballer_Statics.Context;
using Wiki___Footballer_Statics.Services.Concrete;
using Microsoft.EntityFrameworkCore;
namespace Wiki___Footballer_Statics
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const string filePath = "D:\\repos\\Wikipedia Maçkolik-TFF Match Scraper\\Wikipedia Maçkolik-TFF Match Scraper\\bin\\Debug\\";
        private async void button1_Click(object sender, EventArgs e)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(filePath + "settings.ini");

            string setting1 = data["Settings"]["VAR"];
            string setting2 = data["Settings"]["Hafta"];
            string setting3 = data["Settings"]["Start"];
            string setting4 = data["Settings"]["Finish"];
            string setting5 = data["Settings"]["File"];
            string[] strings = File.ReadAllLines(filePath + setting5);
            var result = false;
            await Task.Run(async () =>
            {
                for (int i = Convert.ToInt32(setting3); i < Convert.ToInt32(setting4); i++)
                {
                    label1.Text = i.ToString();
                    var mId = strings[i].Split(',')[1]; ;
                    var match = await MatchService.GetMatch(mId);
                    var match2 = new Match
                    {
                        AwayTeam = match.away,
                        HomeTeam = match.home,
                        HalfTime = match.d.ht,
                        MatchResult = match.d.s,
                        MId = Convert.ToInt32(mId),

                    };
                    var homeLineUps = match.h.Select((x, j) => new MatchLineUp
                    {
                        PlayerId = Convert.ToInt32(x[0].ToString()),
                        Team = match.home,
                        Number = Convert.ToInt32(x[2].ToString()),
                        IsFirstEleven = j < 11,
                        IsSubtituted = match.e.Where(y => y.FirstActorPlayerId == Convert.ToInt32(x[0].ToString()) && y.EventDetail == Enums.EventDetail.Substitution).Any()
                        ,
                        SubstitutionMinute = match.e.Where(y => (y.FirstActorPlayerId == Convert.ToInt32(x[0].ToString()) || y.SecondActorPlayerId == Convert.ToInt32(x[0].ToString())) && y.EventDetail == Enums.EventDetail.Substitution).Select(y => y.Minute).FirstOrDefault()
                    }).ToList();

                    var awayLineUps = match.a.Select((x, j) => new MatchLineUp
                    {
                        PlayerId = Convert.ToInt32(x[0].ToString()),
                        Team = match.away,
                        Number = Convert.ToInt32(x[2].ToString()),
                        IsFirstEleven = j < 11,
                        IsSubtituted = match.e.Where(y => (y.FirstActorPlayerId == Convert.ToInt32(x[0].ToString()) || y.SecondActorPlayerId == Convert.ToInt32(x[0].ToString())) && y.EventDetail == Enums.EventDetail.Substitution).Any()
                                                ,
                        SubstitutionMinute = match.e.Where(y => (y.FirstActorPlayerId == Convert.ToInt32(x[0].ToString()) || y.SecondActorPlayerId == Convert.ToInt32(x[0].ToString())) && y.EventDetail == Enums.EventDetail.Substitution).Select(y => y.Minute).FirstOrDefault()

                    }).ToList();

                    var events = match.e.Select(x => new MatchEvent
                    {
                        EventDetail = x.EventDetail,
                        FirstActorPlayerId = x.FirstActorPlayerId,
                        Minute = x.Minute,
                        SecondActorPlayerId = x.SecondActorPlayerId,
                        Team = x.Team,

                    }).ToList();
                    result = await MatchService.AddMatchWithDetails(match2, homeLineUps, awayLineUps, events);

                }
            });
            MessageBox.Show(result.ToString());
        }
        private readonly AppDbContext _context = new AppDbContext();
        private void button2_Click(object sender, EventArgs e)
        {
            var playedMatches = _context.MatchLineUps
      .Include(y => y.Match)
      .ThenInclude(m => m.MatchEvents)
      .Where(x => x.IsFirstEleven || x.IsSubtituted)
      .GroupBy(x => new { x.PlayerId, x.Team })
        .ToList()
      .Select(y => new
      {
          Team = y.Key.Team,
          PlayerId = y.Key.PlayerId,
          Played = y.Count(),
          FirstEleven = y.Count(x => x.IsFirstEleven),
          PlayedMinute = y.Sum(x => x.PlayedMinute),
          Number= y.FirstOrDefault().Number,

          Goals = y.Sum(x => x.Match.MatchEvents.Count(z => (z.EventDetail == Enums.EventDetail.Goal || z.EventDetail == Enums.EventDetail.Penalty) && z.FirstActorPlayerId == x.PlayerId && z.Team == y.Key.Team)),
          Penalties = y.Sum(x => x.Match.MatchEvents.Count(z => z.EventDetail == Enums.EventDetail.Penalty && z.FirstActorPlayerId == x.PlayerId && z.Team == y.Key.Team)),
          YellowCards = y.Sum(x => x.Match.MatchEvents.Count(z => z.EventDetail == Enums.EventDetail.YellowCard && z.FirstActorPlayerId == x.PlayerId && z.Team == y.Key.Team)),
          YellowAndRedCards = y.Sum(x => x.Match.MatchEvents.Count(z => z.EventDetail == Enums.EventDetail.SecondYellowRedCard && z.FirstActorPlayerId == x.PlayerId && z.Team == y.Key.Team)),
          RedCards = y.Sum(x => x.Match.MatchEvents.Count(z => z.EventDetail == Enums.EventDetail.DirectRedCard && z.FirstActorPlayerId == x.PlayerId && z.Team == y.Key.Team)),
          Assist = y.Sum(x => x.Match.MatchEvents.Count(z => z.EventDetail == Enums.EventDetail.Goal && z.SecondActorPlayerId == x.PlayerId && z.Team == y.Key.Team))
          ,MissedPenalties= y.Sum(x => x.Match.MatchEvents.Count(z => z.EventDetail == Enums.EventDetail.MissedPenalty && z.FirstActorPlayerId == x.PlayerId && z.Team == y.Key.Team))
      })
      .OrderBy(x => x.Team).ThenBy(x=>x.Number).ThenBy(y=>y.PlayerId)
      .ToList();

            richTextBox1.Text += "{{#switch:{{{1}}}";
            playedMatches.ForEach(x =>
            {
                richTextBox1.Text += $"|{x.Team}-{x.PlayerId}={{{{#switch:{{{{{{2}}}}}}\n" +
                $"|Id={x.PlayerId}\n" +
                $"|Takým={x.Team}\n" +
                $"|Numara={x.Number}\n" +
                $"|OynadýðýMaç={x.Played}\n" +
                $"|Goller={x.Goals}\n" +
                $"|Penaltýlar={x.Penalties}\n" +
                $"|KaçanPenaltýlar={x.MissedPenalties}\n" +
                $"|Asistler={x.Assist}\n" +
                $"|SarýKartlar={x.YellowCards}\n" +
                $"|SarýKýrmýzýKartlar={x.YellowAndRedCards}\n" +
                $"|KýrmýzýKartlar={x.RedCards}\n" +
                $"|Dakika={x.PlayedMinute}\n" +
                $"|ÝlkOnbir={x.FirstEleven}\n"+
                $"}}}}\n\n";
            });
            richTextBox1.Text += "}}\n";
            //richTextBox1.Text += "Team\tPlayerId\tPlayed\tFirstEleven\tPlayedMinute\tGoals\tPenalties\tYellowCards\tYellowAndRedCards\tRedCards\tAssist\nMissPenalties";

            //playedMatches.ForEach(x =>
            //{
            //    richTextBox1.Text += $"{x.Team}\t{x.PlayerId}\t{x.Played}\t{x.FirstEleven}\t{x.PlayedMinute}\t{x.Goals}\t{x.Penalties}\t{x.YellowCards}\t{x.YellowAndRedCards}" +
            //        $"\t{x.RedCards}\t{x.Assist}\t{x.MissedPenalties}\n";
            //});
        }
    }
}
