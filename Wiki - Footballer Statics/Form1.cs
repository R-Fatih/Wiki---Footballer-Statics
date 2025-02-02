using IniParser;
using IniParser.Model;
using Wiki___Footballer_Statics.Classes;
using Wiki___Footballer_Statics.Services.Concrete;

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
            string[] strings = File.ReadAllLines(filePath+ setting5);
            var result = false;
            await Task.Run(async () =>
            {
                for (int i = Convert.ToInt32(setting3); i < Convert.ToInt32(setting4); i++)
                {
                    label1.Text = i.ToString();
                    var mId = strings[i].Split(',')[1]; ;
                  var match=  await  MatchService.GetMatch(mId);
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
                        IsSubtituted = match.e.Where(y => y.FirstActorPlayerId == Convert.ToInt32(x[0].ToString())&&y.EventDetail==Enums.EventDetail.Substitution).Any()
                        ,SubstitutionMinute = match.e.Where(y => (y.FirstActorPlayerId == Convert.ToInt32(x[0].ToString())|| y.SecondActorPlayerId == Convert.ToInt32(x[0].ToString())) && y.EventDetail == Enums.EventDetail.Substitution).Select(y => y.Minute).FirstOrDefault()
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
                    result= await MatchService.AddMatchWithDetails(match2, homeLineUps, awayLineUps,events);

                }
            });
            MessageBox.Show(result.ToString());
        }
    }
}
