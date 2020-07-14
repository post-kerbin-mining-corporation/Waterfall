using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIMaterialEditWindow : UIPopupWindow
  {

    protected string windowTitle = "";
    WaterfallMaterial matl;
    WaterfallModel model;
    string[] materialList;
    int materialID = 0;
    int savedID = -1;


    Dictionary<string, Color> colorValues = new Dictionary<string, Color>();
    Dictionary<string, float> floatValues = new Dictionary<string, float>();
    Dictionary<string, string> textureValues = new Dictionary<string, string>();
    Dictionary<string, Vector2> textureScaleValues = new Dictionary<string, Vector2>();
    Dictionary<string, Vector2> textureOffsetValues = new Dictionary<string, Vector2>();
    Dictionary<string, Texture2D> colorTextures = new Dictionary<string, Texture2D>();
    Dictionary<string, bool> colorEdits = new Dictionary<string, bool>();

    Dictionary<string, string[]> colorStrings = new Dictionary<string, string[]>();
    Dictionary<string, string> floatStrings = new Dictionary<string, string>();
    Dictionary<string, string[]> textureOffsetStrings = new Dictionary<string, string[]>();
    Dictionary<string, string[]> textureScaleStrings = new Dictionary<string, string[]>();


    public UIMaterialEditWindow(WaterfallModel modelToEdit, bool show) : base(show)
    {
      model = modelToEdit;
      Utils.Log($"[UIMaterialEditWindow]: Started editing materials on {modelToEdit.ToString()}");

      materialList = new string[model.materials.Count];
      for (int i = 0; i < model.materials.Count; i++)
      {
        materialList[i] = model.materials[i].materials[0].name;
      }
      matl = modelToEdit.materials[materialID];


      InitializeShaderProperties(matl.materials[0]);
      WindowPosition = new Rect(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }

    protected override void InitUI()
    {
      windowTitle = "Material Editor";
      base.InitUI();


    }


    public void ChangeMaterial(WaterfallModel modelToEdit)
    {
      model = modelToEdit;
      Utils.Log($"[UIMaterialEditWindow]: Started editing materials on {modelToEdit.ToString()}");

      materialList = new string[model.materials.Count];
      for (int i = 0; i < model.materials.Count; i++)
      {
        materialList[i] = model.materials[i].materials[0].name;
      }
      matl = modelToEdit.materials[materialID];
      showWindow = true;

      InitializeShaderProperties(matl.materials[0]);

      WindowPosition = new Rect(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      DrawMaterials();
      DrawMaterialEdit();
      GUI.DragWindow();

    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }
    protected void DrawMaterials()
    {
      GUILayout.BeginHorizontal();
      materialID = GUILayout.SelectionGrid(materialID, materialList, 2, GUIResources.GetStyle("radio_text_button"));
      if (materialID != savedID)
      {
        savedID = materialID;
        matl = model.materials[savedID];
        InitializeShaderProperties(model.materials[savedID].materials[0]);
      }
      GUILayout.EndHorizontal();
    }
    protected void DrawMaterialEdit()
    {
      float headerWidth = 120f;
      bool delta = false;
      GUILayout.Label("<b>Textures</b>");
      foreach (KeyValuePair<string, string> kvp in textureValues.ToList())
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{kvp.Key}", GUILayout.Width(headerWidth));
        GUILayout.Label("Texture Path");
        GUILayout.Label(kvp.Value);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(headerWidth);
        GUILayout.Label($"UV Scale", GUILayout.Width(headerWidth));
        textureScaleValues[kvp.Key] = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), textureScaleValues[kvp.Key], textureScaleStrings[kvp.Key], GUI.skin.label, GUI.skin.textArea, out delta);
        if (delta)
        {
          matl.SetTextureScale(kvp.Key, textureScaleValues[kvp.Key]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(headerWidth);
        GUILayout.Label($"UV Offset", GUILayout.Width(headerWidth));
        textureOffsetValues[kvp.Key] = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), textureOffsetValues[kvp.Key], textureOffsetStrings[kvp.Key], GUI.skin.label, GUI.skin.textArea, out delta);
        if (delta)
        {
          matl.SetTextureOffset(kvp.Key, textureOffsetValues[kvp.Key]);
        }
        GUILayout.EndHorizontal();
      }
      GUILayout.Label("<b>Material Parameters</b>");
      foreach (KeyValuePair<string, Color> kvp in colorValues.ToList())
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label(kvp.Key, GUILayout.Width(headerWidth));
        GUILayout.Space(10);

        // Button to set that we are toggling the color picker
        if (GUILayout.Button("", GUILayout.Width(60)))
        {
          colorEdits[kvp.Key] = !colorEdits[kvp.Key];
          Utils.Log($"[CP] Edit flag state {colorEdits[kvp.Key]}");
          // if yes, open the window
          if (colorEdits[kvp.Key])
          {
            WaterfallUI.Instance.OpenColorEditWindow(colorValues[kvp.Key]);
            Utils.Log("[CP] Open Window");
          }
        }

        // If picker open
        if (colorEdits[kvp.Key])
        {
          // Close all other pickers
          foreach (KeyValuePair<string, bool> kvp2 in colorEdits.ToList())
          {
            if (kvp2.Key != kvp.Key)
              colorEdits[kvp2.Key] = false;
          }

          Color c = WaterfallUI.Instance.GetColorFromPicker();
          if (!c.IsEqualTo(colorValues[kvp.Key]))
          {
            colorValues[kvp.Key] = c;
            delta = true;
          }
        }
        Rect tRect = GUILayoutUtility.GetLastRect();
        tRect = new Rect(tRect.x + 3, tRect.y + 3, tRect.width - 6, tRect.height - 6);
        GUI.DrawTexture(tRect, colorTextures[kvp.Key]);



        //GUILayout.Space(10);
        //colorValues[kvp.Key] = UIUtils.ColorInputField(GUILayoutUtility.GetRect(200f, 30f), colorValues[kvp.Key], colorStrings[kvp.Key], GUI.skin.label, GUI.skin.textArea, out delta);
        if (delta)
        {
          colorTextures[kvp.Key] = MaterialUtils.GenerateColorTexture(64, 32, colorValues[kvp.Key]);
          matl.SetColor(kvp.Key, colorValues[kvp.Key]);
        }
        GUILayout.EndHorizontal();
      }
      foreach (KeyValuePair<string, float> kvp in floatValues.ToList())
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label(kvp.Key, GUILayout.Width(headerWidth));
        floatStrings[kvp.Key] = GUILayout.TextArea(floatStrings[kvp.Key]);

        float parsed = kvp.Value;
        if (float.TryParse(floatStrings[kvp.Key], out parsed))
        {
          floatValues[kvp.Key] = parsed;
          matl.SetFloat(kvp.Key, parsed);
        }
        GUILayout.EndHorizontal();
      }
    }

    protected void InitializeShaderProperties(Material m)
    {
      colorValues = new Dictionary<string, Color>();
      floatValues = new Dictionary<string, float>();
      textureValues = new Dictionary<string, string>();
      textureScaleValues = new Dictionary<string, Vector2>();
      textureOffsetValues = new Dictionary<string, Vector2>();
      colorStrings = new Dictionary<string, string[]>();
      colorEdits = new Dictionary<string, bool>();
      floatStrings = new Dictionary<string, string>();
      textureOffsetStrings = new Dictionary<string, string[]>();
      textureScaleStrings = new Dictionary<string, string[]>();
      colorTextures = new Dictionary<string, Texture2D>();

      foreach (KeyValuePair<string, WaterfallMaterialPropertyType> mProp in WaterfallConstants.ShaderPropertyMap)
      {
        if (m.HasProperty(mProp.Key))
        {
          if (mProp.Value == WaterfallMaterialPropertyType.Color)
          {
            colorValues.Add(mProp.Key, m.GetColor(mProp.Key));
            colorEdits.Add(mProp.Key, false);
            colorStrings.Add(mProp.Key, MaterialUtils.ColorToStringArray(m.GetColor(mProp.Key)));
            colorTextures.Add(mProp.Key, MaterialUtils.GenerateColorTexture(32, 32, m.GetColor(mProp.Key)));
          }
          if (mProp.Value == WaterfallMaterialPropertyType.Float)
          {
            floatValues.Add(mProp.Key, m.GetFloat(mProp.Key));
            floatStrings.Add(mProp.Key, m.GetFloat(mProp.Key).ToString());
          }
          if (mProp.Value == WaterfallMaterialPropertyType.Texture)
          {
            textureValues.Add(mProp.Key, m.GetTexture(mProp.Key).name);
            textureScaleValues.Add(mProp.Key, m.GetTextureScale(mProp.Key));
            textureOffsetValues.Add(mProp.Key, m.GetTextureOffset(mProp.Key));
            textureOffsetStrings.Add(mProp.Key, new string[] { $"{m.GetTextureOffset(mProp.Key).x}", $"{m.GetTextureOffset(mProp.Key).y}" });
            textureScaleStrings.Add(mProp.Key, new string[] { $"{m.GetTextureScale(mProp.Key).x}", $"{m.GetTextureScale(mProp.Key).y}" });
          }
        }
      }
    }
  }
}

