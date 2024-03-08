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

    let arguments = Seq.empty |> dict

    // Declaring a queue is idempotent - it will only be created if it doesn't exist already.
    //
    // Change durable to true to make it durable, but keep in mind that RabbitMQ doesn't allow
    // you to redefine an existing queue with different parameters and will return an error to
    // any program that tries to do that.
    channel.QueueDeclare(
        queue = "task_queue",
        durable = false,
        exclusive = false,
        autoDelete = false,
        arguments = arguments
    )
    |> ignore

    channel.BasicQos(prefetchSize = (uint32 0), prefetchCount = (uint16 1), ``global`` = false)

    printfn "[*] Waiting for messages."

    let consumer = new EventingBasicConsumer(channel)

    consumer.Received.AddHandler(fun _model ea ->
        let body = ea.Body.ToArray()
        let message = Encoding.UTF8.GetString(body)
        printfn $"[x] Received {message}"

        let dots = message.Split('.').Length - 1
        Thread.Sleep(dots * 1000)

        printfn "[x] Done"

        // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
        // Acknowledgement must be sent on the same channel that received the delivery.
        // Attempts to acknowledge using a different channel will result in a channel-level
        // protocol exception.
        channel.BasicAck(deliveryTag = ea.DeliveryTag, multiple = false))

    channel.BasicConsume(queue = "task_queue", autoAck = false, consumer = consumer)
    |> ignore

    printfn "Press [enter] to exit."
    Console.ReadLine() |> ignore

    0
