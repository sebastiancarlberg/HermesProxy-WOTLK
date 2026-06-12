using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildPermissionsQueryResults : ServerPacket
{
    public uint RankID;
    public int Flags;
    public int WithdrawGoldLimit;
    public int NumTabs;
    public List<GuildRankTabPermissions> Tab = new List<GuildRankTabPermissions>();

    public class GuildRankTabPermissions
    {
        public int Flags;
        public int WithdrawItemLimit;
    }

    public GuildPermissionsQueryResults()
        : base(Opcode.SMSG_GUILD_PERMISSIONS_QUERY_RESULTS, ConnectionType.Realm)
    {
    }

    public override void Write()
    {
        base._worldPacket.WriteUInt32(this.RankID);
        base._worldPacket.WriteInt32(this.Flags);
        base._worldPacket.WriteInt32(this.WithdrawGoldLimit);
        base._worldPacket.WriteInt32(this.NumTabs);
        base._worldPacket.WriteUInt32((uint)this.Tab.Count);
        foreach (GuildRankTabPermissions tab in this.Tab)
        {
            base._worldPacket.WriteInt32(tab.Flags);
            base._worldPacket.WriteInt32(tab.WithdrawItemLimit);
        }
    }
}
