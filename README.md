# Spotify Playlist Exporter

> C#/.NET library and console utility for exporting Spotify playlists

Please note that traditional user authentication is not yet implemented. Instead, the library and console utility require a Client ID and Client Secret which are available on https://developer.spotify.com/dashboard/

## Features
- Export a Spotify playlist to a text file for backup purposes

## Usage
1. Download the latest `spotify-playlist-exporter-x.x.x.zip` (where x.x.x is the version number) on the [Releases](https://github.com/GeorgeGee/spotify-playlist-exporter/releases) page and extract the contents into a folder
2. Open PowerShell or Command Prompt and change directory to the folder which contains the downloaded files
3. Run the following command:  
`.\SpotifyPlaylistExporterConsole.exe --clientId "CLIENT_ID" --clientSecret "CLIENT_SECRET" --playlistId "PLAYLIST_ID" --output "playlist.txt"`
> Replace `CLIENT_ID` and `CLIENT_SECRET` with your Spotify Client ID and Client Secret. This will (hopefully) soon be removed and replaced with traditional user authentication  
> Replace `PLAYLIST_ID` with the Spotify Playlist ID

## Arguments
Name|Description
-|-
`-i` or `--clientId`|Required. Client ID
`-s` or `--clientSecret`|Required. Client Secret
`-p` or `--playlistId`|Required. Playlist ID
`-o` or `--output`|Required. Output .txt file path