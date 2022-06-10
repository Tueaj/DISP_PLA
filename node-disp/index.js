const express = require('express')
const app = express()
const amqp = require('amqplib/callback_api');

let _connection;

const RABBITMQ_HOST = process.env.MessageHandling__HostName || "localhost";
const PORT = parseInt(process.env.ListenPort) || 3000;

const exchange = 'COMMAND';
const routingKey = 'Messages.ShipOrder';
const queue = 'NODE_SHIPMENT_SERVICE';

amqp.connect(`amqp://${RABBITMQ_HOST}`, function (error, connection) {
    if (error) {
        throw error;
    }
    _connection = connection

    _connection.createChannel(function(error, channel) {
        if (error) {
            console.log("nooo error uhhhhhhhwwwwuus");
            throw error;
        }

        channel.assertExchange(exchange, 'direct');
        channel.assertQueue(queue);

        channel.bindQueue(queue, exchange, routingKey);
        
        channel.consume(queue, message => {
            const messageObject = JSON.parse(message.content.toString());
            console.log(messageObject);
            channel.publish(exchange, 'Messages.ShipOrderAck', Buffer.from(JSON.stringify({TransactionId: messageObject.TransactionId})))
            channel.ack(message);
        })
      });
});

app.listen(PORT);