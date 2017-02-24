# PSHandHistoryManager

## What does it do?
### The Problem
PSHandHistoryManager is a free program to solve the following problem in HUDs:  
(This problem is persistent in HM2, other HUDs have to be tested!)  
Imagine you use a HUD to track players.  
Let's say you collect 200 hands on a player, where the player was sitting out or disconnected during 100 of them.  
The HUD handles those 100 hands as normal hands where the player just folded.  
Now let's say on the other 100 hands the player has a VPR of 20 and a PFR of 10.  
Your HUD would show you a VPR 10 / PFR 5, since it didn't recognize that the player wasn't there in those 100 afk-hands.  
With raising amount of data the values get more blurred.  
The HM2-Team seems to not think of that as a big issue: [Link to HM2-Forumthread](http://forums.holdemmanager.com/general-support/535817-sit-out-hands.html)  
Since I play on PokerStars - to me it is an issue which has to be solved.  
So I solved it myself.  
### The Solution
The PSHandHistoryManager scans your PokerStars-Handhistory-directory for any new hands.  
If it finds a hand, it checks it for sitting out and disconnected players and removes them from the hand.  
#### Example of a pristine hand (I have anonymized the hand):  
PokerStarsHeaderInformations here  
Seat 1: Player 1 (1815 in chips)  
Seat 2: Player 2 (1215 in chips)  
Seat 3: Player 3 (1470 in chips)  
Seat 4: Player 4 (1500 in chips)  
Seat 5: Player 5 (1500 in chips)  
Seat 6: Player 6 (1500 in chips)  
Seat 7: Player 7 (1500 in chips)  
Seat 8: Hero (1500 in chips)  
Seat 9: Player 8 (1500 in chips)  
Seat 10: Player 9 (1500 in chips)  
Player 3: posts small blind 15  
Player 4: posts big blind 30  
*** HOLE CARDS ***  
Dealt to Hero [As Qs]  
Player 5: folds  
Player 6: folds  
Player 7: folds  
Hero: raises 45 to 75  
Player 9 is disconnected  
Player 8: folds  
Player 9 has timed out while disconnected  
Player 9: folds  
Player 9 is sitting out  
Player 1: folds  
Player 2: calls 75  
Player 3: folds  
Player 4: folds  
*** FLOP *** [7s 3s Td]  
Hero: bets 98  
Player 2: folds  
Uncalled bet (98) returned to Hero  
Hero collected 195 from pot  
Hero: doesn't show hand  
*** SUMMARY ***  
Total pot 195 | Rake 0  
Board [7s 3s Td]  
Seat 1: Player 1 folded before Flop (didn't bet)  
Seat 2: Player 2 (button) folded on the Flop  
Seat 3: Player 3 (small blind) folded before Flop  
Seat 4: Player 4 (big blind) folded before Flop  
Seat 5: Player 5 folded before Flop (didn't bet)  
Seat 6: Player 6 folded before Flop (didn't bet)  
Seat 7: Player 7 folded before Flop (didn't bet)  
Seat 8: Hero collected (195)  
Seat 9: Player 8 folded before Flop (didn't bet)  
Seat 10: Player 9 folded before Flop (didn't bet)  
  
As you can clearly see, Player 9 has a disconnect and is sitting out.  
PSHandHistoryManager eliminates the player from the hand, so your HUD would see this:  
#### Example of a processed hand:  
PokerStarsHeaderInformations here  
Seat 1: Player 1 (1815 in chips)  
Seat 2: Player 2 (1215 in chips)  
Seat 3: Player 3 (1470 in chips)  
Seat 4: Player 4 (1500 in chips)  
Seat 5: Player 5 (1500 in chips)  
Seat 6: Player 6 (1500 in chips)  
Seat 7: Player 7 (1500 in chips)  
Seat 8: Hero (1500 in chips)  
Seat 9: Player 8 (1500 in chips)   
Player 3: posts small blind 15  
Player 4: posts big blind 30  
*** HOLE CARDS ***  
Dealt to Hero [As Qs]  
Player 5: folds  
Player 6: folds  
Player 7: folds  
Hero: raises 45 to 75  
Player 8: folds  
Player 1: folds  
Player 2: calls 75  
Player 3: folds  
Player 4: folds  
*** FLOP *** [7s 3s Td]  
Hero: bets 98  
Player 2: folds  
Uncalled bet (98) returned to Hero  
Hero collected 195 from pot  
Hero: doesn't show hand  
*** SUMMARY ***  
Total pot 195 | Rake 0  
Board [7s 3s Td]  
Seat 1: Player 1 folded before Flop (didn't bet)  
Seat 2: Player 2 (button) folded on the Flop  
Seat 3: Player 3 (small blind) folded before Flop  
Seat 4: Player 4 (big blind) folded before Flop  
Seat 5: Player 5 folded before Flop (didn't bet)  
Seat 6: Player 6 folded before Flop (didn't bet)  
Seat 7: Player 7 folded before Flop (didn't bet)  
Seat 8: Hero collected (195)  
Seat 9: Player 8 folded before Flop (didn't bet)  

As you can see, the player is eliminated from the hand, while keeping the data on the other players.

#### But what if the afk-player is involed in the flop/turn/river or the only one besides me in the hand?  
In that case PokerStarsHandHistoryManager uses a fake-player, which replaces the afk-player.  
With that way, you don't lose any data on the other players.  
During the setup it tells you the name of the fake-player, so it can be identified within the database of your HUD.  
This fake-player has a randomized name, so noone can make himself undetectable by using the fake-playername as accountname.

## Why is the program OpenSource and free?
It came to my mind to monetarize this tool, but if PokerStars is not willing to help me here (see "What is the roadmap?") I will need the community.  
Besides that, it looks like cheating to me that some people have access to an advantage, which other people don't have.  
And I want this tool to be evaluated and allowed by PokerStars.

## What works so far?
Nothing valuable.  
As today (23.Feb.2017) the following features are implemented:  

* a small User-Interface (right now just for debugging-purposes)
* a setup-routine (right now just for debugging-purposes, it's more like a draft)
* Scanning the PokerStars-HandHistory-Directory and serialize the hands for further processing (only english handhistoryfiles)
* most of the interface-routines are localized to english and german
* most of the current working routines are documented

## What is the roadmap?
Nextup is the handprocessing itself.  
Problem is that PokerStars saves the files in the language the PokerStars-Client runs on. This is not a problem for german or english, but I don't speak russian or chinese, or sth. like that.  
I've already asked PokerStars if they would provide me their localizationtables they use for their Handhistory-creation which would help me alot.  
After the processing itself, I will focus on the User-Interface.  

## When will you release the program?  
There is no specific date so far.  
It depends on weither PokerStars will help me (see "What is the roadmap?") and how fast I can comeup with a mostly-stable alpha.  
As soon as I have something to release I will upload the binaries.  

## Which licence do you use for the program?  
While it's in development, I use a non-specific licence which allows you to use it for personal purposes and for contribution.  
When the first releases comes up, I switch the licence for the MIT-Licence.
If you work for a company and you want to improve your product - feel free to use the code as inspiration. But please don't just copy it, even tho the licence allows it. :)