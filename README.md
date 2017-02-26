# PSHandHistoryManager

## Current release
You can either clone the source and compile it yourself or you can download a precompiled version.  
Download version 1.0.0.0-alpha here:  [Download](http://www.husky110.de/uploads/PSHandHistoryManager.zip) -  [Virus Total Scan](https://virustotal.com/de/file/950be594fed4f6d2800cf3d390f52dfaa4a5490ed79897c2226addc9d4fec141/analysis/1488150795/)

### Current features:
For some detailed info, see section "What does it do?"
* Processes English handhistory files for one user
  * other languages are beeing passed through...
  * this tool was written for Holdem-hands - other playtimes should work as well, but I can't guarantee it
* Archive PSHandHistory-Data
* Store processed Files as backup
* Runs on multiple threads
* German and English UserInterface
* Built-in setuproutine
  * automaticlly configures PokerStars  

__BE CAREFUL! THIS TOOL IS CURRENTLY IN AN ALPHA STATE!__

### System Requirements  
This tool should run on any PC running Windows 7 or above.  
It is not tested on Linux or Mac!  
On my I7-6700K it processed about 2.100 hands within 2 minutes and used about 50 MB RAM.  

### Does this tool cost me anything?  
No it doesn't. :)  
But I would realy appreciate if you would donate something, since I've put time, brain and heart into this. :)  

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=P65FV828ZRBB2)

### How to setup?
Just download the release, unzip it somewhere and run the file "PSHandHistoryManager.exe".  
I've implemented a friendly Bot that tells you what do to. Just carefully follow the steps. :)  

### I've got an issue!
Just use the issue-tracker on github.  

### How can I contribute to the project?
Currently there is a need for translations of handhistory-processing to other languages.  
If you can contribute there by either coding yourself, or upload me some fully translated (your language -> english) files, you could help this project a lot! 
Right now we need every language - except English.

### I have thousands of hands in HM2 which should be processed by your tool...
Don't worry, I've got you covered! :)  
I've written a small tool which should work on any standard-installed HM2.  
Just run it and wait for it to finish.  
But you will have to delete those PokerStars-Hands out of your HM2 manually...
Download here: [Download](http://www.husky110.de/uploads/HM2HandExporter.zip)

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
After the setup it tells you the name of the fake-player, so it can be identified within the database of your HUD.  
This fake-player has a randomized name, so noone can make himself undetectable by using the fake-playername as accountname.

## Why is the program OpenSource and free?
It came to my mind to monetarize this tool, and since PokerStars is not willing to help processing their files, I will need the community.  
Besides that, it looks like cheating to me that some people have access to an advantage, which other people don't have.  
But I would realy appreciate if you would donate something. :) (see button on top)

## Which licence do you use for the program?  
I use the MIT-Licence.

##What's up next?
Right now I will wait for feedback and maybe some community help with the other languages.  
And the Codestyle is kinda... well... not there... ;)