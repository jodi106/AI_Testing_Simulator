import pandas as pd

def get_number_of_cars_without_ego():
    df = pd.read_csv("scripts\\scenario_editor\\data\\demo_3Cars.txt")  
    vehicle_list, passenger_list = get_coord_simple(df)
    print(len(vehicle_list)-1)
    return len(vehicle_list)-1

def get_vehicle_list():
    df = pd.read_csv("scripts\\scenario_editor\\data\\demo_3Cars.txt")  
    vehicle_list, passenger_list = get_coord_simple(df)
    return vehicle_list

def get_coord_simple(df):
    cars = df[df["EntityType"]=="C"][["xCoord","yCoord","zCoord", "rotationDeg"]]
    passenger = df[df["EntityType"]=="P"][["xCoord","yCoord","zCoord","rotationDeg"]]
    coord_cars, coord_pass = [],[]
    for x in range(len(cars)):
        coord_cars.append(list(cars.iloc[x]))
    for x in range(len(passenger)):
        coord_pass.append(list(passenger.iloc[x]))
    return coord_cars, coord_pass

# Driver Code
if __name__ == "__main__": 
    get_number_of_cars_without_ego()
    print("finished!")


