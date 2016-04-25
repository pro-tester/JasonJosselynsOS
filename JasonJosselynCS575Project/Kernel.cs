using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace JasonJosselynCS575Project

{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Jason Josselyn's CS575 Project OS!");
            Console.WriteLine("---------------------------------------------");
        }

        protected override void Run()
        {
            Console.Write("Password: ");
            string input = Console.ReadLine();
            while (input != "Passw0rd")
            {
                Console.WriteLine("Incorrect password. Please try again.");
                Console.Write("Password: ");
                input = Console.ReadLine();
            }
            TopLevelCommandsFactory.PrintCommands();
            while (true)
            {
                Console.Write("/>");
                InputHelper inputHelper = new InputHelper(Console.ReadLine());
                string command = inputHelper.Command;
                string parameters = inputHelper.Parameters;
                TopLevelCommandsFactory.CreateCommandType(command).CreateCommand(parameters).Execute();
            }
        }
    }

    class InputHelper
    {
        public string Command { get; }
        public string Parameters { get; }
        public InputHelper(string input)
        {
            Command = "";
            Parameters = "";
            int partOfCommand = 1;
            foreach (Char c in input)
            {
                if (c != ' ' && partOfCommand == 1)
                {
                    Command += c;
                }
                else if (c == ' ' && partOfCommand == 1)
                {
                    partOfCommand = 0;
                }
                else
                {
                    Parameters += c;
                }
            }
        }
    }

    class CommandsFactory
    {
        private Creator[] creators;

        public CommandsFactory(Creator[] creators)
        {
            this.creators = creators;
        }

        public Creator CreateCommandType(string command)
        {
            foreach (Creator creator in creators)
            {
                if (creator.Command == command)
                {
                    return creator;
                }
            }
            return new ConcreteCreatorHelp();
        }

        public void PrintCommands()
        {
            Console.WriteLine("Supported commands:");
            foreach (Creator creator in creators)
            {
                Console.WriteLine(creator.Command + ": " + creator.Description);
            }
        }
    }

    static class TopLevelCommandsFactory
    {
        private static CommandsFactory commandFactory = new CommandsFactory(new Creator[] { new ConcreteCreatorAbout(), new ConcreteCreatorClearScreen(), new ConcreteCreatorEcho(), new ConcreteCreatorHelp(), new ConcreteCreatorSettings(), new ConcreteCreatorReboot() });

        public static Creator CreateCommandType(string command)
        {
            return commandFactory.CreateCommandType(command);
        }

        public static void PrintCommands()
        {
            commandFactory.PrintCommands();
        }
    }

    static class SettingsLevelCommandsFactory
    {
        private static CommandsFactory commandFactory = new CommandsFactory(new Creator[] { new ConcreteSettingsCreatorBackgroundColor(), new ConcreteSettingsCreatorTextColor(), new ConcreteSettingsCreatorHelp() });

        public static Creator CreateCommandType(string command)
        {
            return commandFactory.CreateCommandType(command);
        }

        public static void PrintCommands()
        {
            commandFactory.PrintCommands();
        }
    }

    abstract class Command
    {
        protected string parameters;
        protected Command()
        {
            this.parameters = "";
        }
        protected Command(string parameters)
        {
            this.parameters = parameters;
        }
        public virtual void Execute()
        {
            Console.WriteLine("This command has not been implmented.");
        }
    }

    class ConcreteCommandAbout : Command
    {
        public override void Execute()
        {
            Console.WriteLine("OS: Jason Josselyn's CS575 Project");
            Console.WriteLine("Version: 1.0.0.0");
        }
    }

    class ConcreteCommandClearScreen : Command
    {
        public override void Execute()
        {
            Console.Clear();
        }
    }

    class ConcreteCommandEcho : Command
    {
        public ConcreteCommandEcho(string parameters) : base(parameters) { }
        public override void Execute()
        {
            Console.WriteLine(parameters);
        }
    }

    class ConcreteCommandHelp : Command
    {
        public override void Execute()
        {
            TopLevelCommandsFactory.PrintCommands();
        }
    }

    class ConcreteCommandSettings : Command
    {
        public ConcreteCommandSettings(string parameters) : base(parameters) { }
        public override void Execute()
        {
            if (parameters == "")
            {
                SettingsLevelCommandsFactory.PrintCommands();
            }
            else
            {
                InputHelper inputHelper = new InputHelper(parameters);
                string settingsCommand = inputHelper.Command;
                string settingsParameters = inputHelper.Parameters;
                SettingsLevelCommandsFactory.CreateCommandType(settingsCommand).CreateCommand(settingsParameters).Execute();
            }
        }
    }

    class ConcreteSettingsCommandBackgroundColor : Command
    {
        public ConcreteSettingsCommandBackgroundColor(string parameters) : base(parameters) { }
        public override void Execute()
        {
            ConsoleColor consoleColor = ConsoleColorFactory.GetColor(parameters);
            if (consoleColor == ConsoleColor.DarkCyan)
            {
                Console.WriteLine(ConsoleColorFactory.GetColorsArrayString());
            }
            else
            {
                Console.BackgroundColor = consoleColor;
            }
        }
    }

    class ConcreteSettingsCommandTextColor : Command
    {
        public ConcreteSettingsCommandTextColor(string parameters) : base(parameters) { }
        public override void Execute()
        {
            ConsoleColor consoleColor = ConsoleColorFactory.GetColor(parameters);
            if (consoleColor == ConsoleColor.DarkCyan)
            {
                Console.WriteLine(ConsoleColorFactory.GetColorsArrayString());
            }
            else
            {
                Console.ForegroundColor = consoleColor;
            }
        }
    }

    class ConcreteSettingsCommandHelp : Command
    {
        public override void Execute()
        {
            SettingsLevelCommandsFactory.PrintCommands();
        }
    }

    class ConcreteCommandReboot : Command
    {
        public override void Execute()
        {
            Sys.Power.Reboot();
        }
    }

    abstract class Creator
    {
        public abstract string Command { get; }
        public abstract string Description { get; }
        public abstract Command CreateCommand(string parameters);
    }

    class ConcreteCreatorAbout : Creator
    {
        public override string Command
        {
            get { return "about"; }
        }
        public override string Description
        {
            get { return "Displays the version information about this operating system"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteCommandAbout();
        }
    }

    class ConcreteCreatorClearScreen : Creator
    {
        public override string Command
        {
            get { return "cls"; }
        }
        public override string Description
        {
            get { return "Clears the screen"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteCommandClearScreen();
        }
    }

    class ConcreteCreatorEcho : Creator
    {
        public override string Command
        {
            get { return "echo"; }
        }
        public override string Description
        {
            get { return "Returns the text you enter after it"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteCommandEcho(parameters);
        }
    }

    class ConcreteCreatorHelp : Creator
    {
        public override string Command
        {
            get { return "help"; }
        }
        public override string Description
        {
            get { return "Displays this text"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteCommandHelp();
        }
    }

    class ConcreteCreatorSettings : Creator
    {
        public override string Command
        {
            get { return "settings"; }
        }
        public override string Description
        {
            get { return "Returns a walkthrough of adjusting basic display and mouse settings"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteCommandSettings(parameters);
        }
    }

    class ConcreteCreatorReboot : Creator
    {
        public override string Command
        {
            get { return "reboot"; }
        }
        public override string Description
        {
            get { return "Reboots the machine"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteCommandReboot();
        }
    }

    class ConcreteSettingsCreatorBackgroundColor : Creator
    {
        public override string Command
        {
            get { return "background_color"; }
        }
        public override string Description
        {
            get { return "adjusts background color using parameter string of color"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteSettingsCommandBackgroundColor(parameters);
        }
    }

    class ConcreteSettingsCreatorTextColor : Creator
    {
        public override string Command
        {
            get { return "text_color"; }
        }
        public override string Description
        {
            get { return "adjusts text color using parameter string of color"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteSettingsCommandTextColor(parameters);
        }
    }

    class ConcreteSettingsCreatorHelp : Creator
    {
        public override string Command
        {
            get { return "help"; }
        }
        public override string Description
        {
            get { return "Displays this menu about settings options"; }
        }
        public override Command CreateCommand(string parameters)
        {
            return new ConcreteSettingsCommandHelp();
        }
    }

    static class ConsoleColorFactory
    {
        private static string[] consoleColorsStrings = new string[] { "Black", "Blue", "Cyan", "DarkBlue", "DarkGray", "DarkGreen", "DarkMagenta", "DarkRed", "DarkYellow", "Gray", "Green", "Magenta", "Red", "White", "Yellow" };
        private static ConsoleColor[] consoleColors = new ConsoleColor[] { ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.DarkBlue, ConsoleColor.DarkGray, ConsoleColor.DarkGreen, ConsoleColor.DarkMagenta, ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Gray, ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Yellow };

        public static ConsoleColor GetColor(string input)
        {
            for (int index = 0; index < consoleColorsStrings.Length; index++)
            {
                string consoleColorString = consoleColorsStrings[index];
                if (input == consoleColorString)
                {
                    return consoleColors[index];
                }
            }
            return ConsoleColor.DarkCyan;
        }
        public static string GetColorsArrayString()
        {
            string extraColorString = "";
            foreach (string consoleColorString in consoleColorsStrings)
            {
                extraColorString += consoleColorString;
                extraColorString += ", ";
            }
            string colorString = "";
            for (int index=0; index<extraColorString.Length-2; index++)
            {
                colorString += extraColorString[index];
            }
            return colorString;
        }
    }
}