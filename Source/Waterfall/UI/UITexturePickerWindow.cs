using System.Collections.Generic;
using UnityEngine;

namespace Waterfall.UI
{
  public class UITexturePickerWindow : UIPopupWindow
  {
    protected string                      windowTitle        = "";
    protected string                      currentTexturePath = "";
    protected Dictionary<string, Texture> texThumbs;
    protected Vector2                     scrollPos;

    public UITexturePickerWindow(string texToEdit, string currentTexture, bool show) : base(show)
    {
      currentTexturePath = currentTexture;
      GenerateTextures();

      if (!showWindow)
        WindowPosition = new(Screen.width / 2 - 175, Screen.height / 2f, 350, 100);
    }


    public void ChangeTexture(string texToEdit, string currentTexture)
    {
      currentTexturePath = currentTexture;
      GenerateTextures();


      showWindow = true;
      GUI.BringWindowToFront(windowID);
    }

    public string GetTexturePath() => currentTexturePath;

    public void Update() { }

    public void GenerateTextures()
    {
      texThumbs = new();
      foreach (var asset in WaterfallAssets.Textures)
      {
        texThumbs.Add(asset.Path, GameDatabase.Instance.GetTexture(asset.Path, false));
      }
    }

    protected override void InitUI()
    {
      windowTitle = "Texture Picker";
      base.InitUI();
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      DrawTextures();
      GUI.DragWindow();
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      var buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawTextures()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("<b>Selected Texture</b>");
      var bRect = GUILayoutUtility.GetRect(64f, 64f);
      if (texThumbs != null && currentTexturePath != null)
        GUI.DrawTexture(bRect, texThumbs[currentTexturePath]);
      GUILayout.Label(currentTexturePath);
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

      GUILayout.BeginVertical();
      GUILayout.Label("<b>Available Textures</b>");
      scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));
      foreach (var asset in WaterfallAssets.Textures)
      {
        DrawTextureButton(asset);
      }

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }

    protected void DrawTextureButton(WaterfallAsset texture)
    {
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
      {
        currentTexturePath = texture.Path;
      }

      var bRect = GUILayoutUtility.GetLastRect();
      if (texThumbs != null)
        GUI.DrawTexture(bRect, texThumbs[texture.Path]);
      GUILayout.Label(texture.Name);
      GUILayout.EndHorizontal();
    }
  }
}