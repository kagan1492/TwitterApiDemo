## TwitterApiDemo

## How to run:
Please run TwitterDemoAPI as a startup project and it will bring up Swagger API Documentation GUI.

## Dependencies:
.NET 5 SDK

## Some points to consider:
- A flexible throttling logic needs to be implemented in order to meet Twitter Api usage limitations.
- SaveTweet / GetTweet methods can have database implementations via Entity Framework / UnitofWork
- Logging structure needs to be improved. Also it can be logged to db.
- Hangfire can be utilized for background cron jobs
- More application settings parameters(cache, throttling durations etc) can be added to make the app even more configurable
- 100% test coverage needs to be implemented
- Application Keys should be stored in a secure Key Vault (Azure Vault, Envy Key etc)
- Swagger API documentation can be improved
- Twitter API http error codes can be translated, handled and displayed in a more organized manner
- Cancellation token can be implemented so that stream can be cancelled or paused

Scalability / Distributed approach improvements:  
- Pub/Sub (RabbitMQ, Azure Event Hub) can be implemented so that new tweets can be queued. Whenever a new Tweet Event gets raised, it can be published to all subscribed clients. All events can be handled appropriately. Solution then becomes distributed thus scalable.
- Processing can be made into a Microservice if it requires more complex logic in order to make it more scalable and independently deployable
- Distributed cache (Redis, MemCache) can be implemented in order to increase availability
- Cloud (Docker, Azure Event Hub, Azure Functions, Azure Cosmos DB, Azure Worker Service) can be utilized
