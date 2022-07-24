using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class RemapControllerUIOptions : DefaultEffectControllerUIOptions<RemapController>
  {
    private readonly Vector2 curveButtonDims = new(100f, 50f);

    private readonly int texWidth = 80;
    private readonly int texHeight = 30;
    private FloatCurve mappingCurve;
    private string sourceController;
    private Texture2D miniCurve;
    private UICurveEditWindow curveEditor;

    public RemapControllerUIOptions()
    {
      sourceController = "";
      mappingCurve = new();
      mappingCurve.Add(0f, 0f);
      mappingCurve.Add(1f, 1f);
      GenerateCurveThumbs();
    }

    public override void DrawOptions()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Source controller", UIResources.GetStyle("data_header"));
      sourceController = GUILayout.TextArea(sourceController);
      GUILayout.EndHorizontal();

      var buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      var imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(mappingCurve, UpdateEventCurve);
      }

      GUI.DrawTexture(imageRect, miniCurve);
    }

    protected override void LoadOptions(RemapController controller)
    {
      mappingCurve = controller.mappingCurve;
      sourceController = controller.sourceController;

      GenerateCurveThumbs();
    }

    protected override RemapController CreateControllerInternal() =>
      new()
      {
        sourceController = sourceController,
        mappingCurve = mappingCurve,
      };

    private void EditCurve(FloatCurve toEdit, CurveUpdateFunction function)
    {
      Utils.Log($"Started editing curve {toEdit.Curve}", LogType.UI);
      curveEditor = WaterfallUI.Instance.OpenCurveEditor(toEdit, function);
    }

    private void GenerateCurveThumbs()
    {
      miniCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, mappingCurve, Color.green);
    }

    private void UpdateEventCurve(FloatCurve curve)
    {
      mappingCurve = curve;
      GenerateCurveThumbs();
    }
  }
}
