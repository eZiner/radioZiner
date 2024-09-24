# radioZiner
Streaming audio & video recorder

![radioZiner-01](https://github.com/user-attachments/assets/0515a02e-0fe9-430b-8b56-72bc49409164)


# Installation

* [Download](https://github.com/eZiner/radioZiner/releases/) latest version (e.g. radioZiner-v#.#.#.zip)
* Create a folder "radioZiner" and unzip files into it
* Within the folder start radioZiner.exe
* You should get a message:
  - No streaming directory
  - Create new streaming directory in "\<Your-Videos-Folder\>\\radioZiner"?
* Click on "Yes"
* Select File/Home (Strg+H) from menu to open streaming directory in file explorer
* Close the application
* Go to the radioZiner application folder and select the files
  - backup_de1_2024-03-21.sql - Latest version can be [downloaded](https://backups.radio-browser.info/latest.sql.gz) from radio-browser.info
  - index-1.m3u - Latest version can be [downloaded](https://iptv-org.github.io/iptv/index.m3u) from iptv-org
* Move them to "\<Your-Videos-Folder\>\\radioZiner\\streams\\"
* Start radioZiner.exe again
* After a few seconds the text of the "Search" button at the upper left should change from grey to black
* radioZiner is ready to use


# First steps after installation

* Create new channel-group(s)
  - Select File/New (Strg+N) from menu
  - Enter name for channel group (e.g. "Radio") and commit with \<Enter\> or cancel with \<Esc\>
  - Repeat if you want (e.g. "TV", "Irish Folk", ...)
  - To change the name of a channel group select name and hit \<Enter\>, edit name and hit \<Enter\> again
  
* Find radio / tv station in database
  - Click into textbox right to the search button at the upper left
  - Enter search terms (e.g. "pub .ie" for Irish stations with "pub" in their name or key words)
  - Select a station 
  - Playback will start immediately
 
* Control playback
  - To pause / play click right mouse button or use pause button at the bottom left
  - Use sliders or mouse wheel to seek forward/backward
  - When paused the mouse wheel will step frame by frame (Jog/Shuttle)

* Drag & Drop or Copy & Paste
  - Drag or copy any valid stream or file url into right panel to play audio or video
  - Free Radio Stations: [fmstream.org](http://fmstream.org/) | [Icecast Directory](http://dir.xiph.org/) | [RadioBrowser](https://www.radio-browser.info/)
  - Free TV Stations: [iptv-org](https://iptv-org.github.io/) | [MediathekViewWeb (Germany)](https://mediathekviewweb.de/)

* Add new station to channel group
  - Choose appropriate channel group for new station (i.e. "Irish Folk")
  - Click into Channel Name / New Channel text at the top
  - Edit the name and commit with \<Enter\> or cancel with \<Esc\>
  - The channel name can be changed at any time in the same way
 
* Move / Delete channel
  - Choose channel and select Edit/Cut (Strg+X) from menu
  - To delete the channel skip next steps
  - To move the channel choose a channel group and select Edit/Paste (Strg+V) from menu
  
* Record stream
  - Click Rec button at the bottom middle / left
  - Recording will start immediately and the channel name is added as a button to the recording list
  - For read after write monitoring (listen / watch while recording) click on that button
  - If the channel was new it had been added automatically to the selected channel group
  - To stop recording click Stop button at the bottom middle / left

to be continued ...

---

Needed binaries:<br>
[ffmpeg.exe](https://www.ffmpeg.org/download.html) FFmpeg [project](https://www.ffmpeg.org/)<br>
[libmpv-2.dll](https://sourceforge.net/projects/mpv-player-windows/files/libmpv/) mpv-player [project](https://github.com/mpv-player/mpv)<br>

