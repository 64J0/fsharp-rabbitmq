# Basic F# + RabbitMQ

This repository holds a simple project that illustrates how one can use F# along with RabbitMQ to send and receive messages. For more example you can check this other repository [edgarsanchez/FsRabbitMQ-Tutorials](https://github.com/edgarsanchez/FsRabbitMQ-Tutorials).

## How to use?

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