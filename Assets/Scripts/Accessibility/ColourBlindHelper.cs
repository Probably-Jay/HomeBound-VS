using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accessibility
{
    public enum ColourBlindMode 
    { 
        Trichromacy // full colour
        , Deuteranopia // red-green colourblind (green light) 
        , Protanopia // red-green colourblind (red light)
        , Tritanopia // blue-yellow colourblind (blue light)
    }

    //public enum ColourName
    //{ 
    //        white
    //        ,black
    //        ,lightGray
    //        ,gray
    //        ,darkGray
    //        ,red
    //        ,green
    //        ,blue
    //        ,cyan
    //        ,magenta
    //        ,yellow
    //        ,orange
    //        ,pink

    //}


    public static class ColourBlindHelper
    {
        public static ColourBlindMode Mode { get; set; }


        //static Dictionary<ColourBlindMode, Dictionary<ColourName, Color>> dictionary = new Dictionary<ColourBlindMode, Dictionary<ColourName, Color>>()
        //{
        //    {
        //        ColourBlindMode.Trichromacy, new Dictionary<ColourName, Color>()
        //        {
        //            {ColourName.white, new Color(1,1,1) }
        //        }


        //    }

        //};

        //private static Color GetColour(ColourName c)
        //{

        //}

        public static Color GetColour(Color colour)
        {
            return Get(colour);
        }

        public static Color GetColour(string colour)
        {
            if (!ColorUtility.TryParseHtmlString($"#{colour}", out Color c)) throw new Exception($"Invalid colour string {colour}");
            return Get(c);
        }

        public static string GetColourString(Color colour)
        {
            var c = GetColour(colour);
            return ColorUtility.ToHtmlStringRGB(c);
        }

        public static string GetColourString(string colour)
        {
            var c = GetColour(colour);
            return ColorUtility.ToHtmlStringRGB(c);
        }


        private static Color Get(Color c)
        {
            if (Mode == ColourBlindMode.Trichromacy)
            {
                return c;
            }

            float diff = CalculateDistance(c);

            
            Color colour = SpinColour(c, diff);

            var d2 = CalculateDistance(colour);

            Debug.Log($"{c} ({diff}) to {colour} ({d2})");
            return colour;

        }

        private static Color SpinColour(Color c, float diff)
        {
            if (Mathf.Abs(diff) < 0.05) diff = 0;

            Color.RGBToHSV(c, out var h, out var s, out var v);

            h = (h + diff + 1) % 1;

            var colour = Color.HSVToRGB(h, s, v);
            return colour;
        }

        private static float CalculateDistance(Color c)
        {
            var pSpaceVector = PerceptualDiff(c) / 100f;
            var pSpaceColour = new Color(pSpaceVector.x, pSpaceVector.y, pSpaceVector.z);
            float diff;
            switch (Mode)
            {
                case ColourBlindMode.Deuteranopia:

                    diff = (pSpaceColour.g - pSpaceColour.r);// * (pSpaceColour.g - pSpaceColour.b);
                    break;
                case ColourBlindMode.Protanopia:

                    diff = (pSpaceColour.r - pSpaceColour.g);// * (pSpaceColour.r - pSpaceColour.b);

                    break;
                case ColourBlindMode.Tritanopia:

                    var y = (pSpaceColour.r + pSpaceColour.g) / 2f;
                    diff = (pSpaceColour.b - y);

                    break;
                default: throw new Exception();
            }

            return (diff);
        }

        /// <summary>
        /// http://www.easyrgb.com/en/math.php
        /// </summary>
        private static Vector3 PerceptualDiff(Color cl)
        {
            var var_R = cl.r;
            var var_G = cl.g;
            var var_B = cl.g;

            if (var_R > 0.04045f) var_R = Mathf.Pow(((var_R + 0.055f) / 1.055f), 2.4f);
            else var_R = var_R / 12.92f;
            if (var_G > 0.04045f) var_G = Mathf.Pow(((var_G + 0.055f) / 1.055f), 2.4f);
            else var_G = var_G / 12.92f;
            if (var_B > 0.04045f) var_B = Mathf.Pow(((var_B + 0.055f) / 1.055f), 2.4f);
            else var_B = var_B / 12.92f;

            var_R = var_R * 100;
            var_G = var_G * 100;
            var_B = var_B * 100;

            var X = var_R * 0.4124f + var_G * 0.3576f + var_B * 0.1805f;
            var Y = var_R * 0.2126f + var_G * 0.7152f + var_B * 0.0722f;
            var Z = var_R * 0.0193f + var_G * 0.1192f + var_B * 0.9505f;

            return new Vector3(X, Y, Z);
        }
    }
}