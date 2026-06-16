using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MassMaterialAdder : EditorWindow
{
    public Material materialToAdd;

    [MenuItem("Tools/Masowe Dodawanie Materialu")]
    public static void ShowWindow()
    {
        GetWindow<MassMaterialAdder>("Dodaj Materia³");
    }

    void OnGUI()
    {
        GUILayout.Label("Wybierz materia³, który chcesz dodaæ jako DRUGI:", EditorStyles.boldLabel);

        // Pole na przeci¹gniêcie materia³u (np. Twojego outline)
        materialToAdd = (Material)EditorGUILayout.ObjectField("Materia³", materialToAdd, typeof(Material), false);

        if (GUILayout.Button("Dodaj do wszystkich obiektów na scenie"))
        {
            if (materialToAdd == null)
            {
                EditorUtility.DisplayDialog("B³¹d", "Najpierw wybierz materia³!", "OK");
                return;
            }

            AddMaterialToAllRenderers();
        }
    }

    void AddMaterialToAllRenderers()
    {
        // Znajduje wszystkie obiekty MeshRenderer na obecnej scenie
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include);
        int counter = 0;

        // Pozwala na cofniêcie operacji przez Ctrl + Z
        Undo.RecordObjects(renderers, "Masowe dodawanie materia³u");

        foreach (MeshRenderer renderer in renderers)
        {
            // Pobieramy obecn¹ listê materia³ów obiektu
            List<Material> currentMaterials = new List<Material>(renderer.sharedMaterials);

            // Sprawdzamy, czy ten materia³ ju¿ tam jest, ¿eby nie dodawaæ go dwa razy
            if (!currentMaterials.Contains(materialToAdd))
            {
                currentMaterials.Add(materialToAdd);

                // Przypisujemy zaktualizowan¹ listê z powrotem do obiektu
                renderer.sharedMaterials = currentMaterials.ToArray();
                counter++;
            }
        }

        // Informujemy Unity, ¿e scena siê zmieni³a i trzeba j¹ zapisaæ
        if (counter > 0)
        {
            EditorUtility.SetDirty(this);
            Debug.Log($"Sukces! Dodano materia³ do {counter} obiektów na scenie.");
        }
    }
}
