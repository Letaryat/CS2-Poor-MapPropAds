using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CS2_Poor_MapPropAds.Managers;

public class CommandsManager(CS2_Poor_MapPropAds plugin)
{
    private readonly CS2_Poor_MapPropAds _plugin = plugin;

    public void RegisterCommands()
    {
        if (_plugin.Config.EnableCMD)
        {
            _plugin.AddCommand("css_placedecals", "Allow to place advertisements", OnAllowPlacing);
            _plugin.AddCommand("css_setdecal", "Set advertisement model for bullet & ping", OnConfigureAd);
            _plugin.AddCommand("css_pingdecals", "Place advertisement by ping", OnClickAdvertisement);
            _plugin.AddCommand("css_removedecal", "Remove BS Entity using ID", OnRemoveEntity);
            _plugin.AddCommand("css_tpdecal", "Teleport to BS Entity using ID", TeleportToEntity);
            _plugin.AddCommand("css_showdecals", "List of entities on this map", ShowEntityList);
            _plugin.AddCommand("css_printdecals", "List of a models that can be placed", PrintAdModels);
        }
    }

    [CommandHelper(minArgs: 2, usage: "[modelid], [ForceToVip (true / false)]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    private void OnConfigureAd(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || !player!.PlayerPawn.IsValid || player.PlayerPawn.Value == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        if (!AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }

        var model = commandInfo.GetArg(1);
        var forceToVipArg = commandInfo.GetArg(2);
        bool forceToVipParsed = bool.TryParse(forceToVipArg, out bool forceToVip);

        var Count = _plugin.Config.Props.Count();
        if (model == null || Int32.Parse(model) >= Count || !forceToVipParsed)
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoArg"]}");
            return;
        }

        _plugin.DecalAdToPlace = Int32.Parse(model);
        _plugin.ForceOnVip = forceToVip;

        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["SetModel", model, forceToVip]}");

    }
    private void OnAllowPlacing(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || !player!.PlayerPawn.IsValid || player.PlayerPawn.Value == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        if (!AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }

        if (_plugin.AllowAdminCommands)
        {
            _plugin.AllowAdminCommands = false;
        }
        else
        {
            _plugin.AllowAdminCommands = true;
        }

        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["PlacingMode", _plugin.AllowAdminCommands]}");
    }


    private void OnClickAdvertisement(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || !player!.PlayerPawn.IsValid || player.PlayerPawn.Value == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        if (!_plugin.AllowAdminCommands || !AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }

        if (_plugin.PingPlacement)
        {
            _plugin.PingPlacement = false;
        }
        else
        {
            _plugin.PingPlacement = true;
        }
        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["PingMode", _plugin.PingPlacement]}");

    }


    [CommandHelper(minArgs: 1, usage: "[id]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    private void OnRemoveEntity(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || player.PlayerPawn == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        if (!_plugin.AllowAdminCommands || !AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }

        if (commandInfo.ArgCount < 2)
        {
            return;
        }
        var arg = commandInfo.GetArg(1);

        _plugin.PropManager!.RemovePropFromFile(arg);

        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["RemoveEntity"]}");
    }

    [CommandHelper(minArgs: 1, usage: "[id]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    private void TeleportToEntity(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || player.PlayerPawn == null) return;

        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        if (!AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }

        var arg = commandInfo.GetArg(1);
        var id = Convert.ToInt32(arg);
        var prop = _plugin.PropManager!.GetPropById(id);

        var propPos = new Vector(prop!.posX, prop!.posY, prop!.posZ);

        player.PlayerPawn.Value!.Teleport(propPos);

        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["Teleport"]}");
    }


    private void ShowEntityList(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || player.PlayerPawn == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        if (!AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }
        player.PrintToConsole($"| ID | MODELID | POS | ");
        foreach (var entity in _plugin.PropManager!._props)
        {
            player.PrintToConsole($"| {entity.Id} | {entity.ModelIndex}  | X: {entity.posX}, Y: {entity.posY}, Z: {entity.posZ} |");
        }
        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["Console"]}");
    }
    private void PrintAdModels(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || !player!.PlayerPawn.IsValid || player.PlayerPawn.Value == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;
        var i = 0;
        if (!AdminManager.PlayerHasPermissions(player, _plugin.Config.AdminFlag))
        {
            player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["NoAccess"]}");
            return;
        }
        player.PrintToConsole($"| ID | MODEL ");
        foreach (var models in _plugin.Config.Props)
        {
            player.PrintToConsole($"| {i} | {models}");
            i++;
        }
        player.PrintToChat($"{_plugin.Localizer["Prefix"]}{_plugin.Localizer["Console"]}");

    }

}
