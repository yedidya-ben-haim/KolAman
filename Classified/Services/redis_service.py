import redis


class redis_service:
    def __init__(self, host="localhost", port=6379, ttl = 3600):
        self.client = redis.Redis(host=host, port=port, decode_responses=True)
        self.ttl = ttl





    def sent_alert(self, alert: dict):
        key = alert.get("alert_id")

        result = self.client.set(key, "1", nx=True, ex=self.ttl)

        return result is True
