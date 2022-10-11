def accelerate_all_simulation_cars(root, storyboard, number_of_sim_cars, speed, start_after_x_sec, stop_after_x_sec):
    story = root.createElement('Story')
    story.setAttribute('name', 'MyStory')
    act = root.createElement('Act')
    act.setAttribute('name', 'Behavior')
    maneuver_group = root.createElement('ManeuverGroup')
    maneuver_group.setAttribute('maximumExecutionCount', '1')
    maneuver_group.setAttribute('name', 'ManeuverSequence')
    actors = root.createElement('Actors')
    actors.setAttribute('selectTriggeringEntities', 'false')
    for n in range(0, number_of_sim_cars): # which vehicles are affected (all sim. cars)
        entity_ref = root.createElement('EntityRef')
        entity_ref.setAttribute('entityRef', 'adversary'+str(n))
        actors.appendChild(entity_ref)
    maneuver = root.createElement('Maneuver')
    maneuver.setAttribute('name', 'AccelerateManeuver')

    # What this maneuver is doing --> Accelerate cars
    event = root.createElement('Event')
    event.setAttribute('name', 'AdversaryAccelerates')
    event.setAttribute('priority', 'overwrite')
    action = root.createElement('Action')
    action.setAttribute('name', 'AdversaryAccelerates')
    private_action = root.createElement('PrivateAction')
    longitudinal_action = root.createElement('LongitudinalAction')
    speed_action = root.createElement('SpeedAction')
    speed_action_dynamics = root.createElement('SpeedActionDynamics')
    speed_action_dynamics.setAttribute('dynamicsShape', 'step')
    speed_action_dynamics.setAttribute('value', '50')
    speed_action_dynamics.setAttribute('dynamicsDimension', 'distance')
    speed_action_target = root.createElement('SpeedActionTarget')
    absolute_target_speed = root.createElement('AbsoluteTargetSpeed')
    absolute_target_speed.setAttribute('value', str(speed))
    
    # Start condition
    start_trigger = root.createElement('StartTrigger')
    accelerate_condition(root, start_trigger, "StartTime", start_after_x_sec)

    # Stop condition
    stop_trigger = root.createElement('StopTrigger')
    accelerate_condition(root, stop_trigger, "StopTime", stop_after_x_sec)
    
    # Hierarchy
    storyboard.appendChild(story)
    story.appendChild(act)
    act.appendChild(maneuver_group)
    maneuver_group.appendChild(actors)
    # for n in range(0, number_of_sim_cars):
    #     actors.appendChild(entity_ref[n])
    maneuver_group.appendChild(maneuver)
    maneuver.appendChild(event)
    event.appendChild(action)
    action.appendChild(private_action)
    private_action.appendChild(longitudinal_action)
    longitudinal_action.appendChild(speed_action)
    speed_action.appendChild(speed_action_dynamics)
    speed_action.appendChild(speed_action_target)
    speed_action_target.appendChild(absolute_target_speed)
    event.appendChild(start_trigger)
    act.appendChild(root.createElement('StartTrigger'))
    act.appendChild(stop_trigger)
    storyboard.appendChild(root.createElement('StopTrigger'))

def accelerate_condition(root, trigger, name, time_in_sec):
    condition_group = root.createElement('ConditionGroup')
    condition = root.createElement('Condition')
    condition.setAttribute('name', str(name))
    condition.setAttribute('delay', '0')
    condition.setAttribute('conditionEdge', 'rising')
    by_value_condition = root.createElement('ByValueCondition')
    simulation_time_condition = root.createElement('SimulationTimeCondition')
    simulation_time_condition.setAttribute('value', str(time_in_sec))
    simulation_time_condition.setAttribute('rule', 'greaterThan')

    trigger.appendChild(condition_group)
    condition_group.appendChild(condition)
    condition.appendChild(by_value_condition)
    by_value_condition.appendChild(simulation_time_condition)

    

