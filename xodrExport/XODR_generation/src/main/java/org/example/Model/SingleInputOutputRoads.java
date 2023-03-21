package org.example.Model;

import java.rmi.server.UID;

public class SingleInputOutputRoads {
    private UID inputRoadUID;
    private UID outputRoadUID;

    public UID getInputRoadUID() {
        return inputRoadUID;
    }

    public void setInputRoadUID(UID inputRoadUID) {
        this.inputRoadUID = inputRoadUID;
    }

    public UID getOutputRoadUID() {
        return outputRoadUID;
    }

    public void setOutputRoadUID(UID outputRoadUID) {
        this.outputRoadUID = outputRoadUID;
    }
}
