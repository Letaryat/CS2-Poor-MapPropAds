using CounterStrikeSharp.API.Modules.Utils;
using CS2_Poor_MapPropAds.Models;
using System.Text.Json;

namespace CS2_Poor_MapPropAds.Managers
{
    public class PropManager(CS2_Poor_MapPropAds plugin)
    {
        private readonly CS2_Poor_MapPropAds _plugin = plugin;
        public string? _mapName;
        public string? _mapFilePath;
        public readonly List<PropModel> _props = [];
        private static readonly object _fileLock = new();

        public void GenerateJsonFile()
        {
            string directoryPath = Path.Combine(_plugin.ModuleDirectory, "maps");
            try
            {
                if(!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                if(!File.Exists(_mapFilePath))
                {
                    File.WriteAllText(_mapFilePath!, "[]");
                }
            }
            catch(Exception e)
            {
                _plugin.DebugMode($"{e}");
            }
        }

        public void PushCordsToFile(Vector pos, QAngle angle, int newIndex, bool forceToVip, bool onGround)
        {
            lock (_fileLock)
            {
                if (pos == null || angle == null) return;
                int newId = _props.Count();

                _props.Add(new PropModel
                {
                    Id = newId,
                    ModelIndex = newIndex,
                    posX = pos.X,
                    posY = pos.Y,
                    posZ = pos.Z,
                    angleX = angle.X,
                    angleY = angle.Y,
                    angleZ = angle.Z,
                    forceOnVip = forceToVip,
                    isOnGround = onGround
                });
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_mapFilePath!, JsonSerializer.Serialize(_props, options));
            }
        }

        public void LoadPropsFromMap()
        {
            if (File.Exists(_mapFilePath))
            {
                string json = File.ReadAllText(_mapFilePath);
                if(!string.IsNullOrEmpty(json))
                {
                    _props.Clear();
                    var loadedProps = JsonSerializer.Deserialize<List<PropModel>>(json) ?? [];
                    _props.AddRange(loadedProps);
                }
            }

        }

        public void SpawnProps()
        {
            foreach (var prop in _props)
            {
                _plugin.PluginUtils!.CreateDecal(new Vector(prop.posX, prop.posY, prop.posZ), new QAngle(prop.angleX, prop.angleY, prop.angleZ), prop.ModelIndex, prop.forceOnVip, prop.isOnGround ? true : false);
            }
        }

        public PropModel? GetPropById(int id)
        {
            return id >= 0 && id < _props.Count ? _props[id] : null;
        }
        public void RemovePropFromFile(string idstring)
        {
            int id = Convert.ToInt32(idstring);
            if (id < 0 || id >= _props.Count()) return;
            _props.RemoveAt(id);

            for (int i = 0; i < _props.Count; i++)
            {
                _props[i].Id = i;
            }
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_mapFilePath!, JsonSerializer.Serialize(_props, options));
        }
    }
}
