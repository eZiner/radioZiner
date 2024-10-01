# radioZiner
Streaming audio & video recorder

![radioZiner-01](https://github.com/user-attachments/assets/0515a02e-0fe9-430b-8b56-72bc49409164)


# Installation

* [Download](https://github.com/eZiner/radioZiner/releases/) latest version (e.g. radioZiner-v#.#.#.zip).
* Create a folder "radioZiner" and unzip files into it.
* Within the folder start radioZiner.exe.
* You should get a message like:

```
No streaming directory
Create new streaming directory in "C:\Users\<username>\Videos\radioZiner"?
<Yes>  <No>
```

* Click on "Yes" or see below how to create a custom streaming directory.

> #### How to create and use a custom streaming directory
>* Create a new folder (i.e. *myStreamingDirectory*).
>* Create a shortcut of radioZiner.exe (i.e. *myRadioZiner.exe*).
>* Open the properties of *myRadioZiner.exe*.
>* Append a \<Space\> + "mainDir=*PathToYourStreamingDirectory*" to the target path.
>* Start *myRadioZiner.exe*.

* Select File/Home (Ctrl+H) from menu to open streaming directory in file explorer.
* Close the application.
* Go to the radioZiner application folder and select the files:
    * **backup_de1_2024-03-21.sql** - Latest version can be [downloaded](https://backups.radio-browser.info/latest.sql.gz) from radio-browser.info.
    * **index-1.m3u** - Latest version can be [downloaded](https://iptv-org.github.io/iptv/index.m3u) from iptv-org.
* Move them to \<*PathToYourStreamingDirectory*\>\\streams.
* To update your database replace the files with the latest versions.
* Start radioZiner.exe again.
* After a few seconds the text of the "Search" button at the upper left should change from grey to black.
* radioZiner is ready to use.

# First steps after installation

* Create new channel group(s)
    * Select File/New (Ctrl+N) from menu.
    * Enter name for channel group (e.g. "Radio") and commit with \<Enter\> or cancel with \<Esc\>.
    * Repeat if you want (e.g. "TV", "Irish Folk", ...).
    * To change the name of a channel group select name twice and hit \<Enter\>, edit name and hit \<Enter\> again.
  
* Find radio/tv station in database
    * Click into textbox right to the search button at the upper left.
    * Enter search terms (e.g. "pub .ie" for Irish stations with "pub" in their name or key words).
    * Select a station .
    * Playback will start immediately.
 
* Control playback
    * To pause/play click right mouse button or use pause button at the bottom left.
    * Use sliders or mouse wheel to seek forward/backward.
    * The use of the mouse wheel is somewhat unusual: with the mouse pointer on the left-hand side of the panel, the wheel changes the font size, in the middle it seeks forwards/backwards and on the right-hand side it scrolls the content up and down.
    * When paused the mouse wheel will step frame by frame (Jog/Shuttle).
    * To show/hide the station list click on the ▷ Left Panel Toggle ▼.
    * To show/hide the play list when watching a video click on the ◁ Right Panel Toggle ▼.
    * To show/hide all controls press \<F12\>.
    * To toggle fullscreen press \<F11\>.

* Play any audio/video source per drag/drop or copy/paste
    * Drag or paste any valid stream or file url into right panel to play audio or video.
    * Free Radio Stations: [fmstream.org](http://fmstream.org/) | [Icecast Directory](http://dir.xiph.org/) | [RadioBrowser](https://www.radio-browser.info/)
    * Free TV Stations: [iptv-org](https://iptv-org.github.io/) | [MediathekViewWeb (Germany)](https://mediathekviewweb.de/)

* Add new station/file/folder to channel group.
    * While playing a new station/file or after dragging/pasting a folder ...
    * Choose appropriate channel group for new station/file/folder (i.e. "Irish Folk").
    * Click into Channel Name ("New Channel" if it is new) at the top.
    * Edit the name and commit with \<Enter\> or cancel with \<Esc\>.
    * The channel name can be changed at any time in the same way. After the change, a copy with the old name of the channel remains. Delete the old copy or use it as an alias.
    * Local files and folders that have been added as channels can be selected by clicking on the "Files" button at the top. Folders are indicated by square brackets \[\] around the name. If you click on a folder, all playable audios/videos in this folder and its subfolders are listed in descending order by date. Click on the "Files" button at the top to bring up/refresh the list of files and folders again.

* Move/Delete channel
    * Choose channel and select Edit/Cut (Ctrl+X) from menu.
    * To delete the channel skip next steps.
    * To move the channel choose a channel group and select Edit/Paste (Ctrl+V) from menu.
  
* Record stream
    * Click Rec button at the bottom middle/left.
    * Recording will start immediately and the channel name is added as a button to the recording list.
    * For read after write monitoring (listen/watch while recording) click on that button
    * If the channel was new it had been added automatically to the selected channel group.
    * To stop recording click Stop button at the bottom middle/left
    * Some video channels, such as ARTE Germany, cannot be recorded correctly due to stream errors that cause problems for mpv recording. In these cases, you can try to force the use of ffmpeg instead of mpv by appending an @ to the channel name.
    * All recordings are stored in your streaming directory together with a textfile that holds the playlist.
    * To listen/watch the audio/video files click on the "Recordings" button at the top. Click again to refresh the list.


# Export music titles to audio files

To save individual titles from the recorded radio broadcasts as an audio file, first click on the "Recordings" button at the top left and select the desired recording. Then click on the "Export" button at the bottom to show the controls needed for setting start/end time, the filename and export format.

Locate the start of the title you want to export. If the radio station the audio file was recorded from supports icecast you already have a playlist. Just click on the title and you are almost done. Often the displayed start time does not exactly correspond with the title start, in which case some fine tuning is still to be done.

* To set the export name and start time hold the \<Ctrl\> key and select the title from the playlist.
* Enter/edit the export name and set the two timestamps on the left-hand side of the input field.
* To set the start time hold the \<Ctrl\> key and click on the left timestamp.
* To set the end time hold the \<Ctrl\> key and click on the right timestamp.
* To verify the position click on the start/end timestamp while playing.
* Select .mp3 (in most cases) or .aac depending on the stream format.
* Finally click on the green "Export" button.

The exported audio file will be stored in your streaming directory. Click on the "Exports" button at the top to get a list of your exports. Click again to refresh the list.

# Cut & export to video files

Exporting movies, scenes or sequences works in pretty much the same way as with audio files. Enter title, set start/end time, select .mp4 (in most cases) or .ts as export  format and finally click on the green "Export" button.

# Editing the playlist
Click on the "Title" button at the bottom to show the controls needed for adding, deleting and editing playlist titles.

* To edit/copy/delete a title hold the \<Ctrl\> key and select the title from the playlist.
* To delete the title click on the red "Delete" button.
* If needed: To set the title time seek to new position, hold the \<Ctrl\> key and click on the timestamp. To verify the position click on the timestamp while playing.
* Edit the title and commit with \<Enter\> or cancel with \<Esc\>
* To add a new title click on the green "Add" button.

All changes to the playlist can be done while recording. There is no backup or undo - so be careful.

# Setting labels on slidebars

Inserting an exclamation mark ! as the first character in a playlist title will place the first word as a clickable label under the main slider at the bottom.

To place a label under the title slider at the top append an at sign @ to the relevant word followed by the absolute position in seconds. To get the position hold the \<Ctrl\> key and click into the edit field.

In order to place the label above the line insert a circumflex \^ directly after the word. Use the degree sign ° to place the label on the line.

# Multi column playlist

Use the vertical bar | to separate titles into columns. This can be helpful for example if you use radioZiner as a karaoke tool with differnt languages.

# Sharing channel groups

Channel groups are stored as separate M3U/TVG formatted (.m3u) text files in folder \<*PathToYourStreamingDirectory*\>\\channels. To import a channel group place the .m3u file (i.e. [All German TV-Stations](https://iptv-org.github.io/iptv/countries/de.m3u)) in the channels folder and restart radioZiner. To share channel groups with your friends simply send them your .m3u files.

---

Needed binaries:<br>
[ffmpeg.exe](https://www.ffmpeg.org/download.html) FFmpeg [project](https://www.ffmpeg.org/)<br>
[libmpv-2.dll](https://sourceforge.net/projects/mpv-player-windows/files/libmpv/) mpv-player [project](https://github.com/mpv-player/mpv)<br>

