For importing project there are 3 folders with used assets, which is in folders "Plugins", "Graphics" and "Procedural Worlds" which should be moved to the project directory.

**Modifications to restore on assets updates:**

Create asmdef ProceduralWorlds in "Procedural Worlds" directory for separating Gaia and GeNa plugins from main game code.

Modify SRDebugger StandardConsoleService method to this for flashing on all log types:
    
    private void AdjustCounter(LogType type, int amount)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    ErrorCount += amount;

                    if (Error != null)
                    {
                        Error.Invoke(this);
                    }
                    break;

                case LogType.Warning:
                    WarningCount += amount;
                    
                    // Flash error on all log types
                    if (Error != null)
                    {
                        Error.Invoke(this);
                    }
                    break;

                case LogType.Log:
                    InfoCount += amount;
                    
                    // Flash error on all log types
                    if (Error != null)
                    {
                        Error.Invoke(this);
                    }
                    break;
            }
        }