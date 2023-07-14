# TwitchWindowsNotifications
This Windows app notifies you when channels have specified keywords in their title or category

![Example](https://github.com/bggrund/TwitchWindowsNotifications/assets/64152538/6e4e8f8e-ed80-4ddc-9655-3e8c864d520a)

This app runs in the taskbar minimized, and polls the Twitch API every 20 seconds for updates on channel information, as specified in the config.json file. When you update this file, you have to restart the app for changes to take effect. Refer to the bottom of this README for an example.
When a poll matches something in the file, a Windows "Toast" notification will appear. Title and category checks are separate and don't need to both be matching for a notification to appear.
When checking for a match, the app removes spaces and converts everything to lowercase. So "This Is a tiTLE" will match "thisisatitle".

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
