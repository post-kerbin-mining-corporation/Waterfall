using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;

namespace Waterfall.UI
{
  public class UIResources
  {
    private Dictionary<string, AtlasIcon> iconList;
    private Dictionary<string, GUIStyle> styleList;
    private Dictionary<string, Color> colorList;

    private Texture generalIcons;

    // Get any color, given its name
    public Color GetColor(string name)
    {
      Color color = Color.white;
      colorList.TryGetValue(name, out color);
      return color;
    }

    // Get any icon, given its name
    public AtlasIcon GetIcon(string name)
    {
      AtlasIcon icon = iconList.First().Value;
      iconList.TryGetValue(name, out icon);
      return icon;
    }

    // Get a style, given its name
    public GUIStyle GetStyle(string name)
    {
      GUIStyle style = styleList.First().Value;
      styleList.TryGetValue(name, out style);
      return style;
    }

    // Constructor
    public UIResources()
    {
      CreateIconList();
      CreateStyleList();
      CreateColorList();
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Loaded Assets");
    }

    /// <summary>
    /// Gets a texture from the UI folder by name
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    private Texture GetUITexture(string textureName)
    {
      Texture toReturn;
      try
      {
        toReturn = (Texture)GameDatabase.Instance.GetTexture($"{UIConstants.UIResourcePath}{textureName}", false);
      } 
      catch
      {
        
        Debug.LogError($"Error loading UI texture {textureName}");
        return null;
      }

      return toReturn;
    }

    // Iniitializes the icon database
    private void CreateIconList()
    {
      generalIcons = GetUITexture("icon_general");
      iconList = new Dictionary<string, AtlasIcon>();

      // Add the general icons
      iconList.Add("lightning", new AtlasIcon(generalIcons, 0.00f, 0.75f, 0.25f, 0.25f));
      iconList.Add("fire", new AtlasIcon(generalIcons, 0.25f, 0.75f, 0.25f, 0.25f));
      iconList.Add("thermometer", new AtlasIcon(generalIcons, 0.50f, 0.75f, 0.25f, 0.25f));
      iconList.Add("timer", new AtlasIcon(generalIcons, 0.75f, 0.75f, 0.25f, 0.25f));

      iconList.Add("battery", new AtlasIcon(generalIcons, 0.5f, 0.50f, 0.25f, 0.25f));

      iconList.Add("cancel", new AtlasIcon(generalIcons, 0.75f, 0.00f, 0.25f, 0.25f));

    }

    // Initializes all the styles
    private void CreateStyleList()
    {
      styleList = new Dictionary<string, GUIStyle>();

      GUIStyle draftStyle;

      // -- REGIONS --
      // Window
      draftStyle = new GUIStyle(HighLogic.Skin.window);
      draftStyle.padding = new RectOffset(draftStyle.padding.left, draftStyle.padding.right, 2, draftStyle.padding.bottom);
      styleList.Add("window_main", new GUIStyle(draftStyle));

      // Area Background
      draftStyle = new GUIStyle(HighLogic.Skin.textArea);
      draftStyle.active = draftStyle.hover = draftStyle.normal;
      draftStyle.padding = new RectOffset(8, 8, 8, 8);
      styleList.Add("block_background", new GUIStyle(draftStyle));

      // --- BUTTONS ---
      // Toggle Buttonss
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      styleList.Add("radio_text_button", new GUIStyle(draftStyle));

      // Accept button
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_accept", new GUIStyle(draftStyle));
      // Cancel button
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_cancel", new GUIStyle(draftStyle));
      // Image overlaid button
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_overlaid", new GUIStyle(draftStyle));
      // Image overlaid button
      draftStyle = new GUIStyle(HighLogic.Skin.toggle);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_toggle", new GUIStyle(draftStyle));


      // Reddish header button type
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      draftStyle.fontSize = 14;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      styleList.Add("positive_button", new GUIStyle(draftStyle));
      draftStyle.stretchWidth = true;
      // Blueish head button type
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      draftStyle.fontSize = 14;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      styleList.Add("negative_button", new GUIStyle(draftStyle));
      draftStyle.stretchWidth = true;
      // Blueish head button type
      draftStyle = new GUIStyle(HighLogic.Skin.button);
      draftStyle.fontSize = 14;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("category_header_button", new GUIStyle(draftStyle));
      draftStyle.stretchWidth = true;


      // -- TEXT ---
      // Window Header
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontStyle = FontStyle.Bold;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      draftStyle.fontSize = 18;
      draftStyle.stretchWidth = true;
      styleList.Add("window_header", new GUIStyle(draftStyle));
      // Basic text
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 12;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      styleList.Add("text_basic", new GUIStyle(draftStyle));

      // Category table left header
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.fontStyle = FontStyle.Bold;
      draftStyle.alignment = TextAnchor.MiddleCenter;
      draftStyle.stretchWidth = true;

      styleList.Add("panel_header_centered", new GUIStyle(draftStyle));

      // Category table left header
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 12;

      draftStyle.fontStyle = FontStyle.Bold;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("positive_category_header", new GUIStyle(draftStyle));

      // Category table field right
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.alignment = TextAnchor.MiddleRight;
      draftStyle.padding.right = 5;
      styleList.Add("positive_category_header_field", new GUIStyle(draftStyle));

      // Category table left header
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.fontStyle = FontStyle.Bold;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("negative_category_header", new GUIStyle(draftStyle));

      // Category table field right
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.alignment = TextAnchor.MiddleRight;
      draftStyle.padding.right = 5;
      styleList.Add("negative_category_header_field", new GUIStyle(draftStyle));


      // Category table left header
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.fontStyle = FontStyle.Bold;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("category_header", new GUIStyle(draftStyle));

      // Category table field right
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.alignment = TextAnchor.MiddleRight;
      draftStyle.padding.right = 5;
      styleList.Add("category_header_field", new GUIStyle(draftStyle));

      // Data table left header
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.fontStyle = FontStyle.Bold;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 5;
      styleList.Add("data_header", new GUIStyle(draftStyle));

      // Data table field right
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 14;
      draftStyle.padding.right = 3;
      draftStyle.alignment = TextAnchor.MiddleRight;
      styleList.Add("data_field", new GUIStyle(draftStyle));

      // Data table field right
      draftStyle = new GUIStyle(HighLogic.Skin.label);
      draftStyle.fontSize = 16;
      draftStyle.alignment = TextAnchor.MiddleRight;
      styleList.Add("data_field_large", new GUIStyle(draftStyle));
    }
    void CreateColorList()
    {
      colorList = new Dictionary<string, Color>();

      colorList.Add("cancel_color", new Color(208f / 255f, 131f / 255f, 86f / 255f));
      colorList.Add("accept_color", new Color(209f / 255f, 250f / 255f, 146f / 255f));
      colorList.Add("capacitor_blue", new Color(134f / 255f, 197f / 255f, 239f / 255f));
      colorList.Add("readout_green", new Color(203f / 255f, 238f / 255f, 115f / 255f));
    }
  }

  // Represents an atlased icon via a source texture and rectangle
  public class AtlasIcon
  {
    public Texture iconAtlas;
    public Rect iconRect;

    public AtlasIcon(Texture theAtlas, float bl_x, float bl_y, float x_size, float y_size)
    {
      iconAtlas = theAtlas;
      iconRect = new Rect(bl_x, bl_y, x_size, y_size);
    }
  }

}
