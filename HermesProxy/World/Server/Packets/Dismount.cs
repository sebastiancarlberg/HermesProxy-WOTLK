using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class Dismount : ServerPacket
{
	public Dismount()
		: base(Opcode.SMSG_DISMOUNT, ConnectionType.Instance)
	{
	}

	public override void Write()
	{
	}
}
