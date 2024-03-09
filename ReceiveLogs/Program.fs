open System
open System.Text
open System.Threading
open RabbitMQ.Client
open RabbitMQ.Client.Events

[<EntryPoint>]
let main (_args: string[]) : int =
    let factory = new ConnectionFactory()
    factory.HostName <- "localhost"

    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()

    channel.ExchangeDeclare(exchange = "logs", ``type`` = ExchangeType.Fanout)

    // declare a server-named queue
    let queueName = channel.QueueDeclare().QueueName
    channel.QueueBind(queue = queueName, exchange = "logs", routingKey = "")

    printfn "[*] Waiting for logs."

    let consumer = new EventingBasicConsumer(channel)

    consumer.Received.AddHandler(fun _model ea ->
        let body = ea.Body.ToArray()
        let message = Encoding.UTF8.GetString(body)
        printfn $"[x] Received {message}")

    channel.BasicConsume(queue = queueName, autoAck = true, consumer = consumer)
    |> ignore

    printfn "Press [enter] to exit."
    Console.ReadLine() |> ignore

    0
