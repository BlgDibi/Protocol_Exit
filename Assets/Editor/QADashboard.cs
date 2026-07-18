using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class QADashboard : EditorWindow
{
    [MenuItem("QA Tools/Test Dashboard")]
    public static void ShowWindow()
    {
        GetWindow<QADashboard>("Test Dashboard");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Mechanics QA", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Test 1: Player HP Damage", GUILayout.Height(30))) TestHPDamage();
        EditorGUILayout.Space();

        if (GUILayout.Button("Test 2: Key in Inventory", GUILayout.Height(30))) TestKeyInventory();
        EditorGUILayout.Space();

        if (GUILayout.Button("Test 3: Save & Load System", GUILayout.Height(30))) TestSaveLoadSystem();
        EditorGUILayout.Space();

        if (GUILayout.Button("Test 4: Scale Validation (WeightPlate)", GUILayout.Height(30))) TestScale();
        EditorGUILayout.Space();

        if (GUILayout.Button("Test 5: Doors Loading UI Check", GUILayout.Height(30))) TestDoorUI();
    }

    private void TestHPDamage()
    {
        Debug.Log("<b>--- STARTING TEST 1: HP DAMAGE ---</b>");

        Debug.Log("Step 1: Searching for Player HP script...");
        HP playerHP = FindFirstObjectByType<HP>();
        if (playerHP == null) { Debug.LogWarning("[WARNING] No HP script found in the scene."); return; }

        float originalHp = playerHP.Hp;
        Debug.Log($"Step 2: Player found. Original HP is {originalHp}.");

        Debug.Log("Step 3: Simulating 15 damage...");
        playerHP.Hp -= 15f;

        Debug.Log($"Step 4: Expected HP: {originalHp - 15f} | Actual HP: {playerHP.Hp}");

        if (playerHP.Hp == (originalHp - 15f)) Debug.Log("<color=green>[PASS] HP damage works correctly.</color>");
        else Debug.LogError("[FAIL] HP damage calculation failed.");

        playerHP.Hp = originalHp;
        Debug.Log("Test finished. Restored original HP.");
    }

    private void TestKeyInventory()
    {
        Debug.Log("<b>--- STARTING TEST 2: INVENTORY ---</b>");

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[WARNING] Run this test in Play Mode.");
            return;
        }

        Debug.Log("Step 1: Searching for KeyInventory in the scene...");
        KeyInventory myInventory = FindFirstObjectByType<KeyInventory>();
        if (myInventory == null)
        {
            Debug.LogError("[FAIL] KeyInventory not found in the scene!");
            return;
        }

        Debug.Log("Step 2: Bypassing security to access private key list (Reflection)...");
        var keyIdsField = typeof(KeyInventory).GetField("keyIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (keyIdsField != null)
        {
            List<int> actualKeyList = (List<int>)keyIdsField.GetValue(myInventory);

            int testKeyID = 999;
            Debug.Log($"Step 3: Injecting fake Key (ID: {testKeyID}) directly into pocket...");
            actualKeyList.Add(testKeyID);

            Debug.Log("Step 4: Asking the Game System if it sees the new key...");
            bool hasKey = myInventory.GetSavedKeyIds().Contains(testKeyID);

            if (hasKey)
                Debug.Log("<color=green>[PASS] Key successfully added and recognized by the system.</color>");
            else
                Debug.LogError("[FAIL] Key system failed to recognize the key.");

            actualKeyList.Remove(testKeyID);
            Debug.Log("Test finished. Fake key removed from pocket.");
        }
    }

    private void TestSaveLoadSystem()
    {
        Debug.Log("<b>--- STARTING TEST 3: SAVE/LOAD ---</b>");

        string testNPC = "QA_test_npc_999";
        Debug.Log($"Step 1: Simulating interaction with fake NPC ({testNPC})...");

        Debug.Log("Step 2: Sending data to SaveSystem...");
        SaveSystem.SaveNPCMet(testNPC);

        Debug.Log("Step 3: Attempting to read data back from memory...");
        bool wasMet = SaveSystem.WasNPCMet(testNPC);

        Debug.Log($"Step 4: Memory check result -> {wasMet}");

        if (wasMet) Debug.Log("<color=green>[PASS] Save/Load system works perfectly.</color>");
        else Debug.LogError("[FAIL] Save/Load system failed to remember the data.");
    }

    private void TestScale()
    {
        Debug.Log("<b>--- STARTING TEST 4: SCALE VALIDATION ---</b>");

        Debug.Log("Step 1: Searching for WeightPlate in the scene...");
        WeightPlate scale = FindFirstObjectByType<WeightPlate>();
        if (scale == null)
        {
            Debug.LogWarning("[WARNING] No WeightPlate found in the current scene.");
            return;
        }

        Debug.Log("Step 2: Reading private target weight variable...");
        var optimalWeightField = typeof(WeightPlate).GetField("optimalweight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (optimalWeightField != null)
        {
            float targetWeight = (float)optimalWeightField.GetValue(scale);
            Debug.Log($"Step 3: Found Target Weight: {targetWeight}kg");

            if (targetWeight > 0) Debug.Log($"<color=green>[PASS] Scale is correctly configured.</color>");
            else Debug.LogError("[FAIL] Scale target weight is 0! The puzzle cannot be solved.");
        }
    }

    private void TestDoorUI()
    {
        Debug.Log("<b>--- STARTING TEST 5: DOOR UI ---</b>");

        Debug.Log("Step 1: Searching for all Doors in the scene...");
        DoorAnimation[] doors = FindObjectsByType<DoorAnimation>(FindObjectsSortMode.None);

        if (doors.Length == 0)
        {
            Debug.LogWarning("[WARNING] No doors (DoorAnimation) found in the current scene.");
            return;
        }

        Debug.Log($"Found {doors.Length} door(s).");
        Debug.Log("Step 2: Checking if every door has the Loading UI assigned...");

        bool allGood = true;
        foreach (var door in doors)
        {
            if (door.loadingUI == null)
            {
                Debug.LogError($"[FAIL] The door '{door.gameObject.name}' is missing the Loading UI Canvas!");
                allGood = false;
            }
            else
            {
                Debug.Log($"Door '{door.gameObject.name}' is OK.");
            }
        }

        if (allGood) Debug.Log("<color=green>[PASS] All doors are safe. No visual bugs expected.</color>");
    }
}