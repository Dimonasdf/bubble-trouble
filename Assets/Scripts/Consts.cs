using UnityEngine;

public static class Consts
{
    public static int Default = LayerMask.NameToLayer("Default");
    public static int Bubbles = LayerMask.NameToLayer("Bubbles");
    public static int Scraper = LayerMask.NameToLayer("Scraper");
    public static int Protector = LayerMask.NameToLayer("Protector");
    public static int Screen = LayerMask.NameToLayer("Screen");

    public static int MouseCastMask = LayerMask.GetMask("Default", "Screen");

    public static LayerMask MaskFromLayer(this int layer)
    {
        return 1 << layer;
    }
}
