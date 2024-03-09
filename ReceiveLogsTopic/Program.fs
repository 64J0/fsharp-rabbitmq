open System
open System.Text
open System.Threading
open RabbitMQ.Client
open RabbitMQ.Client.Events

[<EntryPoint>]
let main (args: string[]) : int =
    let factory = new ConnectionFactory()
    factory.HostName <- "localhost"

    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()

    channel.ExchangeDeclare(exchange = "topic_logs", ``type`` = ExchangeType.Topic)

    // declare a server-named queue
    let queueName = channel.QueueDeclare().QueueName

    if (args.Length = 0) then
        raise (Exception "Please provide args")

    for bindingKey in args do
        channel.QueueBind(queue = queueName, exchange = "topic_logs", routingKey = bindingKey)

    printfn "[*] Waiting for logs."

    let consumer = new EventingBasicConsumer(channel)

    consumer.Received.AddHandler(fun _model ea ->
        let body = ea.Body.ToArray()
        let message = Encoding.UTF8.GetString(body)
        let routingKey = ea.RoutingKey
        printfn $"[x] Received '{routingKey}'.'{message}'")

    channel.BasicConsume(queue = queueName, autoAck = true, consumer = consumer)
    |> ignore

    printfn "Press [enter] to exit."
    Console.ReadLine() |> ignore

    0
