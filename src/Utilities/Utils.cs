using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2_Poor_MapPropAds.Utils;

public class PluginUtils(CS2_Poor_MapPropAds plugin)
{
    private readonly CS2_Poor_MapPropAds _plugin = plugin;

    public void CreateDecal(Vector cords, QAngle angle, int index, bool forceOnVip, bool onGround)
    {
        var entity = Utilities.CreateEntityByName<CPhysicsPropOverride>("prop_physics_override");
        if (entity == null) return;

        try
        {
            entity.Entity!.Name = "advert";

            if (forceOnVip)
            {
                entity.Entity!.Name += "force";
            }

            QAngle qangle = new QAngle(0, angle.Y, 0);
            
            entity.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 << 2);

            entity.SetModel(_plugin.Config.Props[index]);

            entity.Teleport(new Vector(cords.X, cords.Y, cords.Z), qangle);

            if (onGround)
            {
                entity.AbsRotation!.X = -90;
            }
            

            entity!.DispatchSpawn();
        }
        catch (Exception error)
        {
            _plugin.DebugMode($"{error}");
        }

    }

    public void CreateDecalOnClick(CCSPlayerController player, Vector position, bool forceOnVip)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        QAngle eyeAngles = player.PlayerPawn.Value!.EyeAngles!;

        float flippedYaw = (eyeAngles.Y + 180.0f) % 360.0f;
        QAngle spriteAngle = new QAngle(eyeAngles.X, flippedYaw, eyeAngles.Z);

        Vector impactPos = new Vector(position.X, position.Y, position.Z);
        Vector backward = -GetForwardVector(eyeAngles);
        backward = Normalize(backward);
        Vector offsetPos = impactPos + backward * 2f;

        double eyeAngleZ = GetPlayerEyeVector(pawn);
        _plugin.DebugMode("Test1");
        try
        {
            if (eyeAngleZ < -0.90)
            {
                offsetPos.Z += 1f;
                CreateDecal(offsetPos, new QAngle(0, spriteAngle.Y, 0), _plugin.DecalAdToPlace, forceOnVip, true);
                _plugin.PropManager!.PushCordsToFile(offsetPos, new QAngle(0, spriteAngle.Y, 0), _plugin.DecalAdToPlace, forceOnVip, true);
            }
            else
            {
                CreateDecal(offsetPos, new QAngle(90, spriteAngle.Y, 0), _plugin.DecalAdToPlace, forceOnVip, false);
                _plugin.PropManager!.PushCordsToFile(offsetPos, new QAngle(90, spriteAngle.Y, 0), _plugin.DecalAdToPlace, forceOnVip, false);
            }
        }
        catch (Exception error)
        {
            _plugin.DebugMode($"{error}");
        }
    }


    public Vector GetForwardVector(QAngle angles)
    {
        float radYaw = angles.Y * (float)(Math.PI / 180.0);
        return new Vector((float)Math.Cos(radYaw), (float)Math.Sin(radYaw), 0);
    }
    public Vector Normalize(Vector vec)
    {
        float length = MathF.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        if (length == 0)
            return new Vector(0, 0, 0);
        return new Vector(vec.X / length, vec.Y / length, vec.Z / length);
    }

    private float GetPlayerEyeVector(CCSPlayerPawn pawn)
    {
        // Credits to: 
        // https://github.com/edgegamers/Jailbreak/blob/main/mod/Jailbreak.Warden/Paint/WardenPaintBehavior.cs#L131
        if (pawn == null || !pawn.IsValid) return 0;
        var eyeAngle = pawn.EyeAngles;
        var pitch = Math.PI / 180 * eyeAngle.X;
        var yaw = Math.PI / 180 * eyeAngle.Y;
        var eyeVector = new Vector((float)(Math.Cos(yaw) * Math.Cos(pitch)), (float)(Math.Sin(yaw) * Math.Cos(pitch)), (float)-Math.Sin(pitch));
        return eyeVector.Z;
    }

}
