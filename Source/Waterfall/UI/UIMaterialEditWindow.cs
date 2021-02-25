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


    Dictionary<string, Vector4> vec4Values = new Dictionary<string, Vector4>();
    Dictionary<string, Color> colorValues = new Dictionary<string, Color>();
    Dictionary<string, float> floatValues = new Dictionary<string, float>();
    Dictionary<string, string> textureValues = new Dictionary<string, string>();
    Dictionary<string, Vector2> textureScaleValues = new Dictionary<string, Vector2>();
    Dictionary<string, Vector2> textureOffsetValues = new Dictionary<string, Vector2>();
    Dictionary<string, Texture2D> colorTextures = new Dictionary<string, Texture2D>();
    Dictionary<string, bool> colorEdits = new Dictionary<string, bool>();
    Dictionary<string, bool> textureEdits = new Dictionary<string, bool>();

    Dictionary<string, string[]> colorStrings = new Dictionary<string, string[]>();
    Dictionary<string, string[]> vec4Strings = new Dictionary<string, string[]>();
    Dictionary<string, string> floatStrings = new Dictionary<string, string>();
    Dictionary<string, string[]> textureOffsetStrings = new Dictionary<string, string[]>();
    Dictionary<string, string[]> textureScaleStrings = new Dictionary<string, string[]>();


    public UIMaterialEditWindow(WaterfallModel modelToEdit, bool show) : base(show)
    {
      materialID = 0;
      model = modelToEdit;
      Utils.Log($"[UIMaterialEditWindow]: Started editing materials on {modelToEdit.ToString()}", LogType.UI);

      materialList = new string[model.materials.Count];
      for (int i = 0; i < model.materials.Count; i++)
      {
        materialList[i] = $"{model.materials[i].transformName} ({model.materials[i].materials[0].name.Split('(').First()})";
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
      Utils.Log($"[UIMaterialEditWindow]: Started editing materials on {modelToEdit.ToString()}", LogType.UI);
      materialID = 0;
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
          // Button to set that we are toggling the texture picker
        if (GUILayout.Button(kvp.Value))
        {
          textureEdits[kvp.Key] = !textureEdits[kvp.Key];
          Utils.Log($"[TP] Edit flag state {textureEdits[kvp.Key]}", LogType.UI);
          // if yes, open the window
          if (textureEdits[kvp.Key])
          {
            WaterfallUI.Instance.OpenTextureEditWindow(kvp.Key, textureValues[kvp.Key]);
            Utils.Log("[TP] Open Window", LogType.UI);
          }
        }

        // If picker open
        if (textureEdits[kvp.Key])
        {
          // Close all other pickers
          foreach (KeyValuePair<string, bool> kvp2 in textureEdits.ToList())
          {
            if (kvp2.Key != kvp.Key)
              textureEdits[kvp2.Key] = false;
          }

          string newTex = WaterfallUI.Instance.GetTextureFromPicker();
          if (newTex != kvp.Value)
          {
            textureValues[kvp.Key] = newTex;
            Utils.Log($"[MaterialEditWindow] Changed {kvp.Key} to {textureValues[kvp.Key]}", LogType.UI);
            model.SetTexture(matl, kvp.Key, textureValues[kvp.Key]);
          }
        }

        GUILayout.EndHorizontal();
        delta = false;
        GUILayout.BeginHorizontal();
        GUILayout.Space(headerWidth);
        GUILayout.Label($"UV Scale", GUILayout.Width(headerWidth));
        textureScaleValues[kvp.Key] = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), textureScaleValues[kvp.Key], textureScaleStrings[kvp.Key], GUI.skin.label, GUI.skin.textArea, out delta);
        if (delta)
        {
          model.SetTextureScale(matl, kvp.Key, textureScaleValues[kvp.Key]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(headerWidth);
        GUILayout.Label($"UV Offset", GUILayout.Width(headerWidth));
        textureOffsetValues[kvp.Key] = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), textureOffsetValues[kvp.Key], textureOffsetStrings[kvp.Key], GUI.skin.label, GUI.skin.textArea, out delta);
        if (delta)
        {
          model.SetTextureOffset(matl, kvp.Key, textureOffsetValues[kvp.Key]);
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
          Utils.Log($"[CP] Edit flag state {colorEdits[kvp.Key]}", LogType.UI);
          // if yes, open the window
          if (colorEdits[kvp.Key])
          {
            WaterfallUI.Instance.OpenColorEditWindow(colorValues[kvp.Key]);
            Utils.Log("[CP] Open Window", LogType.UI);
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
          model.SetColor(matl, kvp.Key, colorValues[kvp.Key]);
        }
        GUILayout.EndHorizontal();
      }
      foreach (KeyValuePair<string, float> kvp in floatValues.ToList())
      {
        float sliderVal;
        string textVal;
        GUILayout.BeginHorizontal();
        GUILayout.Label(kvp.Key, GUILayout.Width(headerWidth));
        sliderVal = GUILayout.HorizontalSlider(floatValues[kvp.Key], WaterfallConstants.ShaderPropertyMap[kvp.Key].floatRange.x, WaterfallConstants.ShaderPropertyMap[kvp.Key].floatRange.y);

        if (sliderVal != floatValues[kvp.Key])
        {
          floatValues[kvp.Key] = sliderVal;
          floatStrings[kvp.Key] = sliderVal.ToString();

          model.SetFloat(matl, kvp.Key, floatValues[kvp.Key]);
          
        }
        
        textVal = GUILayout.TextArea(floatStrings[kvp.Key], GUILayout.Width(90f));

        
        if (textVal != floatStrings[kvp.Key])
        {
          float outVal;
          if (float.TryParse(textVal, out outVal))
          {
            floatValues[kvp.Key] = outVal;

            model.SetFloat(matl, kvp.Key, floatValues[kvp.Key]);
          }
          floatStrings[kvp.Key] = textVal;
        }

        //float parsed = kvp.Value;
        //if (float.TryParse(textVal, out parsed))
        //{

        //  if (parsed != floatValues[kvp.Key])
        //  {
        //    floatValues[kvp.Key] = parsed;
        //    matl.SetFloat(kvp.Key, parsed);
        //  }
        //}
        
        GUILayout.EndHorizontal();
      }
      foreach (KeyValuePair<string, Vector4> kvp in vec4Values.ToList())
      {
        Vector3 vecVal;
        string textVal;
        GUILayout.BeginHorizontal();
        GUILayout.Label(kvp.Key, GUILayout.Width(headerWidth));
        vecVal = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), kvp.Value, vec4Strings[kvp.Key], GUI.skin.label, GUI.skin.textArea);
        Vector4 temp = new Vector4(vecVal.x, vecVal.y, vecVal.z, 0f);
        if (temp != vec4Values[kvp.Key])
        {
          vec4Values[kvp.Key] = temp;
          matl.SetVector4(kvp.Key, temp);
        }

       
        GUILayout.EndHorizontal();
      }
      GUILayout.BeginHorizontal();
      GUILayout.Space(headerWidth);
      GUILayout.Label($"Queue: {matl.materials[0].renderQueue}", GUILayout.Width(headerWidth));
      GUILayout.EndHorizontal();
    }

    protected void InitializeShaderProperties(Material m)
    {
      Utils.Log($"[MaterialEditor] Generating shader property map for {m}", LogType.UI);
      colorValues = new Dictionary<string, Color>();
      floatValues = new Dictionary<string, float>();
      textureValues = new Dictionary<string, string>();
      textureEdits = new Dictionary<string, bool>();
      textureScaleValues = new Dictionary<string, Vector2>();
      textureOffsetValues = new Dictionary<string, Vector2>();
      vec4Values = new Dictionary<string, Vector4>();

      colorStrings = new Dictionary<string, string[]>();
      colorEdits = new Dictionary<string, bool>();
      floatStrings = new Dictionary<string, string>();
      textureOffsetStrings = new Dictionary<string, string[]>();
      textureScaleStrings = new Dictionary<string, string[]>();
      vec4Strings = new Dictionary<string, string[]>();
      colorTextures = new Dictionary<string, Texture2D>();

      foreach (KeyValuePair<string, MaterialData> mProp in WaterfallConstants.ShaderPropertyMap)
      {

        if (m.HasProperty(mProp.Key))
        {
          if (mProp.Value.type == WaterfallMaterialPropertyType.Color)
          {
            colorValues.Add(mProp.Key, m.GetColor(mProp.Key));
            colorEdits.Add(mProp.Key, false);
            colorStrings.Add(mProp.Key, MaterialUtils.ColorToStringArray(m.GetColor(mProp.Key)));
            colorTextures.Add(mProp.Key, MaterialUtils.GenerateColorTexture(32, 32, m.GetColor(mProp.Key)));
          }
          if (mProp.Value.type == WaterfallMaterialPropertyType.Float)
          {
            floatValues.Add(mProp.Key, m.GetFloat(mProp.Key));
            floatStrings.Add(mProp.Key, m.GetFloat(mProp.Key).ToString());
          }
          if (mProp.Value.type == WaterfallMaterialPropertyType.Vector4)
          {
            
            Vector4 vec4 = m.GetVector(mProp.Key);
            
            vec4Values.Add(mProp.Key, vec4);
            vec4Strings.Add(mProp.Key, new string[] { $"{vec4.x}", $"{vec4.y}", $"{vec4.z}", $"{vec4.w}" });
          }
          if (mProp.Value.type == WaterfallMaterialPropertyType.Texture)
          {
            if (m.GetTexture(mProp.Key))
            {
              textureValues.Add(mProp.Key, m.GetTexture(mProp.Key).name);
            }
            else
            {
              textureValues.Add(mProp.Key, null);
            }
            textureEdits.Add(mProp.Key, false);
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

