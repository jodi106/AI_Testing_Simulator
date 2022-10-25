class BuildEvent
/// Class to Build Events. Uses BuildAction and BuildTrigger
{

}

class BuildTrigger
{

    /// class to create a trigger with a condtition group containing one or multiple conditions. Uses Conditions
    
    /*
              <StartTrigger OR StopTrigger>
                <ConditionGroup>
                  <Condition name="AfterAdversaryAcceleratesAgain" delay="0" conditionEdge="rising">

                    /// space for condition, use class Conditions

                  </Condition>
                  IF MORE THAN ONE CONTIDITION ADD ANOTHER CONDITION BLOCK
                </ConditionGroup>
              </StartTrigger>    
    
    */

}

class Conditions
/// Class to create conditions for Start and Stop Triggers. Used in BuildTrigger class
{
    public string name;
    public int delay;
    public string edge;
    public string xmlBrick;

    public Conditions(string conditionName)
    {
        name = conditionName;
        delay = 0;
        edge = "rising";
        xmlBrick = ///   <Condition name="AfterAdversaryAcceleratesAgain" delay="0" conditionEdge="rising">
                   ///   </Condition>
    }
    public void ByValueCondition(string ValueCondition, dict args)
    {
        /* All Value Conditions
        <!-- parameterCondition -->
        <!-- timeOfDayCondition -->
        <!-- IMPLEMENTED simulationTimeCondition params: value(float), rule (enum(less, greater, equal))-->
        <!-- IMPLEMENTED storyboardElementStateCondition params: storyboardElementType(enum(6 options)), storyboardElementRef(string), state (enum(7 options))-->
        <!-- userDefinedValueCondition -->
        <!-- trafficSignalCondition -->
        <!-- trafficSignalControllerCondition -->
        */
        
        xmlBrick.append/*
        <ByValueCondition>
            
            //Space for value condition

        </ByValueCondition>
        */

        if (ValueCondition == "StoryboardElementStateCondition")
        {
            xmlBrick.append
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



    }

    public void ByEntityCondition(string EntityRef, string EntityCondition, dict args)
    {
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
        <!-- traveledDistanceCondition params-->
        <!-- IMPLEMETED reachPositionCondition params: tolerance(float), WorldPosition (WorldPosition CREATE STRING FROM X,Y,Z,H)  -->
        <!-- distanceCondition params-->
        <!-- relativeDistanceCondition-->
        */

        xmlBrick.append
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





    }
}

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


class BuildAction
/// Class to build actions
{

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