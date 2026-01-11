using Newtonsoft.Json;
using SocialNetwork.Data;
using SocialNetwork.DataProcessor.ExportDTOs;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocialNetwork.DataProcessor
{
    public class Serializer
    {
        public static string ExportUsersWithFriendShipsCountAndTheirPosts(SocialNetworkDbContext context)
        {
            var usersRaw = context.Users
                .Select(u => new
                {
                    u.Username,

                    Friendships = context.Friendships
                        .Count(f => f.UserOneId == u.Id || f.UserTwoId == u.Id),

                    Posts = u.Posts
                        .OrderBy(p => p.Id)
                        .Select(p => new
                        {
                            p.Content,
                            p.CreatedAt
                        })
                        .ToArray()
                })
                .OrderBy(u => u.Username)
                .ToArray();

            var users = usersRaw
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Friendships = u.Friendships,
                    Posts = u.Posts
                        .Select(p => new ExportPostDto
                        {
                            Content = p.Content,
                            CreatedAt = p.CreatedAt
                                .ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
                        })
                        .ToArray()
                })
                .ToArray();

            var xmlRoot = new XmlRootAttribute("Users");
            var xmlSerializer = new XmlSerializer(typeof(ExportUserDto[]), xmlRoot);

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, users, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportConversationsWithMessagesChronologically(SocialNetworkDbContext context)
        {
            var conversationsRaw = context.Conversations
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.StartedAt,
                    Messages = c.Messages
                        .OrderBy(m => m.SentAt)
                        .Select(m => new
                        {
                            m.Content,
                            m.SentAt,
                            Status = (int)m.Status,
                            SenderUsername = m.Sender.Username
                        })
                        .ToArray()
                })
                .OrderBy(c => c.StartedAt)
                .ToArray();

            var conversations = conversationsRaw
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    StartedAt = c.StartedAt
                        .ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    Messages = c.Messages
                        .Select(m => new
                        {
                            m.Content,
                            SentAt = m.SentAt
                                .ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                            m.Status,
                            m.SenderUsername
                        })
                        .ToArray()
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(conversations, Formatting.Indented);
            return json;
        }
    }
}
