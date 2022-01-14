using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIResources
  {
    private Dictionary<string, AtlasIcon> iconList;
    private Dictionary<string, GUIStyle>  styleList;
    private Dictionary<string, Color>     colorList;

    private Texture generalIcons;

    // Constructor
    public UIResources()
    {
      CreateIconList();
      CreateStyleList();
      CreateColorList();
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Loaded Assets");
    }

    // Get any color, given its name
    public Color GetColor(string name)
    {
      var color = Color.white;
      colorList.TryGetValue(name, out color);
      return color;
    }

    // Get any icon, given its name
    public AtlasIcon GetIcon(string name)
    {
      var icon = iconList.First().Value;
      iconList.TryGetValue(name, out icon);
      return icon;
    }

    // Get a style, given its name
    public GUIStyle GetStyle(string name)
    {
      var style = styleList.First().Value;
      styleList.TryGetValue(name, out style);
      return style;
    }

    /// <summary>
    ///   Gets a texture from the UI folder by name
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    private Texture GetUITexture(string textureName)
    {
      Texture toReturn;
      try
      {
        toReturn = GameDatabase.Instance.GetTexture($"{UIConstants.UIResourcePath}{textureName}", false);
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
      iconList     = new();

      // Add the general icons
      iconList.Add("lightning",   new(generalIcons, 0.00f, 0.75f, 0.25f, 0.25f));
      iconList.Add("fire",        new(generalIcons, 0.25f, 0.75f, 0.25f, 0.25f));
      iconList.Add("thermometer", new(generalIcons, 0.50f, 0.75f, 0.25f, 0.25f));
      iconList.Add("timer",       new(generalIcons, 0.75f, 0.75f, 0.25f, 0.25f));

      iconList.Add("battery", new(generalIcons, 0.5f, 0.50f, 0.25f, 0.25f));

      iconList.Add("cancel", new(generalIcons, 0.75f, 0.00f, 0.25f, 0.25f));
    }

    // Initializes all the styles
    private void CreateStyleList()
    {
      styleList = new();

      GUIStyle draftStyle;

      // -- REGIONS --
      // Window
      draftStyle         = new(HighLogic.Skin.window);
      draftStyle.padding = new(draftStyle.padding.left, draftStyle.padding.right, 2, draftStyle.padding.bottom);
      styleList.Add("window_main", new(draftStyle));

      // Area Background
      draftStyle         = new(HighLogic.Skin.textArea);
      draftStyle.active  = draftStyle.hover = draftStyle.normal;
      draftStyle.padding = new(8, 8, 8, 8);
      styleList.Add("block_background", new(draftStyle));

      // --- BUTTONS ---
      // Toggle Buttonss
      draftStyle = new(HighLogic.Skin.button);
      styleList.Add("radio_text_button", new(draftStyle));

      // Accept button
      draftStyle                  = new(HighLogic.Skin.button);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_accept", new(draftStyle));
      // Cancel button
      draftStyle                  = new(HighLogic.Skin.button);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_cancel", new(draftStyle));
      // Image overlaid button
      draftStyle                  = new(HighLogic.Skin.button);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_overlaid", new(draftStyle));
      // Image overlaid button
      draftStyle                  = new(HighLogic.Skin.toggle);
      draftStyle.normal.textColor = draftStyle.normal.textColor;
      styleList.Add("button_toggle", new(draftStyle));


      // Reddish header button type
      draftStyle           = new(HighLogic.Skin.button);
      draftStyle.fontSize  = 14;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      styleList.Add("positive_button", new(draftStyle));
      draftStyle.stretchWidth = true;
      // Blueish head button type
      draftStyle           = new(HighLogic.Skin.button);
      draftStyle.fontSize  = 14;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      styleList.Add("negative_button", new(draftStyle));
      draftStyle.stretchWidth = true;
      // Blueish head button type
      draftStyle              = new(HighLogic.Skin.button);
      draftStyle.fontSize     = 14;
      draftStyle.alignment    = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("category_header_button", new(draftStyle));
      draftStyle.stretchWidth = true;


      // -- TEXT ---
      // Window Header
      draftStyle              = new(HighLogic.Skin.label);
      draftStyle.fontStyle    = FontStyle.Bold;
      draftStyle.alignment    = TextAnchor.MiddleLeft;
      draftStyle.fontSize     = 18;
      draftStyle.stretchWidth = true;
      styleList.Add("window_header", new(draftStyle));
      // Basic text
      draftStyle           = new(HighLogic.Skin.label);
      draftStyle.fontSize  = 12;
      draftStyle.alignment = TextAnchor.MiddleLeft;
      styleList.Add("text_basic", new(draftStyle));

      // Category table left header
      draftStyle              = new(HighLogic.Skin.label);
      draftStyle.fontSize     = 14;
      draftStyle.fontStyle    = FontStyle.Bold;
      draftStyle.alignment    = TextAnchor.MiddleCenter;
      draftStyle.stretchWidth = true;

      styleList.Add("panel_header_centered", new(draftStyle));

      // Category table left header
      draftStyle          = new(HighLogic.Skin.label);
      draftStyle.fontSize = 12;

      draftStyle.fontStyle    = FontStyle.Bold;
      draftStyle.alignment    = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("positive_category_header", new(draftStyle));

      // Category table field right
      draftStyle               = new(HighLogic.Skin.label);
      draftStyle.fontSize      = 14;
      draftStyle.alignment     = TextAnchor.MiddleRight;
      draftStyle.padding.right = 5;
      styleList.Add("positive_category_header_field", new(draftStyle));

      // Category table left header
      draftStyle              = new(HighLogic.Skin.label);
      draftStyle.fontSize     = 14;
      draftStyle.fontStyle    = FontStyle.Bold;
      draftStyle.alignment    = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("negative_category_header", new(draftStyle));

      // Category table field right
      draftStyle               = new(HighLogic.Skin.label);
      draftStyle.fontSize      = 14;
      draftStyle.alignment     = TextAnchor.MiddleRight;
      draftStyle.padding.right = 5;
      styleList.Add("negative_category_header_field", new(draftStyle));


      // Category table left header
      draftStyle              = new(HighLogic.Skin.label);
      draftStyle.fontSize     = 14;
      draftStyle.fontStyle    = FontStyle.Bold;
      draftStyle.alignment    = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 3;
      styleList.Add("category_header", new(draftStyle));

      // Category table field right
      draftStyle               = new(HighLogic.Skin.label);
      draftStyle.fontSize      = 14;
      draftStyle.alignment     = TextAnchor.MiddleRight;
      draftStyle.padding.right = 5;
      styleList.Add("category_header_field", new(draftStyle));

      // Data table left header
      draftStyle              = new(HighLogic.Skin.label);
      draftStyle.fontSize     = 14;
      draftStyle.fontStyle    = FontStyle.Bold;
      draftStyle.alignment    = TextAnchor.MiddleLeft;
      draftStyle.padding.left = 5;
      styleList.Add("data_header", new(draftStyle));

      // Comment below data_header line 
      styleList.Add("data_comment",
                    new(new(HighLogic.Skin.label)
                    {
                      fontSize  = 14,
                      fontStyle = FontStyle.Normal,
                      alignment = TextAnchor.MiddleLeft,
                      padding =
                      {
                        left = 5
                      }
                    }));

      // Data table field right
      draftStyle               = new(HighLogic.Skin.label);
      draftStyle.fontSize      = 14;
      draftStyle.padding.right = 3;
      draftStyle.alignment     = TextAnchor.MiddleRight;
      styleList.Add("data_field", new(draftStyle));

      // Data table field right
      draftStyle           = new(HighLogic.Skin.label);
      draftStyle.fontSize  = 16;
      draftStyle.alignment = TextAnchor.MiddleRight;
      styleList.Add("data_field_large", new(draftStyle));
    }

    private void CreateColorList()
    {
      colorList = new();

      colorList.Add("cancel_color",   new(208f / 255f, 131f / 255f, 86f  / 255f));
      colorList.Add("accept_color",   new(209f / 255f, 250f / 255f, 146f / 255f));
      colorList.Add("capacitor_blue", new(134f / 255f, 197f / 255f, 239f / 255f));
      colorList.Add("readout_green",  new(203f / 255f, 238f / 255f, 115f / 255f));
    }
  }

  // Represents an atlased icon via a source texture and rectangle
  public class AtlasIcon
  {
    public Texture iconAtlas;
    public Rect    iconRect;

    public AtlasIcon(Texture theAtlas, float bl_x, float bl_y, float x_size, float y_size)
    {
      iconAtlas = theAtlas;
      iconRect  = new(bl_x, bl_y, x_size, y_size);
    }
  }
}