import json

from config import (
    get_kafka_topic,
    get_redis_host,
    get_redis_port,
    get_redis_ttl,
    get_regions_file_path,
)
from Services.alert_vlidator import validate_alert
from Services.geo_classified import geo_classified
from Services.kafka_consumer import create_consumer
from Services.rabbitmq_service import rabbitmq_service
from Services.redis_service import redis_service


def main():
    # Starting services
    consumer = create_consumer()
    redis_sender = redis_service(host=get_redis_host(),
                                 port=get_redis_port(), ttl=get_redis_ttl())
    geo_classifier = geo_classified(file_path=get_regions_file_path())
    rabbitmq_servicer = rabbitmq_service()


    topic = get_kafka_topic()

    consumer.subscribe([topic])


    try:
        while True:
            # kafka consume massege

            msg = consumer.poll(1.0)
            if msg is None:
                continue
            elif msg.error():
                print(f"error consume mesaage:{msg.error()}")
                continue
            alert = json.loads(msg.value())
            print(f"consume massege {alert.get('alert_id')} Successfully")

            # validate alert
            if not validate_alert(alert):
                print(f"alert {alert.get('alert_id')} not not validated")
                continue

            try:
                # insert redis
                insert_redis = redis_sender.sent_alert(alert)
                if not insert_redis:
                    print(f"alert: {alert.get('alert_id')} Not entered into Redis")
                    continue

                print(f"redis insert alert: {alert.get('alert_id')} Successfully")

                # get alert region
                region = geo_classifier.get_region(alert.get("lon"), alert.get("lat"))

                rabbitmq_servicer.sent(alert, region)
                print(f"alert: {alert.get('alert_id')} sent to rabbit")


            except Exception as e:
                print(f"error: {e}")






    except KeyboardInterrupt:
        print("puse consume")
    finally:
        consumer.close()

if __name__ == "__main__":
    main()
