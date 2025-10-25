using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UWU.Common;

namespace UWU.Commands
{
  internal class StringCommand : ConsoleCommand
  {
    private readonly Func<string> getValue;
    private readonly Action<string> setValue;
    private readonly bool adminOnly;
    private readonly string name;
    private readonly string help;
    private readonly bool isCheat;

    internal StringCommand(
      string name,
      string help,
      bool adminOnly,
      bool isCheat,
      Func<string> getValue,
      Action<string> setValue)
    {
      this.name = name;
      this.help = help;
      this.adminOnly = adminOnly;
      this.isCheat = isCheat;
      this.getValue = getValue;
      this.setValue = setValue;
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

      if (args.Length == 0)
      {
        UserHud.Alert($"{Name} is {getValue()}");
        return;
      }

      var stringValue = args[0];
      setValue(stringValue);

      UserHud.Alert($"{Name} set to {stringValue}");
    }
  }
}