using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wiki___Footballer_Statics.Enums;
using Wiki___Footballer_Statics.ExternalClasses;

namespace Wiki___Footballer_Statics.Converters
{
    public class EventConverter : JsonConverter<MatchEvent>
    {
        public override MatchEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {

            
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                var root = document.RootElement;
                var mE = new MatchEvent();

                var e = root.Deserialize<List<object>>();
                mE.Team = e[0].ToString();
                mE.Minute = Convert.ToInt32(e[1].ToString());
                mE.FirstActorPlayerId = Convert.ToInt32(e[2].ToString());
                JsonElement d = (JsonElement)e[5];

                switch (Convert.ToInt32(e[4].ToString()))
                {
                    case 1:
                        mE.EventDetail = EventDetail.Goal;

                        if (d.TryGetProperty("d", out JsonElement dElement))
                        {
                            var hasAssist = d.TryGetProperty("astId", out JsonElement element2);


                            switch (dElement.GetInt32())
                            {
                                case 1:
                                    if(hasAssist)
                                    mE.SecondActorPlayerId = element2.GetInt32();
                                    break;
                                case 2:
                                    mE.EventDetail = EventDetail.Penalty;
                                    break;
                                case 3:
                                    mE.EventDetail = EventDetail.OwnGoal;
                                    break;
                            }
                        }


                        break;
                    case 2:
                        mE.EventDetail = EventDetail.YellowCard;
                        break;
                    case 3:
                        
                            if (d.TryGetProperty("d", out JsonElement dElement2))
                            {
                                switch (dElement2.GetInt32())
                                {
                                    case 1:
                                        mE.EventDetail = EventDetail.SecondYellowRedCard;
                                        break;
                                    case 2:
                                        mE.EventDetail = EventDetail.DirectRedCard;
                                        break;
                                }
                            }
                        
                        break;
                    case 4:
                        mE.EventDetail = EventDetail.Substitution;
                        if (d.TryGetProperty("d", out JsonElement dElement3))
                        {
                            
                                mE.SecondActorPlayerId = dElement3.GetInt32();

                            
                        }
                        break;
                    case 7:
                        mE.EventDetail = EventDetail.MissedPenalty;
                        break;
                }

                return mE;
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public override void Write(Utf8JsonWriter writer, MatchEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
