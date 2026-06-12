namespace HermesProxy.World.Server.Packets;

internal class RequestPartyJoinUpdates : ClientPacket
{
	public RequestPartyJoinUpdates(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
	}
}
