using netLogic.Shared;
using netLogic.Terrain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace netLogic.Common
{
    public class GliderProfile
    {
        // Constructor. Needs filename to create object.
        public GliderProfile(String filename)
        {
            Parse(filename);
        }

        // Override Default ToString()
        public override String ToString()
        {
            StringBuilder s = new StringBuilder();

            s.Append("-= Waypoints =-\n");
            foreach(Coordinate item in waypoints)
                s.Append(item.ToString() + "\n");

            s.Append("EOF\n");
           
            return s.ToString();
        }

        // Returns the closest Waypoint Coordinate, based on bots current Location Coordinate
        public Coordinate ClosestWaypoint(Coordinate current)
        {
            float shortestDistance = 0.0f;
            Coordinate closestWaypoint = new Coordinate(0f, 0f, 0f); // <- shut compiler up

            foreach (Coordinate waypoint in waypoints)
            {
                Vect3D v = new Vect3D(current, waypoint);

                if (shortestDistance == 0.0f)
                {
                    shortestDistance = v.LengthFast;
                    closestWaypoint = waypoint;
                }
                else if (v.LengthFast < shortestDistance)
                {
                    shortestDistance = v.LengthFast;
                    closestWaypoint = waypoint;
                }
            }

            return closestWaypoint;
        }

        // Returns the entire List of Waypoints
        public List<Coordinate> Waypoints
        {
            get { return waypoints; }
        }

        // Reads and Parses the Wowglider Waypoint XML file
        private void Parse(String filename)
        {
            XmlTextReader reader = new XmlTextReader(filename);
            String element = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        element = reader.Name;
                        break;

                    case XmlNodeType.Text:
                        if (element.Equals("Waypoint"))
                            waypoints.Add(new Coordinate(reader.Value));
                        break;
                }
            }
        }

        private List<Coordinate> waypoints = new List<Coordinate>();
    }
}