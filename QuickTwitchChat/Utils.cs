using System.Collections.Generic;

namespace QuickTwitchChat;

public static class Utils
{
    public static string listToString(List<string> list)
    {
        var text = "";
        text += "[";
        var i = 0;
        foreach (var s in list)
        {
            if (i > 0)
            {
                text += $", {s}";
            }
            else
            {
                text += $"{s}";
            }
            i++;
        }
        text += "]";

        return text;
    }
}