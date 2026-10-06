from config import get_kafka_bootstrap_servers, get_kafka_group_id
from confluent_kafka import Consumer


def create_consumer():

    bootstrap = get_kafka_bootstrap_servers()
    group_id = get_kafka_group_id()

    config = {
        "bootstrap.servers": bootstrap,
        "group.id": group_id,
        "auto.offset.reset": "earliest",
    }

    return Consumer(config)
