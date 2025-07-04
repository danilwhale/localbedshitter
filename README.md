# localbedshitter

a bot for minecraft classic servers i made when bored

this is a project i was working on few months ago, most likely i won't continue developing this project anymore, so don't expect any bug fixes or things like that. if you want them, feel free to fork the project!

### features
- multibot support
- async job execution

### commands
- `^tp <username>`: teleports main bot to specified player
- `^tp <x> <y> <z>`: teleports main bot to specified coords
- `^say <content>`: says specified values from the face of main bot
- `^setblock <x> <y> <z> <id>`: sets block at specified coords
- jobs
  - `^fill <x0> <y0> <z0> <x1> <y1> <z1> <id>`: enqueues task to fill specified area with block
  - `^replace <x0> <y0> <z0> <x1> <y1> <z1> <oldid> <newid>`: enqueues task to replace old block with new one in specified area
  - `^dry <x0> <y0> <z0> <x1> <y1> <z1> <id>`: places sponges to remove water in specified area
  - `^eatchunks <count>`: enqueues task to eat specified count of random chunks (16xMAPHEIGHTx16)
  - `^sphere <x> <y> <z> <radius> <id>`: enqueues task to build a sphere with specified radius at coords with block
  - `^pyramid <x> <y> <z> <radius> <height> <id>`: enqueues task to build a pyramid with specified radius, height at coords with block
  - `^veryeasy <x> <y> <z>`: enqueues task to build an "its very easy" structure at specified coords (local joke)
- `^jobs`
  - `^jobs list`: lists pending jobs
  - `^jobs clear`: clears pending jobs, however, it doesn't remove executing job

## building
you need .net 9 sdk
```
dotnet build
```

### usage
as this was used on 2d2t classicube server, you need to register multiple classicube account that start the same and have number postfix.
in practice i used `subshitterN` accounts. number of sub-mppasses indicates number of accounts and indexing starts from 0, so if you have `shitfuck0` then you must insert its mppass first
```
dotnet run --project LocalBedShitter/LocalBedShitter.csproj --
--username <main_bot_username>
--sub-username <children_username_prefix>
--ip <server_ip>
--port <server_port>
--mpPass <main_bot_mppass>
--sub-mpPasses <children_mppasses>
```

example:
```
dotnet run --project LocalBedShitter/LocalBedShitter.csproj -- --username localbedshitter --sub-username subshitter --ip 127.0.0.1 --port 25565 --mpPass *********** --sub-mpPasses *********** *********** *********** ***********
```
command above will start the bot, main bot with username 'localbedshitter' will join first, then child bots with usernames 'subshitter0', 'subshitter1', 'subshitter2' and 'subshitter3' will join. this can take some time, as there has been added delay to make sure server doesn't kick out all of the bots due to rapid joining speed

