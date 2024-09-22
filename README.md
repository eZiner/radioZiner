# radioZiner
Streaming audio & video recorder


Installation

* Goto https://github.com/eZiner/radioZiner/releases/ 
* Download latest version (e.g. radioZiner-v#.#.#.zip)
* Create a folder "radioZiner" and unzip files into it
* Within the folder start radioZiner.exe
* You should get a message:
  - No streaming directory
  - Create new streaming directory in "\<Your-Videos-Folder\>\\radioZiner"?
* Click on "Yes"
* Select File/Home (Strg+H) from menu to open streaming directory in file explorer
* Close the application
* Go to the radioZiner application folder and select the files
  - backup_de1_2024-03-21.sql
  - index-1.m3u
* Move them to "\<Your-Videos-Folder\>\\radioZiner\\streams\\"
* Start radioZiner.exe again
* After a few seconds the text of the "Search" button at the upper left should change from grey to black
* radioZiner is ready to use


First steps after installation

* Create new channel-group(s)
  - Select File/New (Strg+N) from menu
  - Enter name for channel group (e.g. "Radio") and commit with \<Enter\> or cancel with \<Esc\>
  - Repeat if you want (e.g. "TV", "Irish Folk", ...)
  - To change the title of a channel group select channel and hit \<Enter\>, edit title and hit \<Enter\> again
  
* Tune in radio/tv station
  - Click into textbox right to the search button at the upper left
  - Enter search terms (e.g. "pub .ie" for Irish stations with "pub" in their name)
  - Select a station 
  - Playback will start immediately
  - To pause/play click right mouse button or use pause button at the bottom left
  
* Record stream and automatically add new station to channel group
  - Choose appropriate channel group for new station (i.e. "Irish Folk")
  - Click Rec button at the bottom middle/left
  - Recording will start immediately and the channel name is added as a button to the recording list
  - For read after write monitoring (listen / watch while recording) click on that button
  - The new channel had been automatically added to the selected channel group

to be continued ...

---

Assets<br>
[ffmpeg.exe](https://www.ffmpeg.org/)<br>
[libmpv-2.dll](https://sourceforge.net/projects/mpv-player-windows/files/libmpv/)<br>
