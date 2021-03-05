using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;


namespace Waterfall.UI
{
  public class UITexturePickerWindow : UIPopupWindow
  {
    
    protected string windowTitle = "";
    protected string currentTexturePath = "";
    protected Dictionary<string, Texture> texThumbs;

    public UITexturePickerWindow(string texToEdit, string currentTexture, bool show) : base(show)
    {
      currentTexturePath = currentTexture;
      GenerateTextures();
      WindowPosition = new Rect(Screen.width / 2 - 175, Screen.height / 2f, 350, 100);
    }

    protected override void InitUI()
    {
      windowTitle = "Texture Picker";
      base.InitUI();
    }


    public void ChangeTexture(string texToEdit, string currentTexture)
    {
      currentTexturePath = currentTexture;
      GenerateTextures();
      WindowPosition = new Rect(Screen.width / 2 - 175, Screen.height / 2f,350, 100);
      
      showWindow = true;
    }

    public string GetTexturePath()
    {
      

      return currentTexturePath;
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
    protected void DrawTextures()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("<b>Selected Texture</b>");
      
      Rect bRect = GUILayoutUtility.GetRect(64f,64f);
      if (texThumbs != null && currentTexturePath != null)
        GUI.DrawTexture(bRect, texThumbs[currentTexturePath]);
      GUILayout.Label(currentTexturePath);
      GUILayout.EndHorizontal();
      GUILayout.BeginVertical();
      GUILayout.Label("<b>Available Textures</b>");
      foreach (WaterfallAsset asset in WaterfallAssets.Textures)
      {
        DrawTextureButton(asset);
      }
      GUILayout.EndVertical();
    }
    protected void DrawTextureButton(WaterfallAsset texture)
    {
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
      {
        currentTexturePath = texture.Path;
      }
      Rect bRect = GUILayoutUtility.GetLastRect();
      if (texThumbs != null)
        GUI.DrawTexture(bRect, texThumbs[texture.Path]);
      GUILayout.Label(texture.Name);
      GUILayout.EndHorizontal();
    }
   
    public void Update()
    {
     
    }

    public void GenerateTextures()
    {
      texThumbs = new Dictionary<string, Texture>();
      foreach (WaterfallAsset asset in WaterfallAssets.Textures)
      {
        texThumbs.Add(asset.Path, GameDatabase.Instance.GetTexture(asset.Path, false));
      }
    }
  }
}

