using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fiap.cartao
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                    //textos explicando o que são os argumentos (não tem nada a ver com o false ou o null)
                                //se a aplicação cair a fila continua la
                                 durable: false,
                                 //fila exclusiva de quem esta criando, se subir outra aplicação e tentar conectar, nao vai 
                                 exclusive: false,
                                 //quando desligar a aplicação, deleta a fila
                                 autoDelete: false,
                                 //argumentos adicionais
                                 arguments: null);
            Console.WriteLine(" [*] Aguardando novas mensagens.");

            //função anonima model é a interação com a fila, e argument é o evento( mensagem no caso)
           
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                //pega a mensagem(ea) e o corpo da mensagem
                var body = ea.Body.ToArray();
                //transforma o array de binario em string
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] recebido {message}");

                channel.BasicAck(ea.DeliveryTag, false);


            };
            //configurando no canal a fila q quer ouvir
            channel.BasicConsume(queue: "hello",
                    //entrega a mensagem, se tiver true so manda e ta tudo certo,
                    //se for false ele vai querer saber se a mensagem realmente chegou
                                 autoAck: false,
                                 consumer: consumer);
            //quando rodar, vai ficar parada aqui, pq se nao tiver o write line vai passar direto
            Console.WriteLine(" Pressione [enter] para finalizar.");
            Console.ReadLine();
        }
    }
}