﻿using SentimentChatbotWebAPI.Models;

namespace SentimentChatbotWebAPI.Interfaces
{
    public interface IChatbotRepository
    {
        string GenerateJwtToken(User user);
    }
}