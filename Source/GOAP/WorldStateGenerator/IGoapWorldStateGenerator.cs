﻿using Godot;
using GodotGOAPAI.Source.GOAP.Abstraction;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.GOAP.WorldStateGenerator;

public interface IGoapWorldStateGenerator
{
    GoapWorldStateModel GenerateWorldStateModel(IGameObject worldCollectionsRootNode);
}