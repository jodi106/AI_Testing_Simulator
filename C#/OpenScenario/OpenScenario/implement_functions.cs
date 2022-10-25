class WorldSettings
{
    public void create_world_settings(XmlNode Actions, TimeOfDay, ...)
    {
        /*
        <GlobalAction>
          <EnvironmentAction>
            <Environment name="Environment1">
              <TimeOfDay animation="false" dateTime="2019-06-25T12:00:00"/>
              <Weather cloudState="free">
                <Sun intensity="0.85" azimuth="0" elevation="1.31"/>
                <Fog visualRange="100000.0"/>
                <Precipitation precipitationType="dry" intensity="0.0"/>
              </Weather>
              <RoadCondition frictionScaleFactor="1.0"/>
            </Environment>
          </EnvironmentAction>
        </GlobalAction>
        */
    }

}


class RoutingAction
{
    public void AcquirePositionAction(WorldPosition)
    {
        /*
        <Action name="RouteCreation">
            <PrivateAction>
                <RoutingAction>
                <AcquirePositionAction>
                    <Position>
                    <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                    </Position>
                </AcquirePositionAction>
                </RoutingAction>
            </PrivateAction>
        </Action>
        */
    }

    public void AssignRouteAction(list Waypoints, string routeStrategy)
    {
        /*
        <AssignRouteAction>
            <Route name = "TestRoute" closed = "false">
                <Waypoint routeStrategy = "fastest">
                    <Position>
                    <WorldPosition x="412" y="-100" z="0.3" h="29.9"/>
                    </Position>
                </Waypoint>
                <Waypoint routeStrategy = "fastest">
                    <Position>
                    <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                    </Position>
                </Waypoint>					  
                <Waypoint routeStrategy = "fastest">
                    <Position>
                    <WorldPosition x="412" y="-200" z="0.3" h="29.9"/>
                    </Position>
                </Waypoint>	
                <Waypoint routeStrategy = "fastest">
                    <Position>
                    <WorldPosition x="390" y="-300" z="0.3" h="29.9"/>
                    </Position>
                </Waypoint>
            </Route>
        </AssignRouteAction>
        */
    }

}

class LongitudinalAction
{
    public void SpeedAction(string dynamicShape, float value, string dynamicDimension, float AbsoluteTargetSpeed)
    {
        /*
        <SpeedAction>
            <SpeedActionDynamics dynamicsShape="step" value="10" dynamicsDimension="time"/>
            <SpeedActionTarget>
                <AbsoluteTargetSpeed value="0.0"/>
            </SpeedActionTarget>
        </SpeedAction>        
        */
    }
}

class LateralAction
{
    LaneChangeAction(string dynamicsShape, float value, string dynamicsDimension, string entityRef, int value)
    {
        /*
        <LaneChangeAction>
            <LaneChangeActionDynamics dynamicsShape="linear" value="25" dynamicsDimension="distance"/>
            <LaneChangeTarget>
                <RelativeTargetLane entityRef="adversary" value="-1"/>
            </LaneChangeTarget>
        </LaneChangeAction>
        */
    }
}

class Trigger
{
    public void ByValueCondition(string ValueCondition, dict args)
    {
        
        if (ValueCondition == "StoryboardElementStateCondition")
        {
            /*
            <StoryboardElementStateCondition storyboardElementType="action" storyboardElementRef="STORY_BOARD_ELEMENT_NAME" state="completeState"/>
            */
        }

        if (ValueCondition == "SimulationTimeCondition")
        {
            /*
            <SimulationTimeCondition value="2.0" rule="greaterThan"/>
            */
        }
        /* All Value Conditions
        <!-- parameterCondition -->
        <!-- timeOfDayCondition -->
        <!-- simulationTimeCondition -->
        <!-- storyboardElementStateCondition -->
        <!-- userDefinedValueCondition -->
        <!-- trafficSignalCondition -->
        <!-- trafficSignalControllerCondition -->
        */

        /*
        <ByValueCondition>
            
            //Space for value condition

        </ByValueCondition>
        */
    }

    public void ByEntityCondition(string EntityRef, string EntityCondition, dict args)
    {
        if (EntityCondition == "ReachPositionCondition")
        {
            /*
            <ReachPositionCondition tolerance="2.0">
                <Position>
                <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                </Position>
            </ReachPositionCondition>
            */
        }

        if (EntityCondition == "DistanceCondition")
        {
            /*

            */
        }

        /* All EntityConditions
        <!-- endOfRoadCondition-->
        <!-- collisionCondition-->
        <!-- offroadCondition-->
        <!-- timeHeadwayCondition-->
        <!-- timeToCollisionCondition-->
        <!-- accelerationCondition-->
        <!-- standStillCondition-->
        <!-- speedCondition-->
        <!-- relativeSpeedCondition-->
        <!-- traveledDistanceCondition-->
        <!-- reachPositionCondition-->
        <!-- distanceCondition-->
        <!-- relativeDistanceCondition-->
        */


        /*
        <ByEntityCondition>
            <TriggeringEntities triggeringEntitiesRule="any">
                <EntityRef entityRef="adversary0"/>
            </TriggeringEntities>
                <EntityCondition>

                    //Space for entity condition

                </EntityCondition>
        </ByEntityCondition>
        */
    }
}

