
def add_storyboard(root, open_scenario, number_of_simulation_cars, x, y, z, h, control_mode):
    # Add tags
    storyboard = root.createElement('Storyboard')
    init = root.createElement('Init')
    actions = root.createElement('Actions')
    add_actions(root, actions, number_of_simulation_cars, x, y, z, h, control_mode)

    # Hierarchy
    open_scenario.appendChild(storyboard)
    storyboard.appendChild(init)
    init.appendChild(actions)


def add_actions(root, actions, number_of_simulation_cars, x, y, z, h, control_mode):
    # Spawn ego vehicle at requested coordinates
    spawn_vehicle_ego(root, actions, x[0], y[0], z[0], h[0], control_mode)

    # Spawn simulation vehicles at requested coordinates
    for n in range(0, number_of_simulation_cars):
        spawn_vehicles_simulation(root, actions, n, x[n+1], y[n+1], z[n+1], h[n+1])

    # Hierarchy
    pass


def spawn_vehicles_simulation(root, actions, n, x, y, z, h):
    private = root.createElement('Private')
    private.setAttribute('entityRef', 'adversary'+str(n))
    private_action = root.createElement('PrivateAction')
    teleport_action = root.createElement('TeleportAction')
    position = root.createElement('Position')
    world_position = root.createElement('WorldPosition')
    world_position.setAttribute('x', x)
    world_position.setAttribute('y', y)
    world_position.setAttribute('z', z)
    world_position.setAttribute('h', h)

    # Hierarchy
    actions.appendChild(private)
    private.appendChild(private_action)
    private_action.appendChild(teleport_action)
    teleport_action.appendChild(position)
    position.appendChild(world_position)


def spawn_vehicle_ego(root, actions, x, y, z, h, control_mode):
    private = root.createElement('Private')
    private.setAttribute('entityRef', 'hero')
    private_action1 = root.createElement('PrivateAction')
    teleport_action = root.createElement('TeleportAction')
    position = root.createElement('Position')
    world_position = root.createElement('WorldPosition')
    world_position.setAttribute('x', x)
    world_position.setAttribute('y', y)
    world_position.setAttribute('z', z)
    world_position.setAttribute('h', h)

    private_action2 = root.createElement('PrivateAction')
    controller_action = root.createElement('ControllerAction')
    assign_controller_action = root.createElement('AssignControllerAction')
    controller = root.createElement('Controller')
    controller.setAttribute('name', 'HeroAgent')
    properties = root.createElement('Properties')
    property = root.createElement('Property')
    property.setAttribute('name', 'module')
    #property.setAttribute('value', 'carla_auto_pilot_control')
    #property.setAttribute('value', 'external_control')
    property.setAttribute('value', control_mode)
    override_controller_value_action = root.createElement('OverrideControllerValueAction')
    throttle = root.createElement('Throttle')
    throttle.setAttribute('value', '0')
    throttle.setAttribute('active', 'false')
    brake = root.createElement('Brake')
    brake.setAttribute('value', '0')
    brake.setAttribute('active', 'false')
    clutch = root.createElement('Clutch')
    clutch.setAttribute('value', '0')
    clutch.setAttribute('active', 'false')
    parking_brake = root.createElement('ParkingBrake')
    parking_brake.setAttribute('value', '0')
    parking_brake.setAttribute('active', 'false')
    steering_wheel = root.createElement('SteeringWheel')
    steering_wheel.setAttribute('value', '0')
    steering_wheel.setAttribute('active', 'false')
    gear = root.createElement('Gear')
    gear.setAttribute('number', '0')
    gear.setAttribute('active', 'false')

    # Hierarchy
    actions.appendChild(private)
    private.appendChild(private_action1)
    private_action1.appendChild(teleport_action)
    teleport_action.appendChild(position)
    position.appendChild(world_position)
    private.appendChild(private_action2)
    private_action2.appendChild(controller_action)
    controller_action.appendChild(assign_controller_action)
    assign_controller_action.appendChild(controller)
    controller.appendChild(properties)
    properties.appendChild(property)
    controller_action.appendChild(override_controller_value_action)
    override_controller_value_action.appendChild(throttle)
    override_controller_value_action.appendChild(brake)
    override_controller_value_action.appendChild(clutch)
    override_controller_value_action.appendChild(parking_brake)
    override_controller_value_action.appendChild(steering_wheel)
    override_controller_value_action.appendChild(gear)
    

