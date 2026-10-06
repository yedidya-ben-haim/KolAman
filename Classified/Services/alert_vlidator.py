def validate_alert(alert: dict):
    if not alert.get("alert_id"):
        return False

    if not alert.get("content"):
        return False

    if not alert.get("lat"):
        return False

    if not alert.get("lon"):
        return False

    if not isinstance(alert.get("lat"), (int, float)):
        return False

    if not isinstance(alert.get("lon"), (int, float)):
            return False

    if not -90 < alert.get("lat") < 90:
        return False

    if not -180 < alert.get("lon") < 180:
        return False

    return True





