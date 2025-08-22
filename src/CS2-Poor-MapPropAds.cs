using CounterStrikeSharp.API.Core;
using CS2_Poor_MapPropAds.Config;
using CS2_Poor_MapPropAds.Managers;
using CS2_Poor_MapPropAds.Utils;
using Microsoft.Extensions.Logging;


namespace CS2_Poor_MapPropAds;


public class CS2_Poor_MapPropAds : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "CS2_Poor_MapPropAds";

    public override string ModuleVersion => "1.0";

    public override string ModuleAuthor => "Letaryat | github.com/letaryat";

    public override string ModuleDescription => "Creates decals with advertisements that can be placed on map.";

    public required PluginConfig Config { get; set; }

    public static CS2_Poor_MapPropAds? Instance { get; private set; }

    public EventManager? EventManager { get; private set; }
    public PropManager? PropManager { get; private set; }

    public PluginUtils? PluginUtils { get; private set; }
    public CommandsManager? CommandsManager { get; private set; }

    public bool AllowAdminCommands = false;
    public bool PingPlacement = false;
    public int DecalAdToPlace = 0;
    public float DecalWidth = 128;
    public float DecalHeight = 128;
    public bool ForceOnVip = false;

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Loaded CS2_Poor_MapPropAds");
        Instance = this;

        EventManager = new EventManager(this);
        PluginUtils = new PluginUtils(this);
        CommandsManager = new CommandsManager(this);
        PropManager = new PropManager(this);

        EventManager.RegisterEvents();
        CommandsManager.RegisterCommands();

    }

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
    }
    public override void Unload(bool hotReload)
    {
        Console.WriteLine("Unloaded CS2_Poor_MapPropAds");
    }

    public void DebugMode(string message)
    {
        if (Config.Debug)
        {
            Logger.LogInformation(message);
        }
    }

}
