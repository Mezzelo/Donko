using System.Collections;
using System.Collections.Generic;

public class GlobalVars
{
    // 0 = default, 1 = loss, 2 = win
    public static int menuScreen = 0;

    public static int startSpawn = 0;

    // 0 = normal, 1 = hard
    public static int lastLevelCompleted = -1;

    public static bool dataHasLoaded = false;

    public static bool playerShadow = false;

    public static int gameVol = 100;
    public static int musicVol = 100;

    public static int currentScore = 0;
    public static float combinedLevelTime = 0;

    public static int newScoreIndex = 0;

    public static bool scoresSetup = false;
    public static string[] highScoreNames = new string[5];
    public static int[] highScores = new int[5];
}
