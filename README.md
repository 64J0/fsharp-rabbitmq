# F# + RabbitMQ

This repository holds a simple project that illustrates how one can use F# along with RabbitMQ to send and receive messages. It is 100% based on the RabbitMQ [Get Started documentation for .NET](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet). 

For an older version you can check this other repository [edgarsanchez/FsRabbitMQ-Tutorials](https://github.com/edgarsanchez/FsRabbitMQ-Tutorials).

I'm adding my notes while studying RabbitMQ documentation to [this document](./NOTES.md). Hope it's useful to understand this tool and most of the code here.

## How to use?

First, make sure that you have started the RabbitMQ container and restored the packages with:

```bash
docker-compose up -d

dotnet restore
```

Now you can start looking for the example instructions.

Basic example:

```bash
# 1. run the Receive/ project
# our consumer
dotnet run --project Receive/

# 2. run the Send/ project
# our publisher
dotnet run --project Send/
```

---

**Work-queue pattern** - each task is delivered to exactly one worker:

```bash
# 1.1. run the Worker/ project
# our consumer 1
dotnet run --project Worker/

# 1.2. run the Worker/ project
# our consumer 2
dotnet run --project Worker/

# 2. run the NewTask/ project
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
# 1.1 run the ReceiveLogs/ project
dotnet run --project ReceiveLogs/

# 1.2 run the ReceiveLogs/ project
dotnet run --project ReceiveLogs/

# 2. run the EmitLog/ project
dotnet run --project EmitLog/ "First message."
dotnet run --project EmitLog/ "Second message.."
dotnet run --project EmitLog/ "Third message..."
dotnet run --project EmitLog/ "Fourth message...."
```

---

**Publish/subscribe pattern 2** - deliver a message to multiple consumers, but limit some consumers to a subset of all the messages (`direct` exchange):

```bash
# 1 run the ReceiveLogsDirect/ project
dotnet run --project ReceiveLogsDirect/

# 2. run the EmitLogDirect/ project
dotnet run --project EmitLogDirect/ error "First message."
dotnet run --project EmitLogDirect/ info "Second message.."
dotnet run --project EmitLogDirect/ warning "Third message..."
dotnet run --project EmitLogDirect/ error "Fourth message...."
```