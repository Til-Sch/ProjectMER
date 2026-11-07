using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.ToolGun;

namespace ProjectMER.Events.Handlers.Internal;

public class TeleporterEventsHandler : CustomEventsHandler
{
    public override void OnPlayerPickedUpItem(PlayerPickedUpItemEventArgs ev)
    {
        foreach (var x in TeleportObject.List)
        {
            x.TeleportedPickups.Remove(ev.Item.Serial);
        }
    }

}
