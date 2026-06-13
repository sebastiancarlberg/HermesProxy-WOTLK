using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryQuestCompletionNPCs : ClientPacket
{
	public List<int> QuestCompletionNPCs = new List<int>();

	public QueryQuestCompletionNPCs(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint count = base._worldPacket.ReadUInt32();
		if (count > 100u)
		{
			count = 100u;
		}
		for (uint i = 0u; i < count; i++)
		{
			this.QuestCompletionNPCs.Add(base._worldPacket.ReadInt32());
		}
	}
}

public class QuestCompletionNPCResponse : ServerPacket
{
	public List<int> QuestIDs = new List<int>();

	public QuestCompletionNPCResponse()
		: base(Opcode.SMSG_QUEST_COMPLETION_NPC_RESPONSE, ConnectionType.Instance)
	{
	}

	public override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.QuestIDs.Count);
		foreach (int questId in this.QuestIDs)
		{
			base._worldPacket.WriteInt32(questId);
			base._worldPacket.WriteUInt32(0u);
		}
	}
}
