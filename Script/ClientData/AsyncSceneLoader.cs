using System;
using System.Threading.Tasks;
using Godot;

namespace Beyondourborders.Script.ClientData;

public class AsyncSceneLoader {
    private bool _isProcessing = false;
    private readonly object Lock = new ();

    public void LoadZone(string zoneName, Action<Node2D, object[]?> callback, params object[]? res) {
        Load($"res://Scene/Terrain/{zoneName}.tscn", callback, res);
    }
    public void Load(string scenePath,  Action<Node2D, object[]?> callback, params object[]? res)
    {
        lock (Lock)
        {
            if (_isProcessing)
            {
                throw new InvalidOperationException("A task is already in progress.");
            }

            _isProcessing = true;
        }
        
        Task.Run(() =>
        {
            try
            {
                var loaded = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<Node2D>(); 
                callback?.Invoke(loaded, res);
            }
            finally
            {
                lock (Lock)
                {
                    _isProcessing = false;
                }
            }
        });
    }
}