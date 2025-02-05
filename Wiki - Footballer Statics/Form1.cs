using IniParser;
using IniParser.Model;
using Wiki___Footballer_Statics.Classes;
using Wiki___Footballer_Statics.Context;
using Wiki___Footballer_Statics.Services.Concrete;
using Microsoft.EntityFrameworkCore;
using Wiki___Footballer_Statics.ExternalClasses;
using System.Linq;
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
            string setting1,setting2,setting3 = "", setting4 = "", setting5="";
            if (!checkBox1.Checked)
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(filePath + "settings.ini");

                 setting1 = data["Settings"]["VAR"];
                 setting2 = data["Settings"]["Hafta"];
                 setting3 = data["Settings"]["Start"];
                 setting4 = data["Settings"]["Finish"];
                 setting5 = data["Settings"]["File"];
            }
            else
            {
               setting5 = textBox1.Text;


            }
            string[] strings = File.ReadAllLines(filePath + setting5);

            if (checkBox1.Checked)
            {
                setting3 = 0.ToString();
                setting4 = strings.Length.ToString();
            }
            var result = false;
            var cId = ((Competition)comboBox1.SelectedValue).Id;
            await Task.Run(async () =>
            {
                for (int i = Convert.ToInt32(setting3); i < Convert.ToInt32(setting4); i++)
                {
                    //label1.Text = i.ToString();
                    var mId = strings[i].Split(',')[1]; ;
                    var match = await MatchService.GetMatch(mId);

                    if (match != null)
                    {
                      

                        var match2 = new Classes.Match
                        {
                            AwayTeam = match.away,
                            HomeTeam = match.home,
                            HalfTime = match.d.ht,
                            MatchResult = match.d.s,
                            MId = Convert.ToInt32(mId),
                            CompetitionId = cId
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

                        var events = match.e.Select(x => new Classes.MatchEvent
                        {
                            EventDetail = x.EventDetail,
                            FirstActorPlayerId = x.FirstActorPlayerId,
                            Minute = x.Minute,
                            SecondActorPlayerId = x.SecondActorPlayerId,
                            Team = x.Team,

                        }).ToList();
                        result = await MatchService.AddMatchWithDetails(match2, homeLineUps, awayLineUps, events);
                    }
                }
            });
            MessageBox.Show(result.ToString());
        }
        private readonly AppDbContext _context = new AppDbContext();
        private async void button2_Click(object sender, EventArgs e)
        {
            var idOfPlayers = await _context.MatchLineUps.Include(y => y.Match).Where(z => z.Match.CompetitionId == 1).Select(x => x.PlayerId).Distinct().ToListAsync();

            var playerDetails = await WikiDataService.GetPlayerDetails(idOfPlayers.ToArray());

            var combinedPlayerDetails = new WikiDataResponse
            {
                head = playerDetails.First().head,
                results = new Results
                {
                    bindings = playerDetails.SelectMany(pd => pd.results.bindings).ToArray()
                }
            };
            // Combine the results from all chunks

            var teamsDemanded =await _context.Matches.Where(x=>x.CompetitionId==1).Select(x=>x.HomeTeam).Distinct().ToListAsync();


            var playerIdsInDetails = combinedPlayerDetails.results.bindings
                .Select(b => Convert.ToInt32(b.id.value))
                .ToHashSet();

            var missingPlayerIds = idOfPlayers
                .Where(playerId => !playerIdsInDetails.Contains(playerId))
                .ToList();

            missingPlayerIds.ForEach(x=>richTextBox2.Text += x.ToString() + "\n");
            var playedMatches = await _context.MatchLineUps
      .Include(y => y.Match)
      
      .ThenInclude(m => m.MatchEvents)
     .Include(y=>y.Match.Competition)
    .Where(x => teamsDemanded.Contains(x.Match.HomeTeam) || teamsDemanded.Contains(x.Match.AwayTeam))

      .GroupBy(x => new { x.PlayerId, x.Team,x.Match.CompetitionId })
        .ToListAsync();
            var playedMatches2 = playedMatches.Where(x=>teamsDemanded.Contains(x.Key.Team)).Select(y =>
            {
                return new
                {
                    Team = y.Key.Team,
                    PlayerId = y.Key.PlayerId,
                    Played = y.Count(x => x.IsFirstEleven || x.IsSubtituted),
                    FirstEleven = y.Count(x => x.IsFirstEleven),
                    Competition = y.Key.CompetitionId,
                    PlayedMinute = y.Sum(x => x.PlayedMinute),
                    Number = y.FirstOrDefault()?.Number,
                    PlayerName = "[[" + combinedPlayerDetails?.results?.bindings?.FirstOrDefault(x =>
                    x.id != null &&
                    Convert.ToInt32(x.id.value) == y.Key.PlayerId
                )?.formattedName?.value + "]]" ?? "-",
                    Nation = combinedPlayerDetails?.results?.bindings?.FirstOrDefault(x =>
                    x.id != null &&
                    Convert.ToInt32(x.id.value) == y.Key.PlayerId
                )?.nationLabel?.value ?? "-",
                    Position = combinedPlayerDetails?.results?.bindings?.FirstOrDefault(x =>
                    x.id != null &&
                    Convert.ToInt32(x.id.value) == y.Key.PlayerId
                )?.positionLabel?.value ?? "-",
                    Birth = $"{{{{Yaþ|{(combinedPlayerDetails?.results?.bindings?.FirstOrDefault(x =>
                    x.id != null &&
                    Convert.ToInt32(x.id.value) == y.Key.PlayerId
                )?.birthDate?.value.ToString("yyyy|MM|dd") ?? "-")}}}}}",
                    Goals = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        (z.EventDetail == Enums.EventDetail.Goal || z.EventDetail == Enums.EventDetail.Penalty) &&
                        z.FirstActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team)),
                    Penalties = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        z.EventDetail == Enums.EventDetail.Penalty &&
                        z.FirstActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team)),
                    YellowCards = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        z.EventDetail == Enums.EventDetail.YellowCard &&
                        z.FirstActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team)),
                    YellowAndRedCards = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        z.EventDetail == Enums.EventDetail.SecondYellowRedCard &&
                        z.FirstActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team)),
                    RedCards = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        z.EventDetail == Enums.EventDetail.DirectRedCard &&
                        z.FirstActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team)),
                    Assist = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        z.EventDetail == Enums.EventDetail.Goal &&
                        z.SecondActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team)),
                    MissedPenalties = y.Sum(x => x.Match.MatchEvents.Count(z =>
                        z.EventDetail == Enums.EventDetail.MissedPenalty &&
                        z.FirstActorPlayerId == x.PlayerId &&
                        z.Team == y.Key.Team))
                };
            })
       .OrderBy(x => x.Team)
       .ThenBy(x => x.Number)
       .ThenBy(y => y.PlayerId)
       .ToList();


            ;
            richTextBox1.Text += "{{#switch:{{{1}}}";
            playedMatches2.ForEach(async x =>
            {
                var competition = await _context.Competitions.FindAsync(x.Competition);
                richTextBox1.Text += $"|{x.Team}-{x.PlayerId}={{{{#switch:{{{{{{2}}}}}}\n" +
                $"|Id={x.PlayerId}\n" +
                $"|Takým={x.Team}\n" +
                $"|Lig=[[{ competition.FullSeasonName}|{competition.Name}]]\n" +
                $"|Ýsim={x.PlayerName}\n" +
                $"|Pozisyon={x.Position}\n" +
                $"|Milliyet={x.Nation}\n" +
                $"|Yaþ={x.Birth}\n" +
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
                $"|ÝlkOnbir={x.FirstEleven}\n" +
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

        private async void Form1_Load(object sender, EventArgs e)
        {
            var competitions =await  CompetitionService.GetCompetitions();
            comboBox1.DataSource = competitions;
            comboBox1.DisplayMember = "Name";
        }
    }
}
