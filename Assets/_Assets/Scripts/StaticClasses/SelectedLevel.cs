using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectedLevel {
    public static int selectedLevel { get; private set; }

    static SelectedLevel() {
        selectedLevel = 0;
    }
    public static void SetSelectedLevel(int level) {
        selectedLevel = level;
    }

}
