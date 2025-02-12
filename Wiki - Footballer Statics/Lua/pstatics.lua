local p = {}

function p.teamPS(frame)
    local selectedTeam = frame.args[1]  
    local league = frame.args[2]

    local templateName = "Şablon:"..league.." futbolcu istatistikleri"  
    local templatePage = mw.title.new(templateName)
    local content = templatePage:getContent()
    
    local pattern = "|Id%s*=%s*(%d+)%s*\n" ..
                "|Takım%s*=%s*([^\n|]*)%s*\n" ..
                "|Lig%s*=%s*([^\n]*)\n"..
                "|İsim%s*=%s*([^\n]*)\n" ..
                "|Pozisyon%s*=%s*([^\n|]*)%s*\n" ..
                "|Milliyet%s*=%s*([^\n|]*)%s*\n" ..
                "|Yaş%s*=%s*([^\n]*)\n" ..
                "|Numara%s*=%s*(%d+)%s*\n" ..
                "|OynadığıMaç%s*=%s*(%d+)%s*\n" ..
                "|Goller%s*=%s*(%d+)%s*\n" ..
                "|Penaltılar%s*=%s*(%d+)%s*\n" ..
                "|KaçanPenaltılar%s*=%s*(%d+)%s*\n" ..
                "|Asistler%s*=%s*(%d+)%s*\n" ..
                "|SarıKartlar%s*=%s*(%d+)%s*\n" ..
                "|SarıKırmızıKartlar%s*=%s*(%d+)%s*\n" ..
                "|KırmızıKartlar%s*=%s*(%d+)%s*\n" ..
                "|Dakika%s*=%s*(%d+)%s*\n" ..
                "|İlkOnbir%s*=%s*(%d+)"

    local tableHeader = [[{| class='wikitable' style='text-align: center; font-size:80%;'
!rowspan='2'|No.!!rowspan='2'|Poz.!!rowspan='2'|Ülke!!rowspan='2'|Oyuncu
]]

    local columnHeader = '!width=50 style="border-right:3px solid grey;" colspan="8"|%s'
    local statsHeader = '![[File:Jersey white.svg|19px]]!!{{gol}}!!{{penaltı}}!!{{kaçan penaltı}}!!{{altın gol}}!!{{sarı kart}}!!{{kırmızı kart|1}}!!style="border-right:3px solid grey;"|{{kırmızı kart}}'


    local result = {tableHeader}
    local teamLeagues = {}
    local leaguesSeen = {}
    local playerStats = {}
    local sortedPlayers = {}
    
    -- First, collect all leagues for the team
    for id, team, league in mw.ustring.gmatch(content, "|Id%s*=%s*(%d+)%s*\n|Takım%s*=%s*([^\n|]*)%s*\n|Lig%s*=%s*([^\n]*)\n") do
        if team == selectedTeam and not leaguesSeen[league] then
            leaguesSeen[league] = true
            table.insert(teamLeagues, league)
        end
    end

    -- Collect player stats for each league
    for id, team, league, name, pos, nation, age, number, match, goals, penalties, mpenalties, assists, yellowcards, yellowredcards, redcards, minutes, firsteleven in mw.ustring.gmatch(content, pattern) do
        if team == selectedTeam then
            if not playerStats[name] then
                playerStats[name] = {
                    number = tonumber(number),
                    pos = pos,
                    nation = nation,
                    leagues = {},
                    totals = {
                        match = 0,
                        goals = 0,
                        penalties = 0,
                        mpenalties = 0,
                        assists = 0,
                        yellowcards = 0,
                        yellowredcards = 0,
                        redcards = 0
                    }
                }
                table.insert(sortedPlayers, name)
            end
            playerStats[name].leagues[league] = {
                match = tonumber(match),
                goals = tonumber(goals),
                penalties = tonumber(penalties),
                mpenalties = tonumber(mpenalties),
                assists = tonumber(assists),
                yellowcards = tonumber(yellowcards),
                yellowredcards = tonumber(yellowredcards),
                redcards = tonumber(redcards)
            }
            -- Update totals
            playerStats[name].totals.match = playerStats[name].totals.match + tonumber(match)
            playerStats[name].totals.goals = playerStats[name].totals.goals + tonumber(goals)
            playerStats[name].totals.penalties = playerStats[name].totals.penalties + tonumber(penalties)
            playerStats[name].totals.mpenalties = playerStats[name].totals.mpenalties + tonumber(mpenalties)
            playerStats[name].totals.assists = playerStats[name].totals.assists + tonumber(assists)
            playerStats[name].totals.yellowcards = playerStats[name].totals.yellowcards + tonumber(yellowcards)
            playerStats[name].totals.yellowredcards = playerStats[name].totals.yellowredcards + tonumber(yellowredcards)
            playerStats[name].totals.redcards = playerStats[name].totals.redcards + tonumber(redcards)
        end
    end

    -- Sort players by number
    table.sort(sortedPlayers, function(a, b)
        return playerStats[a].number < playerStats[b].number
    end)

    if #teamLeagues > 0 then
        -- Add headers for each league
        for _, league in ipairs(teamLeagues) do
            table.insert(result, string.format(columnHeader, league))
        end
        -- Add total stats header
        table.insert(result, string.format(columnHeader, "Toplam"))
        
        table.insert(result, "|- class='unsortable'\n")
        
        -- Add stats headers for each league and totals
        for i = 1, #teamLeagues + 1 do
            table.insert(result, statsHeader)
        end
        
        table.insert(result, "|-")
        
        -- Output player stats for each league and totals
        for _, name in ipairs(sortedPlayers) do
            local player = playerStats[name]
            local row = string.format("||%s||%s||{{bayraksimge|%s}}|| align=left|%s",
                player.number, player.pos, player.nation, name)
            
            -- Add stats for each league
            for _, league in ipairs(teamLeagues) do
                local leagueStats = player.leagues[league] or {
                    match = 0, goals = 0, penalties = 0, mpenalties = 0,
                    assists = 0, yellowcards = 0, yellowredcards = 0, redcards = 0
                }
                
                row = row .. string.format("||%s||%s||%s||%s||%s||%s||%s||style='border-right:3px solid grey;'|%s",
                    leagueStats.match, leagueStats.goals, leagueStats.penalties,
                    leagueStats.mpenalties, leagueStats.assists, leagueStats.yellowcards,
                    leagueStats.yellowredcards, leagueStats.redcards)
            end

            -- Add total stats
            row = row .. string.format("||%s||%s||%s||%s||%s||%s||%s||style='border-right:3px solid grey;'|%s",
                player.totals.match, player.totals.goals, player.totals.penalties,
                player.totals.mpenalties, player.totals.assists, player.totals.yellowcards,
                player.totals.yellowredcards, player.totals.redcards)
            
            table.insert(result, row .. "\n|-")
        end
    else
        return "Error: Selected team not found"
    end

    table.insert(result, "|}")
    return mw.text.trim(frame:preprocess(table.concat(result,"\n")))
end

return p
