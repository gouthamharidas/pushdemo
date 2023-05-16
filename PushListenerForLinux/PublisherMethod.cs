using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushListenerForLinux
{
    internal class PublisherMethod
    {
        public void publish(string IP, string QueueName, string Message, string UserName, string Password)
        {
            var factory = new ConnectionFactory() { HostName = IP, UserName = UserName, Password = Password, VirtualHost = "/" };

            var connection = factory.CreateConnection();
            try
            {
                using (connection)
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: QueueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var body = Encoding.UTF8.GetBytes(Message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: QueueName,
                                             basicProperties: null,
                                             body: body);

                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

                connection.Close();
            }
        }
    }
}
