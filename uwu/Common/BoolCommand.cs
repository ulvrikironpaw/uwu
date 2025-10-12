using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;

namespace UWU.Common
{
  internal class BoolCommand : ConsoleCommand
  {
    private readonly Func<bool> getValue;
    private readonly Action<bool> setValue;
    private readonly bool adminOnly;
    private readonly string name;
    private readonly string help;
    private readonly bool isCheat;

    internal BoolCommand(string name, string help, bool adminOnly, bool isCheat, Func<bool> getValue, Action<bool> setValue)
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
        MessageHud.instance?.ShowMessage(
            MessageHud.MessageType.Center,
            $"{Name} can only be set by an Admin");
        return;
      }

      if (args.Length == 0)
      {
        MessageHud.instance?.ShowMessage(
            MessageHud.MessageType.Center,
            $"{Name} is {getValue()}");
        return;
      }

      switch (args[0].ToLower())
      {
        case "true":
          setValue(true);
          MessageHud.instance?.ShowMessage(
              MessageHud.MessageType.Center,
              $"{Name} set to {true}");
          return;
        case "false":
          setValue(false);
          Jotunn.Logger.LogInfo($"{Name} set to {false}");
          return;
        default:
          MessageHud.instance?.ShowMessage(
              MessageHud.MessageType.Center,
              $"{Name} not set. Invalid value");
          return;
      }
    }
  }
}