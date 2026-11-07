using System;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class TeleportObject : MonoBehaviour
{
	private void Start()
	{
		_mapEditorObject = GetComponent<MapEditorObject>();
		Base = (SerializableTeleport)_mapEditorObject.Base;
		List.Add(this);
	}
	public static List<TeleportObject> List { get; } = new List<TeleportObject>();
	

	public SerializableTeleport Base;
	private MapEditorObject _mapEditorObject;

	public Dictionary<Player, DateTime> NextTimeUse = new ();

	public TeleportObject? GetRandomTarget()
	{
		string targetId = Base.Targets.RandomItem();

		foreach (TeleportObject teleportObject in FindObjectsByType<TeleportObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (teleportObject._mapEditorObject.Id != targetId)
				continue;

			return teleportObject;
		}

		return null;
	}

	public void OnTriggerEnter(Collider other)
	{
		CheckPlayer(other);
		CheckCheckPickup(other);
	}
	
	internal readonly List<ushort> TeleportedPickups = new List<ushort>();
	private void CheckCheckPickup(Collider other)
	{
		
		if(!other.transform.root.TryGetComponent(out ItemPickupBase pickupBase))
			return;
		
		var target = GetRandomTarget();
		if(target == null)
			return;
		var pickup = Pickup.Get(pickupBase);

		if(TeleportedPickups.Contains(pickup.Serial))
			return;
		
		if(!target.TeleportedPickups.Contains(pickup.Serial)) 
			target.TeleportedPickups.Add(pickup.Serial);
		
		Logger.Info($"Teleporting pickup {pickupBase.name} ({pickup.Serial}) to {target._mapEditorObject.Id}");


		pickup.Position = target?.transform.position ?? pickup.Position;
	}

	private void CheckPlayer(Collider other)
	{
		Player? player = Player.Get(other.gameObject);
		if (player is null)
			return;

		if (NextTimeUse.TryGetValue(player, out DateTime nextTime) && nextTime > DateTime.Now)
			return;

		TeleportObject? target = GetRandomTarget();
		if (target == null)
			return;

		DateTime dateTime = DateTime.Now.AddSeconds(Base.Cooldown);
		NextTimeUse[player] = dateTime;
		target.NextTimeUse[player] = dateTime;

		player.Position = target.gameObject.transform.position;
		player.LookRotation = target.gameObject.transform.eulerAngles;
	}
}
