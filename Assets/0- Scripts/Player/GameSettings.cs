using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {
   public float _cameraSensitivity = 1f;
   public bool _invertPitch = false;
   public bool _invertYaw = false;
   public bool _showUIControls = true;

   public float CameraSensitivity { get => _cameraSensitivity; set => _cameraSensitivity = value; }
   public bool InvertPitch { get => _invertPitch; set => _invertPitch = value; }
   public bool InvertYaw { get => _invertYaw; set => _invertYaw = value; }
   public bool ShowUIControls { get => _showUIControls; set => _showUIControls = value; }
}
