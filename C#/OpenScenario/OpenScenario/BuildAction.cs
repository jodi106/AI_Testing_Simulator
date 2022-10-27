using System;
using System.Xml;

class BuildAction
{
    public string xmlBLock { get; set;}
    public string Name { get; set;}

    public BuildAction(string name)
    // Constructor
    {
        Name = name;
        ///xmlBlock = <Action name="REFERENZ_NAME"><PrivateAction> </PrivateAction></Action>
    }

    public void CombineAction() {}
    /// ---Probably not necessary as Action only has one xmlBlock---
    
    public void AcquirePositionAction()
    /// Creates AcquirePositionAction
    {
        /*
                <RoutingAction>
                <AcquirePositionAction>
                    <Position>
                    <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                    </Position>
                </AcquirePositionAction>
                </RoutingAction>
        */
    }  

    public void AssignRouteAction(List<string> Waypoints, string routeStrategy)
    /// Creates AssignRouteAction
    {
        /*
        <RoutingAction>
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
        </RoutingAction>
        */
    }

    public void SpeedAction(string dynamicShape, float value, string dynamicDimension, float AbsoluteTargetSpeed)
    /// Creates Speed Action
    {
        /*
        <LongitudinalAction>
        <SpeedAction>
            <SpeedActionDynamics dynamicsShape="step" value="10" dynamicsDimension="time"/>
            <SpeedActionTarget>
                <AbsoluteTargetSpeed value="0.0"/>
            </SpeedActionTarget>
        </SpeedAction>
        </LongitudinalAction>        
        */
    }

    public void LaneChangeAction(string dynamicsShape, float value, string dynamicsDimension, string entityRef, int value2)
    /// Creates LaneChangeAction
    {
        /*
        <LateralAction>
        <LaneChangeAction>
            <LaneChangeActionDynamics dynamicsShape="linear" value="25" dynamicsDimension="distance"/>
            <LaneChangeTarget>
                <RelativeTargetLane entityRef="adversary" value="-1"/>
            </LaneChangeTarget>
        </LaneChangeAction>
        </LateralAction>
        */
    }    
}