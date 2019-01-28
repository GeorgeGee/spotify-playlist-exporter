using CommandLine;
using SpotifyPlaylistExporter;
using System;
using System.IO;
using System.Linq;

namespace SpotifyPlaylistExporterConsole
{
    public class Options
    {
        [Option('i', "clientId", Required = true, HelpText = "Client ID")]
        public string ClientId { get; set; }

        [Option('s', "clientSecret", Required = true, HelpText = "Client Secret")]
        public string ClientSecret { get; set; }

        [Option('p', "playlistId", Required = true, HelpText = "Playlist ID")]
        public string PlaylistId { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output .txt file path")]
        public string OutputFilePath { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            if (result.Tag == ParserResultType.NotParsed)
                return;
            Options options = ((Parsed<Options>)result).Value;
            
            try
            {
                var spotifyExporter = new SpotifyExporter(options.ClientId, options.ClientSecret);
                var trackList = spotifyExporter.GetPlaylistTracks(options.PlaylistId)
                    .GetAwaiter().GetResult()
                    .OrderByDescending(t => t.AddedAt).ToList();

                var lines = trackList.Select(t => $"{t.Track.Name} - {t.Track.Artists.First().Name}");
                File.WriteAllLines(options.OutputFilePath, lines);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
