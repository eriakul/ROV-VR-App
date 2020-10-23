/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace mwt
{
    public sealed class geolocation_t : LCM.LCM.LCMEncodable
    {
        public double rov_lon;
        public double rov_lat;
        public double ship_lon;
        public double ship_lat;
 
        public geolocation_t()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0xa031e35428e1c3eeL;
 
        static geolocation_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("mwt.geolocation_t"))
                return 0L;
 
            classes.Add("mwt.geolocation_t");
            ulong hash = LCM_FINGERPRINT_BASE
                ;
            classes.RemoveAt(classes.Count - 1);
            return (hash<<1) + ((hash>>63)&1);
        }
 
        public void Encode(LCMDataOutputStream outs)
        {
            outs.Write((long) LCM_FINGERPRINT);
            _encodeRecursive(outs);
        }
 
        public void _encodeRecursive(LCMDataOutputStream outs)
        {
            outs.Write(this.rov_lon); 
 
            outs.Write(this.rov_lat); 
 
            outs.Write(this.ship_lon); 
 
            outs.Write(this.ship_lat); 
 
        }
 
        public geolocation_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public geolocation_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static mwt.geolocation_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            mwt.geolocation_t o = new mwt.geolocation_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.rov_lon = ins.ReadDouble();
 
            this.rov_lat = ins.ReadDouble();
 
            this.ship_lon = ins.ReadDouble();
 
            this.ship_lat = ins.ReadDouble();
 
        }
 
        public mwt.geolocation_t Copy()
        {
            mwt.geolocation_t outobj = new mwt.geolocation_t();
            outobj.rov_lon = this.rov_lon;
 
            outobj.rov_lat = this.rov_lat;
 
            outobj.ship_lon = this.ship_lon;
 
            outobj.ship_lat = this.ship_lat;
 
            return outobj;
        }
    }
}
