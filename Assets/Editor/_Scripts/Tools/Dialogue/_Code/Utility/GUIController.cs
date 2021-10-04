using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ETools.Editor.Dialogue
{

    public static class GUIController
    {
        public static Vector2 lookPos;
        public static Vector2 lookPosTarget;
        public static float lookPosSpeed = 0.1f;

        public static Node backwardsConnectionNode = null;
        public static bool drawingDeleteLine = false;
        public static Vector2 deleteLineStart;
        public static GenericMenu activeContextMenu = null;
        public static Connectable connectionNode;

        public static float zoom = 1f;
        public static float zoomTarget = 1f;
        public static float zoomSpeed = 0.03f;

        public static bool drawingMarquee = false;
        public static Vector2 marqueeSelectPos;

        public static float scrollSensitivity = 0.9f;
        public static float zoomMax = 3f;
        public static float zoomMin = 0.3f;

        public static List<Node> nodeClipboard = new List<Node>();

        public static List<System.Type> NodesAvailable
        {
            get
            {
                return new List<Type>(from t in Assembly.GetExecutingAssembly().GetTypes() where t.GetCustomAttribute<GraphNode>() != null select t);
            }
        }

        public static bool WantsConnection
        {
            get
            {
                return GUIController.connectionNode != null;
            }
        }

        public static bool WantsBackwardsConnection
        {
            get
            {
                return GUIController.backwardsConnectionNode != null;
            }
        }

        public static Vector2 ToDataSpace(Vector2 point)
        {
            return new Vector2(point.x / GUIController.zoom - GUIController.lookPos.x, point.y / GUIController.zoom - GUIController.lookPos.y);
        }
    }
}