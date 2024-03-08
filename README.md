# Basic F# + RabbitMQ

This repository holds a simple project that illustrates how one can use F# along with RabbitMQ to send and receive messages. It is 100% based on the RabbitMQ [Get Started documentation for .NET](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet). 

For an older version you can check this other repository [edgarsanchez/FsRabbitMQ-Tutorials](https://github.com/edgarsanchez/FsRabbitMQ-Tutorials).

## How to use?

Basic example:

```bash
# 1. start the RabbitMQ process
docker-compose up -d

# 2. run the Receive/ project
# our consumer
dotnet run --project Receive/

# 3. run the Send/ project
# our publisher
dotnet run --project Send/
```

---

Work queues:

```bash
# 1. start the RabbitMQ process
docker-compose up -d

# 2.1. run the Worker/ project
# our consumer 1
dotnet run --project Worker/

# 2.2. run the Worker/ project
# our consumer 2
dotnet run --project Worker/

# 3. run the NewTask/ project
# our publisher
dotnet run --project NewTask/ "First message."
dotnet run --project NewTask/ "Second message.."
dotnet run --project NewTask/ "Third message..."
dotnet run --project NewTask/ "Fourth message...."

# notice that the messages are distributed according to round-robin
# through our worker instances
```