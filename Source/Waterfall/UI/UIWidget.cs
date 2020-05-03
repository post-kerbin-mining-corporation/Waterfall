using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIWidget
  {

    UIBaseWindow uiHost;

    public UIBaseWindow UIHost { get { return uiHost; } }

    public UIWidget(UIBaseWindow uiBase)
    {
      uiHost = uiBase;

      Localize();

    }

    protected virtual void Localize()
    {

    }

  }

}
