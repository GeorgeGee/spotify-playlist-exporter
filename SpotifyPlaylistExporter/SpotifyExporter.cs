using SpotifyPlaylistExporter.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyPlaylistExporter
{
    public class SpotifyExporter
    {
        private string _clientId;
        private string _clientSecret;
        private SpotifyToken _spotifyToken;

        public SpotifyExporter(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// Get a Spotify Access Token
        /// Follows the "Client Credentials" flow: https://developer.spotify.com/documentation/general/guides/authorization-guide/
        /// </summary>
        /// <returns>Spotify Token</returns>
        private async Task<SpotifyToken> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                var requestEncoded = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

                client.DefaultRequestHeaders.Add(
                     "Authorization",
                     $"Basic {requestEncoded}");

                var requestBody = new StringContent(
                    "grant_type=client_credentials",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                var tokenResult = await client.PostAsync("https://accounts.spotify.com/api/token", requestBody);
                if (tokenResult.IsSuccessStatusCode)
                {
                    return await tokenResult.Content.ReadAsAsync<SpotifyToken>();
                }
                else
                {
                    throw new Exception("Failed to get access token from Spotify Web API.");
                }
            }
        }

        /// <summary>
        /// Get a Playlist's Tracks
        /// </summary>
        /// <param name="playlistId">Playlist ID</param>
        /// <returns>The task result contains the Playlist</returns>
        public async Task<List<PlaylistTrack>> GetPlaylistTracks(string playlistId)
        {
            if (_spotifyToken == null)
                _spotifyToken = await GetAccessToken();

            // Endpoint: https://developer.spotify.com/documentation/web-api/reference/playlists/get-playlists-tracks/
            // Object model reference: https://developer.spotify.com/documentation/web-api/reference/object-model/#paging-object
            // Object model source: https://github.com/JohnnyCrazy/SpotifyAPI-NET
            var tracks = await Get<Paging<PlaylistTrack>>(_spotifyToken.AccessToken, new Uri($"https://api.spotify.com/v1/playlists/{playlistId}/tracks"));

            List<PlaylistTrack> trackList = new List<PlaylistTrack>();
            while (tracks.HasNextPage())
            {
                trackList.AddRange(tracks.Items);
                tracks = await Get<Paging<PlaylistTrack>>(_spotifyToken.AccessToken, new Uri(tracks.Next));
            }
            trackList.AddRange(tracks.Items);

            return trackList;
        }
        
        /// <summary>
        /// Get a Track
        /// </summary>
        /// <param name="trackId">Track ID</param>
        /// <returns>The task result contains the Track</returns>
        public async Task<FullTrack> GetTrack(string trackId)
        {
            if (_spotifyToken == null)
                _spotifyToken = await GetAccessToken();

            return await Get<FullTrack>(_spotifyToken.AccessToken, new Uri($"https://api.spotify.com/v1/tracks/{trackId}"));
        }

        /// <summary>
        /// Constructs a POST request to the given URI with authorisation
        /// </summary>
        /// <typeparam name="T">Endpoint return type</typeparam>
        /// <param name="accessToken">Spotify Access Token</param>
        /// <param name="uri">Endpoint URI</param>
        /// <returns>The task result contains the POST request result</returns>
        private async Task<T> Get<T>(string accessToken, Uri uri)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                     "Authorization",
                     $"Bearer {accessToken}");

                var tokenResult = await client.GetAsync(uri);
                if (tokenResult.IsSuccessStatusCode)
                {
                    return await tokenResult.Content.ReadAsAsync<T>();
                }
                throw new Exception($"Failed to get data from Spotify Web API. Uri: '{uri.AbsoluteUri}'");
            }
        }
    }
}
