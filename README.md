# Uno Game


## Student Information

- **Name**: Vira Lavrova
- **Email**: vilavr@taltech.ee
- **Student Code**: 223682IVSB

## Introduction

Official rules used to create this game:
https://www.unorules.com/

## Features to Add

Functionality that I haven't time to implement yet, but will do in future

1. **Wild Shuffle Hands cards in deck**: you have to shuffle all your hand into the deck and take new cards from it (has to be updated in card settings and player action: execute move)
2. **Uno declaration**: typing in "uno" if you have 1 card left (if not done is punished by drawing 2 cards), prompt in player action: HumanTakeTurn method


## Known Bugs to Fix

Bugs I know exist and will fix, please if you notice any more bugs add them here!

1. **Displaying correct json files to load settings/game from**: all user saved json files are in one directory and all its content is displayed when the user wants to load game or use pre-saved settings. Obviously, these two kinds of files have different structure and aren't interchangeable. I'll add filtering of their structure and change the allowed lists of files to load game/settings from
2. **Handling first top card that has special effects**: Currently Reverse, Skip etc and Wild cards are treated as regular ones, so their effects aren't taken into accounts. I'll have to add these checks in game play (most probably in player action: take turn). Also, I'm not sure if Wild Draw Four card and other special cards are allowed to be the first ones (probably they should be reshuffled into the deck)


## Repository problems I don't know how to fix :(

1. **Gitignore**: in my local branch I have the full gitignore. However, it doesn't get pushed to git even when I change it. That's why the repo is flooded with unnecessary files
2. **Making separate class libraries**: when I tried to create new class libraries, the referencing didn't work (I tried both adding them through terminal, editing .csproj files and context actions). Now all my 15+ classes are in one library and I don't really know how to refactor that
3. **Json files paths**: I use them a lot and all of them are in Resources directory on solution level. However, if I reference them with solution level paths (/Uno/Resources/file.json) I get exception "file not found", because C# tries to search for this directory in /Bin/Debug directory. That's why I have all paths as absolute ones and the project won't work on any computer except mine :') I would love to get help with fixing this

