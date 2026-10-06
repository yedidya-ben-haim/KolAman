import os
from pathlib import Path

from dotenv import load_dotenv

load_dotenv()


def get_kafka_bootstrap_servers():
    return os.getenv("KAFKA_BOOTSTRAP_SERVERS")


def get_kafka_topic():
    return os.getenv("KAFKA_TOPIC")

def get_kafka_group_id():
    return os.getenv("KAFKA_GROUP_ID")

def get_redis_host():
    return os.getenv("REDIS_HOST")

def get_redis_port():
    return os.getenv("REDIS_PORT")

def get_redis_ttl():
    return os.getenv("REDIS_TTL")


def get_regions_file_path():
    return str(Path(__file__).parent / "regions.geojson")
