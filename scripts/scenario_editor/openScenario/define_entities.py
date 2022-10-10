
## all .xosc files contain these tags
def add_basic_tags(root, open_scenario, map):
    # add tags
    file_header = root.createElement('FileHeader')
    file_header.setAttribute('revMajor', '1')
    file_header.setAttribute('revMinor', '0')
    file_header.setAttribute('date', '')
    file_header.setAttribute('description', '')
    file_header.setAttribute('author', '')
    parameter_declarations = root.createElement('ParameterDeclarations')
    catalog_locations = root.createElement('CatalogLocations')
    road_network = root.createElement('RoadNetwork')
    logic_file = root.createElement('LogicFile')
    logic_file.setAttribute('filepath', map)
    scene_graph_file= root.createElement('SceneGraphFile')
    scene_graph_file.setAttribute('filepath', '')

    # hierarchy
    root.appendChild(open_scenario)
    open_scenario.appendChild(file_header)
    open_scenario.appendChild(parameter_declarations)
    open_scenario.appendChild(catalog_locations)
    open_scenario.appendChild(road_network)
    road_network.appendChild(logic_file)
    road_network.appendChild(scene_graph_file)


def add_entities(root, open_scenario, number_of_simulation_cars, vehicle_model_ego, vehicle_model_sim):
    # Add tags
    entities = root.createElement('Entities')
    # ego-vehicle
    add_vehicle(root, entities, "hero", vehicle_model_ego, "ego_vehicle")
    # other vehicles
    for n in range(0, number_of_simulation_cars):
        add_vehicle(root, entities, "adversary"+str(n), vehicle_model_sim, "simulation")

    # Hierarchy
    open_scenario.appendChild(entities)
    

def add_vehicle(root, entities, sc_name, vehicle_model, value):
    scenario_object = root.createElement('ScenarioObject')
    scenario_object.setAttribute('name', sc_name)
    vehicle = root.createElement('Vehicle')
    vehicle.setAttribute('name', vehicle_model)
    vehicle.setAttribute('vehicleCategory', 'car')
    parameter_declarations = root.createElement('ParameterDeclarations')
    performance = root.createElement('Performance')
    performance.setAttribute('maxSpeed', '69.444')
    performance.setAttribute('maxAcceleration', '200')
    performance.setAttribute('maxDeceleration', '10.0')
    bounding_box = root.createElement('BoundingBox')
    center = root.createElement('Center')
    center.setAttribute('x', '1.5')
    center.setAttribute('y', '0.0')
    center.setAttribute('z', '0.9')
    dimensions = root.createElement('Dimensions')
    dimensions.setAttribute('width', '2.1')
    dimensions.setAttribute('length', '4.5')
    dimensions.setAttribute('height', '1.8')
    axles = root.createElement('Axles')
    front_axle = root.createElement('FrontAxle')
    front_axle.setAttribute('maxSteering', '0.5')
    front_axle.setAttribute('wheelDiameter', '0.6')
    front_axle.setAttribute('trackWidth', '1.8')
    front_axle.setAttribute('positionX', '3.1')
    front_axle.setAttribute('positionZ', '0.3')
    rear_axle = root.createElement('RearAxle')
    rear_axle.setAttribute('maxSteering', '0.0')
    rear_axle.setAttribute('wheelDiameter', '0.6')
    rear_axle.setAttribute('trackWidth', '1.8')
    rear_axle.setAttribute('positionX', '0.0')
    rear_axle.setAttribute('positionZ', '0.3')
    properties = root.createElement('Properties')
    property = root.createElement('Property')
    property.setAttribute('name', 'type')
    property.setAttribute('value', value)

    # Hierarchy
    entities.appendChild(scenario_object)
    scenario_object.appendChild(vehicle)
    vehicle.appendChild(parameter_declarations)
    vehicle.appendChild(performance)
    vehicle.appendChild(bounding_box)
    bounding_box.appendChild(center)
    bounding_box.appendChild(dimensions)
    vehicle.appendChild(axles)
    axles.appendChild(front_axle)
    axles.appendChild(rear_axle)
    vehicle.appendChild(properties)
    properties.appendChild(property)

