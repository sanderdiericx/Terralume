using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Terralume.Engine.Hooks.Tracking
{
    internal static class SunLightTracker
    {
        private static readonly float MAX_INTENSITY = 4f;
        private static readonly float MIN_INTENSITY = 2f;
        private static readonly float AVERAGE_INTENSITY = 3f;

        public static Vector2 GetSunDirection()
        {
            double timePercent = Main.time / (Main.dayTime ? Main.dayLength : Main.nightLength);
            float angle = (float)(Math.PI - (timePercent * Math.PI));
            return Vector2.Normalize(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
        }

        public static Vector3 GetSunColor()
        {
            double timePercent = Main.time / (Main.dayTime ? Main.dayLength : Main.nightLength);
            float t = MathF.Sin((float)timePercent * MathF.PI);
            float sunIntensity = Main.dayTime ? float.Lerp(AVERAGE_INTENSITY, MAX_INTENSITY, t) : float.Lerp(AVERAGE_INTENSITY, MIN_INTENSITY, t);
            return Main.ColorOfTheSkies.ToVector3() * sunIntensity;
        }
    }
}
