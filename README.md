# SentimentChatbotWebAPI

This is the backend API for a chatbot that provides simple sentiment analysis based on the text inputted. See the React UI here: https://github.com/kieron-cairns/Sentiment-Chatbot-UI

Sentiment analysis is based on this pre-trained model by Microsoft: https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis-model-builder

In a production scenario, this sample project would be hosted as an azure function, & results will be retrieved via the PostQueryToSql API endpoint in the project. 

Queries & results are saved to an SQL table. 

. Entity Framework Core used for easy database creation & modification

. User login managed with JSON bearer web tokens & Microsoft azure KeyVault

. Class wrappers used for unit & integration testing purposes & to also re-enforce the Dependency Inversion solid principle.


![8b62626d-a345-499d-b70a-a97b6da65f98](https://github.com/kieron-cairns/SentimentChatbotWebAPI/assets/72394263/b9061499-b07d-4043-a40b-0b91c9f9db4c)

Future goals: Train and implement my own model using online learning for sequential data feeds to the training model in order to achieve continues learning.  
