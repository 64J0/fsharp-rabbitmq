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

    channel.ExchangeDeclare(exchange = "direct_logs", ``type`` = ExchangeType.Direct)

    // declare a server-named queue
    let queueName1 = channel.QueueDeclare().QueueName
    let queueName2 = channel.QueueDeclare().QueueName

    let severities = [ "info"; "warning"; "error" ]

    for severity in severities do
        channel.QueueBind(queue = queueName1, exchange = "direct_logs", routingKey = severity)

    channel.QueueBind(queue = queueName2, exchange = "direct_logs", routingKey = (List.last severities))

    printfn "[*] Waiting for logs."

    let consumer = new EventingBasicConsumer(channel)

    consumer.Received.AddHandler(fun _model ea ->
        let body = ea.Body.ToArray()
        let message = Encoding.UTF8.GetString(body)
        let routingKey = ea.RoutingKey
        let deliveryTag = ea.DeliveryTag
        printfn $"[x] Received {deliveryTag} '{routingKey}'.'{message}'")

    channel.BasicConsume(queue = queueName1, autoAck = true, consumer = consumer)
    |> ignore

    channel.BasicConsume(queue = queueName2, autoAck = true, consumer = consumer)
    |> ignore

    printfn "Press [enter] to exit."
    Console.ReadLine() |> ignore

    0
