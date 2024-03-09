# F# + RabbitMQ

This repository holds a simple project that illustrates how one can use F# along with RabbitMQ to send and receive messages. It is 100% based on the RabbitMQ [Get Started documentation for .NET](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet). 

For an older version you can check this other repository [edgarsanchez/FsRabbitMQ-Tutorials](https://github.com/edgarsanchez/FsRabbitMQ-Tutorials).

I'm adding my notes while studying RabbitMQ documentation to [this document](./NOTES.md). Hope it's useful to understand this tool and most of the code here.

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

**Work-queue pattern** - each task is delivered to exactly one worker:

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

---

**Publish/subscribe pattern** - deliver a message to multiple consumers:

```bash
# 1. start the RabbitMQ process
docker-compose up -d

# 2. run the ReceiveLogs/ project
dotnet run --project ReceiveLogs/

# 3. run the EmitLog/ project
dotnet run --project EmitLog/ "First message."
dotnet run --project EmitLog/ "Second message.."
dotnet run --project EmitLog/ "Third message..."
dotnet run --project EmitLog/ "Fourth message...."
```