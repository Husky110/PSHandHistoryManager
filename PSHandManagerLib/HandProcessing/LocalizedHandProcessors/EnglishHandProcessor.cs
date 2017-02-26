using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSHandManagerLib.Tasks;

namespace PSHandManagerLib.HandProcessing.LocalizedHandProcessors
{
    class EnglishHandProcessor : HandProcessor
    {
        public EnglishHandProcessor(HandTask ht) : base(ht){}


        protected override Dictionary<String, int> detectPlayers()
        {
            // detect players and check if any is already marked as sitting out in the beginning...
            int holeCardsIndicatorLineNum = 100;
            Dictionary<String, int> players = new Dictionary<string, int>(); //playername -> eleminiate before flop? (1 -> just eliminate, 2 -> replace with fakeplayer, 0 do nothing)
            for (int x = 2; x < holeCardsIndicatorLineNum; x++)
            {
                string line = this.handTask.lines[x];
                if (line.StartsWith(this.languageDictionary["seat"]))
                {
                    bool playerIsSittingOut = false;
                    if (line.EndsWith(this.languageDictionary["afk"]))
                    {
                        playerIsSittingOut = true;
                    }
                    int bracketIndex = line.LastIndexOf('('); // the first brackets could be part of the player name, so the last index must be the one before the chipstack
                    int colonIndex = line.IndexOf(':'); // the first colon is the one in "Seat X:";
                    string playername = line.Substring(colonIndex + 1, bracketIndex - colonIndex - 1).Trim(' ');
                    players.Add(playername, Convert.ToInt16(playerIsSittingOut)); // do mode 1 -> eliminate or do nothing -> player is here
                }
                if (line == this.languageDictionary["holeCardIndicatorLine"])
                {
                    holeCardsIndicatorLineNum = x;
                }
            }

            //now we check for players that timeout after the cards are beeing dealt... any none-timeout player has been marked to be eliminated before
            int flopIndicatorLineNum = 100;
            List<String> playersWithTimeout = new List<string>(); //needed so we realy process players which timed out after flop
            for (int x = holeCardsIndicatorLineNum; x < this.handTask.lines.Length; x++)
            {
                string line = this.handTask.lines[x];
                if (line.StartsWith(this.languageDictionary["flopIndicatorLine"]))
                {
                    flopIndicatorLineNum = x;
                    continue;
                }
                if (line.EndsWith(this.languageDictionary["timeoutLine"]))
                {
                    int timeoutIndex = line.LastIndexOf(this.languageDictionary["timeoutLine"]);
                    string player = line.Substring(0, timeoutIndex);
                    playersWithTimeout.Add(player);
                }
                if (line.EndsWith(this.languageDictionary["afk"]))
                {
                    int afkIndex = line.LastIndexOf(this.languageDictionary["afk"]);
                    string player = line.Substring(0, afkIndex - 1);
                    if (x < flopIndicatorLineNum)
                    {
                        //case: The player folds his hand preflop and clicks on "sitout" aftwards -> in this case we have to do nothing, so we have to check for that
                        if (playersWithTimeout.Contains(player))
                        { 
                            //The player has been timed-out preflop -> eliminate him from the hand
                            players[player] = 1;
                        }
                    }
                    else
                    {
                        //case: The player plays normal and clicks on "sitout next hand" which would him be replaced by a fake-player (which would be wrong)
                        if (playersWithTimeout.Contains(player)) 
                        {
                            //The player has been timed-out post-flop -> replace him with a fake-player
                            players[player] = 2;
                        }

                    }

                }
            }
            List<String> unusedPlayers = new List<string>();
            foreach(KeyValuePair<String, int> kvp in players)
            {
                if(kvp.Value == 0)
                {
                    unusedPlayers.Add(kvp.Key);
                }
            }

            foreach(string unused in unusedPlayers)
            {
                players.Remove(unused);
            }

            return players;
        }
    }
}
