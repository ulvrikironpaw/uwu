using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;

namespace UWU
{
    internal class BoolConsoleCommand : ConsoleCommand
    {
        private readonly Action<bool> configAction;
        private readonly bool adminOnly;
        private readonly string name;
        private readonly string help;
        private readonly bool isCheat;

        internal BoolConsoleCommand(string name, string help, bool adminOnly, bool isCheat, Action<bool> configAction)
        {
            this.name = name;
            this.help = help;
            this.adminOnly = adminOnly;
            this.isCheat = isCheat;
            this.configAction = configAction;
        }

        public override string Name => name;
        public override string Help => help;
        public override bool IsCheat => isCheat;
        public override List<string> CommandOptionList() => new();

        public override void Run(string[] args)
        {
            if (adminOnly && !SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Jotunn.Logger.LogWarning($"{Name} can only be set by an Admin");
                return;
            }

            if (args.Length == 0)
            {
                Jotunn.Logger.LogWarning($"{Name} not set. Missing value");
                return;
            }

            switch (args[0].ToLower())
            {
                case "true":
                    configAction(true);
                    Jotunn.Logger.LogInfo($"{Name} set to {true}");
                    return;
                case "false":
                    configAction(false);
                    Jotunn.Logger.LogInfo($"{Name} set to {false}");
                    return;
                default:
                    Jotunn.Logger.LogWarning($"{Name} not set. Invalid value");
                    return;
            }
        }
    }

    internal class FloatConsoleCommand : ConsoleCommand
    {
        private readonly Action<float> configAction;
        private readonly bool adminOnly;
        private readonly string name;
        private readonly string help;
        private readonly bool isCheat;

        internal FloatConsoleCommand(string name, string help, bool adminOnly, bool isCheat, Action<float> configAction)
        {
            this.name = name;
            this.help = help;
            this.adminOnly = adminOnly;
            this.isCheat = isCheat;
            this.configAction = configAction;
        }

        public override string Name => name;
        public override string Help => help;
        public override bool IsCheat => isCheat;
        public override List<string> CommandOptionList() => new();

        public override void Run(string[] args)
        {
            if (adminOnly && !SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Jotunn.Logger.LogWarning($"{Name} can only be set by an Admin");
                return;
            }

            if (args.Length == 0)
            {
                Jotunn.Logger.LogWarning($"{Name} not set. Missing value");
                return;
            }

            try
            {
                var floatValue = float.Parse(args[0]);
                configAction(floatValue);
                Jotunn.Logger.LogInfo($"{Name} set to {floatValue}");
            }
            catch
            {

                Jotunn.Logger.LogWarning($"{Name} not set. Invalid value");
            }
        }
    }
}