using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2_Poor_MapPropAds.Managers;

public class EventManager(CS2_Poor_MapPropAds plugin)
{
    private readonly CS2_Poor_MapPropAds _plugin = plugin;
    public void RegisterEvents()
    {
        //Events:
        _plugin.RegisterEventHandler<EventRoundStart>(OnRoundStart);
        _plugin.RegisterEventHandler<EventPlayerPing>(OnPlayerPing);
        //Listeners:
        _plugin.RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest manifest) =>
        {
            foreach (var prop in _plugin.Config.Props)
            {
                manifest.AddResource(prop);
            }
        });
        _plugin.RegisterListener<Listeners.OnMapStart>(OnMapStart);
        _plugin.RegisterListener<Listeners.CheckTransmit>(OnCheckTransmit);
    }


    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        _plugin.PropManager!.SpawnProps();
        return HookResult.Continue;
    }

    private HookResult OnPlayerPing(EventPlayerPing @event, GameEventInfo info)
    {
        var ping = @event;
        var player = ping.Userid;
        if (player == null) return HookResult.Continue;

        if (!_plugin.AllowAdminCommands || !AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag) || !_plugin.PingPlacement)
        {
            _plugin.DebugMode("Test0");
            return HookResult.Continue;
        }

        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return HookResult.Continue;

        
        _plugin.DebugMode("Test0.5");
        _plugin.PluginUtils!.CreateDecalOnClick(player, new Vector(ping.X, ping.Y, ping.Z), _plugin.ForceOnVip);

        return HookResult.Continue;
    }

    private void OnCheckTransmit(CCheckTransmitInfoList infoList)
    {
        var allAdvs = Utilities.FindAllEntitiesByDesignerName<CPhysicsPropOverride>("prop_physics_override");
        if (allAdvs == null || !allAdvs.Any()) return;
        try
        {
            foreach (var entry in infoList)
            {
                CCheckTransmitInfo info;
                CCSPlayerController? player;

                try
                {
                    (info, player) = ((CCheckTransmitInfo, CCSPlayerController))entry;
                }
                catch
                {
                    continue;
                }

                if (player == null) continue;

                if (AdminManager.PlayerHasPermissions(player, _plugin.Config.VipFlag))
                {
                    foreach (var ad in allAdvs)
                    {
                        //if (ad?.Entity?.Name != null && ad.Entity.Name.Contains("advert"))
                        if (ad.Entity!.Name == null) continue;
                        if (ad!.Entity!.Name.StartsWith("advert") && !ad!.Entity!.Name.Contains("force"))
                        {
                            info.TransmitEntities.Remove(ad);
                        }
                    }
                }
            }
        }
        catch (Exception error)
        {
            _plugin.DebugMode($"CheckTransmit: ${error}");
        }

    }



    private void OnMapStart(string mapName)
    {
        _plugin.PropManager!._props.Clear();
        _plugin.AllowAdminCommands = false;
        _plugin.PingPlacement = false;
        _plugin.DecalAdToPlace = 0;
        Server.NextFrame(() =>
        {
            _plugin.PropManager._mapName = mapName;
            _plugin.PropManager!._mapFilePath = Path.Combine(_plugin.ModuleDirectory, "maps", $"{mapName}.json");

            _plugin.PropManager.GenerateJsonFile();
            Server.NextFrame(() =>
            {
                _plugin.PropManager.LoadPropsFromMap();
            });
        });
    }

}
