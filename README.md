# TwitchWindowsNotifications
This Windows app notifies you when channels have specified keywords in their title or category

![Example](https://github.com/bggrund/TwitchWindowsNotifications/assets/64152538/6e4e8f8e-ed80-4ddc-9655-3e8c864d520a)

Instructions:
  1. Download TwitchNotifications.zip from releases
  2. Extract files
  3. Edit config.json with your desired channel information
  4. Run TwitchNotifications.exe or add it to Windows startup

config.json example:

[

{

"channelName":"BarbarousKing",

"titleContains":["marbles"],

"categoryContains":["marbles"]

},

{

"channelName":"ThaBeast721",

"titleContains":["zelda", "clayfighter", "asteroids", "yoshi"],

"categoryContains":[]

}

]
