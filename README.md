<p align="center">
  English
  | 
  <a href="README.de.md">Deutsch</a>
</p>

# radioZiner
Streaming audio & video recorder

![radioZiner-01](https://github.com/user-attachments/assets/0515a02e-0fe9-430b-8b56-72bc49409164)


# Installation

* [Download](https://github.com/eZiner/radioZiner/releases/) the latest version (e.g. radioZiner-v#.#.#.zip).
* Create a folder "radioZiner" and unzip the files into that folder.
* Within the folder start radioZiner.exe.
* You should get the following message:

```
No streaming directory
Create new streaming directory in "C:\Users\<username>\Videos\radioZiner"?
<Yes>  <No>
```

* Click on "Yes" or read below how to create a custom streaming directory.

> #### How to create and use a custom streaming directory
>* Create a new folder (e.g. *myStreamingDirectory*).
>* Create a shortcut of radioZiner.exe (e.g. *myRadioZiner.exe*).
>* Open the properties of *myRadioZiner.exe*.
>* Append a \<Space\> + mainDir="*PathToYourStreamingDirectory*" to the target path.
>* Start *myRadioZiner.exe*.

* Select File/Home (Ctrl+H) from the menu to open the streaming directory in the file explorer.
* Close the application.
* Go to the radioZiner application folder and select the following files:
    * **backup_de1_2024-03-21.sql** - Latest version can be [downloaded](https://backups.radio-browser.info/latest.sql.gz) from radio-browser.info.
    * **index-1.m3u** - Latest version can be [downloaded](https://iptv-org.github.io/iptv/index.m3u) from iptv-org.
* Move them to \<*PathToYourStreamingDirectory*\>\\streams.
* To update your database, replace the files with the latest versions.
* Start radioZiner.exe again.
* After a few seconds, the text of the "Search" button at the top left should change from grey to black.
* radioZiner is ready to use.

# Getting started after installation

* Create new channel group(s)
    * Select File/New (Ctrl+N) from the menu.
    * Enter a name for the channel group (e.g. "Radio") and confirm with \<Enter\> or cancel with \<Esc\>.
    * Repeat the process if you like (e.g. "TV", "Irish Folk", ...).
    * To change the name of a channel group, select the name twice and press \<Enter\>, edit the name, and press \<Enter\> again.
  
* Find radio/tv stations in the database
    * Click in the text box to the right of the search button at the top left.
    * Enter search term (e.g. "pub .ie" for Irish stations with "pub" in their name or keywords).
    * Select a station.
    * Playback will start immediately.
 
* Control playback
    * To pause/play, click the right mouse button or use the pause button at the bottom left.
    * Use the slider on one of the timelines or the mouse wheel to fast-forward or rewind.
    * Using the mouse wheel is somewhat unusual: with the mouse pointer on the left side of a list, the wheel changes the font size, in the middle it fast-forwards/rewinds and on the right side it scrolls the content up and down.
    * In pause mode, the mouse wheel steps forward/backward image by image (Jog/Shuttle).
    * To show/hide the list on the left-hand side, click on the left switch ▼.
    * To show/hide the playlist while viewing a video, click on the right switch ▼.
    * To show/hide all controls, press \<F12\>.
    * To enable or disable full screen mode, press \<F11\> or double-click in the video.

* Play any audio/video source using drag/drop or copy/paste
    * Drag or paste any valid stream or file url on the playlist or video to play an audio or video.
    * Free radio stations can be found here: [fmstream.org](http://fmstream.org/) | [Icecast Directory](http://dir.xiph.org/) | [RadioBrowser](https://www.radio-browser.info/)
    * Free TV stations/movies can be found here: [iptv-org](https://iptv-org.github.io/) | [MediathekViewWeb (Germany)](https://mediathekviewweb.de/)

* Add new stations, files, or folders to the channel group
    * While playing a new station, a new file or after dragging/pasting a folder ...
    * Choose appropriate channel group for the new station/file/folder (e.g. "Irish Folk").
    * Click on the channel name ("New Channel" if it is new) at the top.
    * Edit the name and confirm with \<Enter\> or cancel with \<Esc\>.
    * The channel name can be changed at any time in the same way. After the change, a copy with the old name of the channel remains. Delete the old copy or use it as an alias.
    * Local files and folders that have been added as channels can be selected by clicking on the "Files" button at the top. Folders are marked by square brackets \[ \] around the name. When you click on a folder, all playable audios/videos in this folder and its subfolders are listed in descending order by date. Click on the "Files" button again to display/update the list of files and folders again.

* Move/Delete channel
    * Choose the channel and select Edit/Cut (Ctrl+X) from the menu.
    * To move the channel, choose a channel group and select Edit/Paste (Ctrl+V) from the menu.
    * To delete the channel, just don´t paste it.
  
* Record stream
    * Click the "Rec" button at the bottom.
    * The recording starts immediately and the name of the channel is added to the recording list as a button.
    * To play while recording (monitoring), click on the corresponding channel in the recording list.
    * If the channel was new it had been automatically added to the selected channel group.
    * To stop recording, click the "Stop" button at the bottom.
    * Some video channels, such as ARTE Germany, cannot be recorded correctly due to stream errors that cause problems for mpv recording. In these cases, you can try to force the use of ffmpeg instead of mpv by appending an At @ to the channel name.
    * All recordings are stored in your streaming directory along with a text file that contains the playlist.
    * To listen/watch the audio/video files click on the "Recordings" button at the top. Click again to update the list.


# Export music titles to audio files

To save individual titles from the recorded radio broadcasts as an audio file, first click on the "Recordings" button at the top and select the recording you want. Then click the "Export" button at the bottom to show the controls needed for setting start/end time, the file name and export format.

Locate the start of the title you want to export. If the radio station the audio file was recorded from supports icecast, you already have a playlist. Just click on the title and you are almost done. Often the displayed start time does not exactly match the beginning of the title. In this case, a fine-tuning has yet to be made.

* To set the export name and start time, hold the \<Ctrl\> key and select the title in the playlist.
* Enter the export name and set the two timestamps on the left side of the input field.
* To set the start time, hold the \<Ctrl\> key and click on the left timestamp.
* To set the end time, hold the \<Ctrl\> key and click on the right timestamp.
* To verify the position, click on the start/end timestamp while playing.
* Select .mp3 (in most cases) or .aac, depending on the stream format.
* Finally, click on the green "Export" button.

The exported audio file will be stored in your streaming directory. Click on the "Exports" button at the top to get a list of your exports. Click again to update the list.

# Editing & exporting to video files

Exporting movies, scenes or sequences works in pretty much the same way as with audio files. Enter the title, set start/end time, select .mp4 (in most cases) or .ts as export format, and finally click on the green "Export" button.

# Editing the playlist
Click the "Title" button at the bottom to show the controls needed for adding, deleting and editing playlist titles.

* To edit/copy/delete a title, hold the \<Ctrl\> key and select the title from the playlist or timeline.
* To delete the title, click on the red "Delete" button.
* If necessary: To change the title time, go to the new position, hold the \<Ctrl\> key and click on the timestamp. To verify the position click on the timestamp while playing.
* Edit the title and confirm with \<Enter\> or cancel with \<Esc\>
* To add a new title, click the green "Add" button.

All changes to the playlist can be made during recording. The changes are saved immediately and cannot be undone. So be careful!

# Setting labels on the timeline

Inserting an exclamation mark ! as the first character in a playlist title will place the first word as a clickable label under the main timeline at the bottom.

To place a label under the title timeline at the top, append an at sign @ to the relevant word followed by the absolute position in seconds. To get the position hold the \<Ctrl\> key and click into the edit field.

In order to place the label above the timeline, insert a circumflex \^ directly after the word. Use the degree sign ° to place the label on the timeline.

# Multi column playlist

Use the vertical bar | to separate titles into columns. This can be helpful for example if you use radioZiner as a karaoke tool with differnt languages.

# Sharing (export/import) of channel groups

Channel groups are stored as separate M3U/TVG formatted (.m3u) text files in the folder \<*PathToYourStreamingDirectory*\>\\channels. To import a channel group, place the .m3u file (e.g. [All German TV Stations](https://iptv-org.github.io/iptv/countries/de.m3u)) in the channels folder and restart radioZiner. To share channel groups with your friends simply send them your .m3u files.

---

Needed binaries:<br>
[ffmpeg.exe](https://www.ffmpeg.org/download.html) FFmpeg [project](https://www.ffmpeg.org/)<br>
[libmpv-2.dll](https://sourceforge.net/projects/mpv-player-windows/files/libmpv/) mpv-player [project](https://github.com/mpv-player/mpv)<br>

