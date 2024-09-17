﻿namespace Waterfall
{
  public enum AssetWorkflow
  {
    Dynamic,
    Deforming,
    Billboards,
    Light,
    Volumetric,
    MeshParticles,
    Particles,
    Other
  }

  public class WaterfallAsset
  {
    public string Name = "default";
    public string Description = "default description";
    public string Asset = "";
    public AssetWorkflow Workflow;
    public string        Path;

    public WaterfallAsset() { }

    public WaterfallAsset(ConfigNode node)
    {
      Load(node);
    }

    public virtual void Load(ConfigNode node)
    {
      node.TryGetEnum("workflow", ref Workflow, AssetWorkflow.Other);
      node.TryGetValue("description", ref Description);
      node.TryGetValue("name", ref Name);
      node.TryGetValue("path", ref Path);
      node.TryGetValue("asset", ref Asset);

    }
  }
}