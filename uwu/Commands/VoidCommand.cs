using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UWU.Common;

namespace UWU.Commands
{
  internal class VoidCommand : ConsoleCommand
  {
    private readonly Action<string[]> action;
    private readonly bool adminOnly;
    private readonly string name;
    private readonly string help;
    private readonly bool isCheat;

    internal VoidCommand(string name, string help, bool adminOnly, bool isCheat, Action<string[]> action)
    {
      this.name = name;
      this.help = help;
      this.adminOnly = adminOnly;
      this.isCheat = isCheat;
      this.action = action;
    }

    public override string Name => name;
    public override string Help => help;
    public override bool IsCheat => isCheat;
    public override List<string> CommandOptionList() => new();

    public override void Run(string[] args)
    {
      if (adminOnly && !SynchronizationManager.Instance.PlayerIsAdmin)
      {
        UserHud.Alert($"{Name} can only be set by an Admin");
        return;
      }

      action(args);
    }
  }
}