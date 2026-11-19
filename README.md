# MusicPlayer

A desktop music player for Windows, written in C# with WPF. A hobby project
from 2021; the interface is in Czech.

## What it does

- Reads all `.mp3` files from the folders chosen in settings and shows them
  as one "all songs" playlist.
- Reads title, artist, album, year, duration and cover art from the file tags
  (TagLib).
- Lets you create your own playlists, add and remove songs, and delete playlists.
- Sorts a playlist by title, artist, album or year, and can shuffle it.
- Plays with play/pause, previous/next, a seek slider, volume and mute.
- Remembers playlists, source folders and volume between runs in `PLdata.xml`.

## Layout

| Path | What it is |
|---|---|
| `Player/MainWindow.xaml(.cs)` | main window: loading songs, playback, saving and loading state |
| `Player/User_Controls/UserControl1` | the player bar: buttons, seek slider, scrolling song title |
| `Player/User_Controls/View_Playlist` | song list of one playlist, sorting and shuffle |
| `Player/User_Controls/Add_Playlist` | dialog for a new playlist |
| `Player/User_Controls/Setting` | volume and source folders |
| `Player/Models/` | `Song`, `Playlist` and `Data` (what gets saved) |

## Build

Windows only (WPF). Open `Player.sln` in Visual Studio, or:

```
dotnet build Player.sln
```

The project targets .NET Core 3.1, which is no longer supported; a newer SDK
builds it with a warning, running it needs the 3.1 desktop runtime or a
retarget in `Player/Player.csproj`.
