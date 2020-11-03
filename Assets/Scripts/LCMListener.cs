﻿using System.Collections;
using System.Collections.Generic;
using LCM;
using LCM.LCM;
using UnityEngine;

public class LCMListener : MonoBehaviour
{   
    private static LCMListener instance;

    //mini_rov_attitude_t
    private double roll_deg;
    public double Roll {get {return roll_deg;}}

    private double pitch_deg;
    public double Pitch {get {return pitch_deg;}}

    private double yaw_deg;
    public double Yaw {get {return yaw_deg;}}

    //mini_rov_depth_t
    private double depth;
    public double Depth {get {return depth;}}

    private double pressure;
    public double Pressure {get {return pressure;}}

    private double rope_length;
    public double RopeLength {get {return rope_length;}}

    private double rov_lon;
    public double ROVLon {get {return rov_lon;}}

    private double rov_lat;
    public double ROVLat {get {return rov_lat;}} 

    private double ship_lon;
    public double ShipLon {get {return ship_lon;}}

    private double ship_lat;
    public double ShipLat {get {return ship_lat;}} 

    LCM.LCM.LCM myLCM;


    // Start is called before the first frame update
    void Start()
    { 
        instance = this;

        myLCM = new LCM.LCM.LCM();

        myLCM.SubscribeAll(new SimpleSubscriber());

        StartCoroutine(SimulateRopeLength(3, 4, .01d));

    }

    class SimpleSubscriber : LCM.LCM.LCMSubscriber
    {
        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCM.LCM.LCMDataInputStream dins)
        {
            if (channel == "MINIROV_ATTITUDE")
            {
                mwt.mini_rov_attitude_t msg = new mwt.mini_rov_attitude_t(dins);
                instance.roll_deg = msg.roll_deg;
                instance.pitch_deg = msg.pitch_deg;
                instance.yaw_deg = msg.yaw_deg;

            }
            else if (channel == "MINIROV_DEPTH")
            {
                mwt.mini_rov_depth_t msg = new mwt.mini_rov_depth_t(dins);
                instance.depth = msg.depth;
                instance.pressure = msg.pressure;
            }
            else if (channel == "MWT_STEREO_IMAGE")
            {
                mwt.stereo_image_t msg = new mwt.stereo_image_t(dins);
            }
            else if (channel == "MINIROV_ROWE_DVL")
            {
                mwt.mini_rov_rowe_dvl_t msg = new mwt.mini_rov_rowe_dvl_t(dins);
            }
            else if (channel == "GEOLOCATION")
            {
                mwt.geolocation_t msg = new mwt.geolocation_t(dins);
                instance.rov_lon = msg.rov_lon;
                instance.rov_lat = msg.rov_lat;
                instance.ship_lat = msg.ship_lat;
                instance.ship_lon = msg.ship_lon;
            }
            else
            {
                // Debug.Log("Unknown Channel: " + channel);
            }
        }
    }

    IEnumerator SimulateRopeLength(double min, double max, double step) 
    {   while (true) {
            for (double f = min; f <  max; f += step) 
            {
                rope_length = f;
                yield return new WaitForSeconds(.2f);
            }
            for (double f = max; f > min; f -= step) 
            {
                rope_length = f;
                yield return new WaitForSeconds(.2f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}