using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MusicHub.Data;

namespace MusicHub
{
    public class StartUp
    {
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var minDuration = TimeSpan.FromSeconds(duration);

            var songs = context.Songs
                .AsNoTracking()
                .Where(s => s.Duration > minDuration)
                .Select(s => new
                {
                    s.Name,
                    WriterName = s.Writer.Name,
                    Performers = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .OrderBy(p => p)
                        .ToArray(),
                    AlbumProducer = s.Album.Producer.Name,
                    s.Duration
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray();

            var sb = new StringBuilder();

            for (int i = 0; i < songs.Length; i++)
            {
                var song = songs[i];

                sb.AppendLine($"-Song #{i + 1}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                foreach (var performer in song.Performers)
                {
                    sb.AppendLine($"---Performer: {performer}");
                }

                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration.ToString("c")}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
