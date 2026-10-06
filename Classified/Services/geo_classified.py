import geopandas as gpd
from shapely.geometry import Point


class geo_classified:
    def __init__(self, file_path: str):
        self.gdf = gpd.read_file(file_path)

    def get_region(self, lon: float, lat: float) -> str:
        pt = Point(lon, lat)

        matched =  self.gdf[self.gdf.geometry.contains(pt)]

        if not matched.empty:
            return matched.iloc[0]["region"]
        return "DEPTH"
