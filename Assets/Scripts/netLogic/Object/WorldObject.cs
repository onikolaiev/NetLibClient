using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netLogic;
using UnityEngine;
using netLogic.Constants;
using netLogic.Shared;
using System.IO;

namespace netLogic
{
    public class WorldObject : Object
    {
        private UInt16 _map;











        public WorldObject()
        {
            _depleted = false;
            _map = 0;
        }
        public void SetCoordinates(Vector3 l)
        {
            transform.position = l;
        }
        public void SetCoordinates(Vector3 l, Quaternion q)
        {
                transform.position = l;
                transform.rotation = q;
        }
        public Vector3 GetCoordinates()
        {
                return transform.position;
            
        }

        public void SetPositionX(float x)
        {
                transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        public void SetPositionY(float y)
        {
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        public void SetPositionZ(float z)
        {
                transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
        public float GetPositionX()
        {
                return transform.position.x;
        }
        public float GetPositionY()
        {
                return transform.position.y;
        }
        public float GetPositionZ()
        {
                return transform.position.z;
        }

        public Quaternion GetRotation()
        {
                return transform.rotation;
        }
        public void SetRotation(Quaternion q)
        {
                transform.rotation=q;
        }

        public float CalculateAngle(float x1, float y1)
        {
                return CalculateAngle(transform.position.x, transform.position.y, x1, y1);
        }
        // Credit to ascent - I'm lazy :P
        public float CalculateAngle(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            double angle = 0.0f;

            // Calculate angle
            if (dx == 0.0)
            {
                if (dy == 0.0)
                    angle = 0.0;
                else if (dy > 0.0)
                    angle = Math.PI * 0.5;
                else
                    angle = Math.PI * 3.0 * 0.5;
            }
            else if (dy == 0.0)
            {
                if (dx > 0.0)
                    angle = 0.0;
                else
                    angle = Math.PI;
            }
            else
            {
                if (dx < 0.0)
                    angle = Math.Atan(dy / dx) + Math.PI;
                else if (dy < 0.0)
                    angle = Math.Atan(dy / dx) + (2 * Math.PI);
                else
                    angle = Math.Atan(dy / dx);
            }

            return (float)angle;
        }
        
        public float GetDistance(WorldObject _obj)
        {
            
            float dx = GetPositionX() - _obj.GetPositionX();
            float dy = GetPositionY() - _obj.GetPositionY();
            float dz = GetPositionZ() - _obj.GetPositionZ();
           // float sizefactor = GetObjectSize(); //TODO
           // float dist = sqrt((dx * dx) + (dy * dy)) - sizefactor;
            float dist = (float)Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
            return (dist > 0 ? dist : 0);
        }
        public float GetDistance2d(WorldObject _obj)
        {
            float dx = GetPositionX() - _obj.GetPositionX();
            float dy = GetPositionY() - _obj.GetPositionY();
            // float sizefactor = GetObjectSize(); //TODO
            // float dist = sqrt((dx * dx) + (dy * dy)) - sizefactor;
            float dist = (float)Math.Sqrt((dx * dx) + (dy * dy));
            return (dist > 0 ? dist : 0);
        }
        public float GetDistance(Vector3 v)
        {
            float dx = GetPositionX() - v.x;
            float dy = GetPositionY() - v.y;
            // float sizefactor = GetObjectSize(); //TODO
            // float dist = sqrt((dx * dx) + (dy * dy)) - sizefactor;
            float dist = (float)Math.Sqrt((dx * dx) + (dy * dy));
            return (dist > 0 ? dist : 0);
        }
        public float GetDistance2d(float x, float y)
        {
            float dx = GetPositionX() - x;
            float dy = GetPositionY() - y;
            // float sizefactor = GetObjectSize(); //TODO
            // float dist = sqrt((dx * dx) + (dy * dy)) - sizefactor;
            float dist = (float)Math.Sqrt((dx * dx) + (dy * dy));
            return (dist > 0 ? dist : 0);
        }

    }
}
