namespace Waterfall.UI
{
  public class UIWidget
  {
    private readonly UIBaseWindow uiHost;

    public UIWidget(UIBaseWindow uiBase)
    {
      uiHost = uiBase;

      Localize();
    }

    public UIBaseWindow UIHost => uiHost;

    protected virtual void Localize() { }
  }
}