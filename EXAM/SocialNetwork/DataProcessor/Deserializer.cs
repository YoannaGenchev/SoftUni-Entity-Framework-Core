using Newtonsoft.Json;
using SocialNetwork.Data;
using SocialNetwork.Data.Models;
using SocialNetwork.Data.Models.Enum;
using SocialNetwork.DataProcessor.ImportDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocialNetwork.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format.";
        private const string DuplicatedDataMessage = "Duplicated data.";

        private const string SuccessfullyImportedMessage =
            "Successfully imported message (Sent at: {0}, Status: {1})";

        private const string SuccessfullyImportedPost =
            "Successfully imported post (Creator {0}, Created at: {1})";

        public static string ImportMessages(SocialNetworkDbContext dbContext, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlRoot = new XmlRootAttribute("Messages");
            var serializer = new XmlSerializer(typeof(ImportMessageDto[]), xmlRoot);

            ImportMessageDto[] messageDtos;

            using (var reader = new StringReader(xmlString))
            {
                messageDtos = (ImportMessageDto[])serializer.Deserialize(reader)!;
            }

            var validConversationIds = dbContext.Conversations
                .Select(c => c.Id)
                .ToHashSet();

            var validUserIds = dbContext.Users
                .Select(u => u.Id)
                .ToHashSet();

            var existingMessagesKeys = dbContext.Messages
                .Select(m => new
                {
                    m.Content,
                    m.SentAt,
                    m.Status,
                    m.SenderId,
                    m.ConversationId
                })
                .ToHashSet();

            var messagesToAdd = new List<Message>();

            foreach (var dto in messageDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!DateTime.TryParseExact(dto.SentAt,
                        "yyyy-MM-ddTHH:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var sentAt))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.TryParse<MessageStatus>(dto.Status, out var status))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!validConversationIds.Contains(dto.ConversationId) ||
                    !validUserIds.Contains(dto.SenderId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var key = new
                {
                    Content = dto.Content,
                    SentAt = sentAt,
                    Status = status,
                    SenderId = dto.SenderId,
                    ConversationId = dto.ConversationId
                };

                bool isDuplicateInDb = existingMessagesKeys.Contains(key);
                bool isDuplicateInBatch = messagesToAdd.Any(m =>
                    m.Content == key.Content &&
                    m.SentAt == key.SentAt &&
                    m.Status == key.Status &&
                    m.SenderId == key.SenderId &&
                    m.ConversationId == key.ConversationId);

                if (isDuplicateInDb || isDuplicateInBatch)
                {
                    sb.AppendLine(DuplicatedDataMessage);
                    continue;
                }

                var message = new Message
                {
                    Content = dto.Content,
                    SentAt = sentAt,
                    Status = status,
                    ConversationId = dto.ConversationId,
                    SenderId = dto.SenderId
                };

                messagesToAdd.Add(message);
                sb.AppendLine(string.Format(
                    SuccessfullyImportedMessage,
                    sentAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    status));
            }

            dbContext.Messages.AddRange(messagesToAdd);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPosts(SocialNetworkDbContext dbContext, string jsonString)
        {
            var sb = new StringBuilder();

            var postDtos = JsonConvert.DeserializeObject<ImportPostDto[]>(jsonString)!;

            var validUserIds = dbContext.Users
                .Select(u => u.Id)
                .ToHashSet();

            var existingPostsKeys = dbContext.Posts
                .Select(p => new
                {
                    p.Content,
                    p.CreatedAt,
                    p.CreatorId
                })
                .ToHashSet();

            var postsToAdd = new List<Post>();

            foreach (var dto in postDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!DateTime.TryParseExact(dto.CreatedAt,
                        "yyyy-MM-ddTHH:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var createdAt))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!validUserIds.Contains(dto.CreatorId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var key = new
                {
                    Content = dto.Content,
                    CreatedAt = createdAt,
                    CreatorId = dto.CreatorId
                };

                bool isDuplicateInDb = existingPostsKeys.Contains(key);
                bool isDuplicateInBatch = postsToAdd.Any(p =>
                    p.Content == key.Content &&
                    p.CreatedAt == key.CreatedAt &&
                    p.CreatorId == key.CreatorId);

                if (isDuplicateInDb || isDuplicateInBatch)
                {
                    sb.AppendLine(DuplicatedDataMessage);
                    continue;
                }

                var post = new Post
                {
                    Content = dto.Content,
                    CreatedAt = createdAt,
                    CreatorId = dto.CreatorId
                };

                postsToAdd.Add(post);

                var creatorUsername = dbContext.Users
                    .Where(u => u.Id == dto.CreatorId)
                    .Select(u => u.Username)
                    .First();

                sb.AppendLine(string.Format(
                    SuccessfullyImportedPost,
                    creatorUsername,
                    createdAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)));
            }

            dbContext.Posts.AddRange(postsToAdd);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResults, true);
        }
    }
}
