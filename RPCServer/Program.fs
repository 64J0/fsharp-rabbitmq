open System
open System.Text
open RabbitMQ.Client
open RabbitMQ.Client.Events

let rec fib (n: int) : int =
    match n with
    | x when x < 0 -> raise (Exception "Valid numbers are positive integers")
    | 0 -> 1
    | 1 -> 1
    | n -> fib (n - 1) + fib (n - 2)

[<EntryPoint>]
let main (args: string[]) : int =
    let factory = new ConnectionFactory(HostName = "localhost")
    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()

    channel.QueueDeclare(queue = "rpc_queue", durable = false, exclusive = false, autoDelete = false, arguments = null)
    |> ignore

    channel.BasicQos(prefetchSize = uint32 0, prefetchCount = uint16 1, ``global`` = false)

    let consumer = new EventingBasicConsumer(channel)

    channel.BasicConsume(queue = "rpc_queue", autoAck = false, consumer = consumer)
    |> ignore

    printfn "[x] Awaiting RPC requests"

    consumer.Received.AddHandler(fun _model ea ->
        let mutable response = ""

        let body = ea.Body.ToArray()
        let props = ea.BasicProperties
        let replyProps = channel.CreateBasicProperties()
        replyProps.CorrelationId <- props.CorrelationId

        try
            try
                let message = Encoding.UTF8.GetString(body)
                let n = int message
                printfn $"[.] Fib({message})"
                response <- fib(n).ToString()
            with exn ->
                eprintfn $"[.] {exn.Message}"
                response <- ""
        finally
            let responseBytes = Encoding.UTF8.GetBytes(response)

            channel.BasicPublish(
                exchange = "",
                routingKey = props.ReplyTo,
                basicProperties = replyProps,
                body = responseBytes
            )

            channel.BasicAck(deliveryTag = ea.DeliveryTag, multiple = false))

    printfn "Press [enter] to exit."

    Console.ReadLine() |> ignore

    0
