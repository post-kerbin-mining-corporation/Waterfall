using System.Collections.Generic;
using UnityEngine;

namespace Waterfall.UI
{

  public delegate void TextureUpdateFunction(string texturePath);

  public class UITexturePickerWindow : UIPopupWindow
  {
    protected string                      windowTitle        = "";
    protected string                      currentTexturePath = "";
    protected Dictionary<string, Texture> texThumbs;
    protected Vector2                     scrollPos;

    internal TextureUpdateFunction function;

    public UITexturePickerWindow(string currentTexture, bool show, TextureUpdateFunction fun) : base(show)
    {
      ChangeTexture(currentTexture, fun);

      if (!showWindow)
        WindowPosition = new(Screen.width / 2 - 175, Screen.height / 2f, 350, 100);
    }


    public void ChangeTexture(string currentTexture, TextureUpdateFunction fun)
    {
      currentTexturePath = currentTexture;
      function = fun;
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
      GUILayout.Label(windowTitle, UIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      var buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = UIResources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", UIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }
      buttonRect = new Rect(buttonRect.x+5, buttonRect.y+5, buttonRect.width-10, buttonRect.height-10);
      GUI.DrawTextureWithTexCoords(buttonRect, UIResources.GetIcon("cancel").iconAtlas, UIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawTextures()
    {
      GUILayout.BeginHorizontal(GUI.skin.textArea);
      GUILayout.Label("<b>SELECTED</b>");
      var bRect = GUILayoutUtility.GetRect(64f, 64f);
      if (texThumbs != null && currentTexturePath != null)
        GUI.DrawTexture(bRect, texThumbs[currentTexturePath]);
      GUILayout.BeginVertical();
      GUILayout.FlexibleSpace();
      GUILayout.Label(currentTexturePath);
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();

      GUILayout.BeginVertical();
      GUILayout.Label("<b>LOADED TEXTURES</b>");
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
      GUILayout.BeginHorizontal(GUI.skin.textArea);
      if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
      {
        currentTexturePath = texture.Path;
        function(currentTexturePath);
      }

      var buttonRect = GUILayoutUtility.GetLastRect();
      buttonRect = new Rect(buttonRect.x + 5, buttonRect.y + 5, buttonRect.width - 10, buttonRect.height - 10);
      if (texThumbs != null)
      {
        GUI.DrawTexture(buttonRect, texThumbs[texture.Path]);
      }
      GUILayout.BeginVertical();
      GUILayout.Label($"<b>{texture.Name}</b>");
      GUILayout.Label(texture.Path);
      GUILayout.Label(texture.Description);
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }
  }
}