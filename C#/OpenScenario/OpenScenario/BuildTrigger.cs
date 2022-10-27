using System;
using System.Xml;

class BuildTrigger
{
    public string xmlBLock { get; set;}

    public BuildTrigger()
    {

    }

    public void CombineTrigger()
    /// Combines Trigger xmlBlock - if required - with multiple condtitions in a condition group 
    {
        ///if StartTrigger{xml = <StartTrigger></StartTrigger><ConditionGroup></ConditionGroup>}

        ///if StopStrigger{xml = <StopTrigger></StopTrigger><ConditionGroup></ConditionGroup> }
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
        
        //xmlBrick.append
        /*
        <ByValueCondition>
            
            //Space for value condition

        </ByValueCondition>
        */

        if (ValueCondition == "StoryboardElementStateCondition")
        {
            //xmlBrick.append
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

        /// xmlBrick.append
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