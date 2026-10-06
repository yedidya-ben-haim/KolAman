import json

import pika


class rabbitmq_service:
    def __init__(self, host="localhost"):
        credentials = pika.PlainCredentials("guest", "guest")

        parameters = pika.ConnectionParameters(
            host=host, port=5672, credentials=credentials
        )
        try:

            self.connection = pika.BlockingConnection(parameters)
            print("Connection established successfully.")
        except Exception as e:
            print("Failed to establish connection.")



        self.channel = self.connection.channel()

        print("rabbitmq is connected")

        Commands = ["CENTER", "SOUTH", "NORTH", "DEPTH"]

        for command in Commands:
            self.channel.queue_declare(
                queue=command, durable=True, arguments={"x-queue-type": "quorum"}
                )

            print(f"{command} queue is created")


    def sent(self, alert: dict, command):
        self.channel.basic_publish(exchange="", routing_key=command, body=json.dumps(alert))

